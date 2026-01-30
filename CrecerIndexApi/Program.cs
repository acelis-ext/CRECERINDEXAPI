using CrecerIndex.Abstraction.DependencyInjection;
using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Abstraction.Interfaces.IServices;
using CrecerIndex.Entities.Models;
using CrecerIndex.Repository;
using CrecerIndex.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === TIMEOUT CONFIGURATION ===
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(3);
});

// ? Mantener Oracle “caliente”
builder.Services.AddHostedService<OracleKeepAliveHostedService>();


// Inyección automática por convención
RepositorySearchService.RegistrarRepository(builder.Services);
RepositorySearchService.RegistrarServices(builder.Services);

// Settings
builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("Recaptcha"));

// Typed HttpClient para IRecaptchaService
builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>(c =>
{
    c.BaseAddress = new Uri("https://www.google.com/recaptcha/api/");
    c.Timeout = TimeSpan.FromSeconds(60);
});

// Necesario si aún no lo tenías
builder.Services.AddHttpContextAccessor();


// ===== CORS =====
// O bien leer desde config (Security:AllowedOrigins) o dejar tus dominios quemados.
// Mantengo tus dominios tal cual y de paso habilito lectura desde config si quieres.
var cfgOrigins = builder.Configuration.GetSection("Security:AllowedOrigins").Get<string[]>();
var allowed = (cfgOrigins is { Length: > 0 })
    ? cfgOrigins
    : new[] {
       "http://localhost:4200",
      "https://crecer-indexfront-crhsbre8hhdxc8dx.eastus-01.azurewebsites.net",
      "https://intraqa.crecerseguros.pe",
      "https://crecer-frontdoor-b5e2-ehgxa4fjahb7hcaw.a03.azurefd.net",
      "http://intranet-new.crecerseguros.pe",
      "https://crecer-frontdoor-prod-edhkfrbpdwfqd7hf.a03.azurefd.net",
      "https://crecer-indexfront-prod.azurewebsites.net",
      "https://index.crecerseguros.pe"
      };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(allowed)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== Seguridad JWT =====
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"]
    ?? throw new InvalidOperationException("Falta Jwt:Key en appsettings.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// cache + store revocados
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITokenRevocationStore, MemoryTokenRevocationStore>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"]
        };

        // Deny-list: rechaza tokens revocados por logout
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var jti = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti)) { ctx.Fail("Token sin JTI."); return; }

                var store = ctx.HttpContext.RequestServices.GetRequiredService<ITokenRevocationStore>();
                if (await store.IsRevokedAsync(jti))
                    ctx.Fail("Token revocado.");
            }
        };
    });

builder.Services.AddAuthorization();

// Health checks (opcional si luego quieres /healthz más avanzado)
// builder.Services.AddHealthChecks();

var app = builder.Build();

// ===== HSTS sólo en prod y bajo HTTPS =====
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Strict-Transport-Security
}

// ===== Encabezados 'forwarded' (estás detrás de Front Door/App GW) =====
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

// Swagger (si quieres sólo en dev, descomenta el if)
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== CORS =====
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

// ===== Seguridad: headers adicionales =====
app.Use(async (ctx, next) =>
{
    ctx.Response.OnStarting(() =>
    {
        var h = ctx.Response.Headers;
        h["X-Content-Type-Options"] = "nosniff";
        // Si embebes en otra app del mismo dominio, usa SAMEORIGIN; si no, DENY
        h["X-Frame-Options"] = "SAMEORIGIN";
        h["Referrer-Policy"] = "no-referrer";
        h["X-XSS-Protection"] = "1; mode=block";
        h["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        // HSTS manual extra (además de UseHsts). Sólo si es HTTPS y no es dev.
        if (ctx.Request.IsHttps && !app.Environment.IsDevelopment())
        {
            h["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
        }
        // Quita banner del servidor cuando sea posible
        h.Remove("Server");
        return Task.CompletedTask;
    });

    await next();
});
// ===== Validación de X-Azure-FDID (bloqueo de acceso directo) =====
app.Use(async (context, next) =>
{
    // Deja pasar PRELIGHTS siempre
    if (HttpMethods.IsOptions(context.Request.Method))
    {
        await next();
        return;
    }

    var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
    if (path.StartsWith("/healthz") || path.StartsWith("/swagger"))
    {
        await next();
        return;
    }

    var require = builder.Configuration.GetValue<bool>("Security:AzureFrontDoor:RequireFDID");
    if (!require)
    {
        await next();
        return;
    }

    var expected = builder.Configuration.GetSection("Security:AzureFrontDoor:ExpectedFDIDs").Get<string[]>() ?? Array.Empty<string>();
    var got = context.Request.Headers["X-Azure-FDID"].ToString();

    if (string.IsNullOrWhiteSpace(got) ||
        (expected.Length > 0 && !expected.Contains(got, StringComparer.OrdinalIgnoreCase)))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Forbidden (Front Door required)");
        return;
    }

    await next();
});


// Auth
app.UseAuthentication();
app.UseAuthorization();

// ===== /healthz =====
app.MapGet("/healthz", () => Results.Ok("OK"));

// Resto de endpoints
app.MapControllers();

app.Run();

using CrecerIndex.Abstraction.DependencyInjection;
using CrecerIndex.Abstraction.Interfaces.IRepository;
using CrecerIndex.Abstraction.Interfaces.IServices;
using CrecerIndex.Repository;
using CrecerIndex.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);

// Inyección automática por convención
RepositorySearchService.RegistrarRepository(builder.Services);
RepositorySearchService.RegistrarServices(builder.Services);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://crecer-indexfront-crhsbre8hhdxc8dx.eastus-01.azurewebsites.net", "https://intraqa.crecerseguros.pe") // sin barra al final
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
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));

// cache + store revocados
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITokenRevocationStore, MemoryTokenRevocationStore>();

// JWT Authentication
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        var config = builder.Configuration;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = false,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = config["Jwt:Issuer"],
//            ValidAudience = config["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
//        };
//    });


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
            ValidateLifetime = true,                 // <— ANTES estaba en false
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
//builder.Services.AddScoped<ICoverageRepository, CoverageRepository>();
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Swagger solo en desarrollo
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

// Seguridad: agrega headers **sin** romper responses ya iniciadas
app.Use(async (ctx, next) =>
{
    ctx.Response.OnStarting(() =>
    {
        var h = ctx.Response.Headers;
        h["X-Content-Type-Options"] = "nosniff";
        h["X-Frame-Options"] = "DENY";
        h["Referrer-Policy"] = "no-referrer";
        h["X-XSS-Protection"] = "1; mode=block";
        // Ajusta según tu política:
        h["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        return Task.CompletedTask;
    });

    await next();
});





// ?? Esto es fundamental
app.UseAuthentication();  // <-- Faltaba esta línea
app.UseAuthorization();

app.MapControllers();

app.Run();

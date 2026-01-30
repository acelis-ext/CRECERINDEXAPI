
//using CrecerIndex.Abstraction.Dtos;
//using CrecerIndex.Abstraction.Interfaces.IServices;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.IdentityModel.Tokens.Jwt;

//namespace CrecerIndexApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthService _authService;
//        private readonly ITokenRevocationStore _revokedStore;
//        private readonly IRecaptchaService _recaptcha;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly ILogger<AuthController> _logger;

//        public AuthController(
//            IAuthService authService,
//            ITokenRevocationStore revokedStore,
//            IRecaptchaService recaptcha,
//            IHttpContextAccessor httpContextAccessor,
//            ILogger<AuthController> logger)
//        {
//            _authService = authService;
//            _revokedStore = revokedStore;
//            _recaptcha = recaptcha;
//            _httpContextAccessor = httpContextAccessor;
//            _logger = logger;
//        }


//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
//        {
//            if (request is null) return BadRequest("Request nulo");

//            // 1) Verificar reCAPTCHA
//            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
//            var captcha = await _recaptcha.VerifyAsync(request.scaptchatoken, remoteIp);
//            if (!captcha.success)
//                return Unauthorized("Captcha inválido"); // ← tu interceptor ya trata 401

//            // 2) Credenciales
//            var token = _authService.Login(request.Usuario, request.Password);
//            if (token == null)
//                return Unauthorized("Credenciales inválidas");

//            // 3) OK
//            return Ok(new { token });
//        }

//        [HttpPost("logout")]
//        [Authorize]
//        public async Task<IActionResult> Logout()
//        {
//            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
//            var expUnix = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

//            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expUnix))
//                return BadRequest("Token sin JTI/EXP.");

//            var expUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expUnix)).UtcDateTime;
//            await _revokedStore.RevokeAsync(jti, expUtc);

//            return Ok(new { message = "Sesión cerrada." });
//        }
//    }
//}
using CrecerIndex.Abstraction.Dtos;
using CrecerIndex.Abstraction.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CrecerIndexApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenRevocationStore _revokedStore;
        private readonly IRecaptchaService _recaptcha;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthController> _logger;

        // Token de bypass para pruebas
        private const string BYPASS_CAPTCHA_TOKEN = "pruebacrecer123";

        public AuthController(
            IAuthService authService,
            ITokenRevocationStore revokedStore,
            IRecaptchaService recaptcha,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _revokedStore = revokedStore;
            _recaptcha = recaptcha;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("=== INICIO LOGIN ===");
            _logger.LogInformation("Usuario: {Usuario}", request?.Usuario ?? "NULL");

            try
            {
                // Validar request
                if (request is null)
                {
                    _logger.LogWarning("Request es NULL");
                    return BadRequest("Request nulo");
                }

                if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Usuario o Password vacío");
                    return BadRequest("Usuario y contraseña son requeridos");
                }

                // 1) Verificar reCAPTCHA (con bypass para pruebas)
                var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                _logger.LogInformation("RemoteIP: {RemoteIP}", remoteIp ?? "NULL");
                _logger.LogInformation("CaptchaToken recibido: {Token}", request.scaptchatoken ?? "NULL");

                bool captchaValid = false;

                // Bypass para pruebas
                if (request.scaptchatoken == BYPASS_CAPTCHA_TOKEN)
                {
                    _logger.LogInformation("✅ BYPASS CAPTCHA ACTIVADO (token de prueba)");
                    captchaValid = true;
                }
                else
                {
                    // Validar con Google reCAPTCHA
                    try
                    {
                        _logger.LogInformation("Validando captcha con Google...");
                        var captcha = await _recaptcha.VerifyAsync(request.scaptchatoken, remoteIp);
                        captchaValid = captcha.success;

                        if (!captchaValid)
                        {
                            _logger.LogWarning("❌ Captcha inválido. Success: {Success}, Hostname: {Host}, Errors: {Errors}",
                                captcha.success,
                                captcha.hostname ?? "NULL",
                                captcha.error_codes != null ? string.Join(",", captcha.error_codes) : "ninguno");
                        }
                        else
                        {
                            _logger.LogInformation("✅ Captcha válido");
                        }
                    }
                    catch (Exception exCaptcha)
                    {
                        _logger.LogError(exCaptcha, "❌ ERROR al validar captcha con Google: {Message}", exCaptcha.Message);
                        return StatusCode(500, $"Error validando captcha: {exCaptcha.Message}");
                    }
                }

                if (!captchaValid)
                {
                    return Unauthorized("Captcha inválido");
                }

                // 2) Validar credenciales
                _logger.LogInformation("Validando credenciales en BD...");
                string token = null;

                try
                {
                    token = _authService.Login(request.Usuario, request.Password);
                }
                catch (Exception exAuth)
                {
                    _logger.LogError(exAuth, "❌ ERROR en AuthService.Login (posible error de BD): {Message}", exAuth.Message);

                    // Mostrar más detalle del error
                    var innerMsg = exAuth.InnerException?.Message ?? "Sin inner exception";
                    _logger.LogError("Inner Exception: {Inner}", innerMsg);

                    return StatusCode(500, $"Error de autenticación: {exAuth.Message}. Inner: {innerMsg}");
                }

                if (token == null)
                {
                    _logger.LogWarning("❌ Credenciales inválidas para usuario: {Usuario}", request.Usuario);
                    return Unauthorized("Credenciales inválidas");
                }

                // 3) Éxito
                _logger.LogInformation("✅ LOGIN EXITOSO para usuario: {Usuario}", request.Usuario);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR GENERAL en Login: {Message}", ex.Message);
                _logger.LogError("StackTrace: {Stack}", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Inner}", ex.InnerException.Message);
                }

                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var expUnix = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

                if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expUnix))
                {
                    _logger.LogWarning("Token sin JTI o EXP");
                    return BadRequest("Token sin JTI/EXP.");
                }

                var expUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expUnix)).UtcDateTime;
                await _revokedStore.RevokeAsync(jti, expUtc);

                _logger.LogInformation("✅ Logout exitoso, token revocado: {Jti}", jti);
                return Ok(new { message = "Sesión cerrada." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en Logout: {Message}", ex.Message);
                return StatusCode(500, $"Error en logout: {ex.Message}");
            }
        }

        // Endpoint de prueba para verificar conexión a BD
        [HttpGet("test-db")]
        [AllowAnonymous]
        public IActionResult TestDb()
        {
            _logger.LogInformation("=== TEST DB ===");
            try
            {
                // Intenta hacer login con credenciales falsas solo para probar conexión
                var result = _authService.Login("__test__", "__test__");
                _logger.LogInformation("✅ Conexión a BD OK (resultado null es esperado)");
                return Ok(new { status = "OK", message = "Conexión a BD exitosa" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error conectando a BD: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    status = "ERROR",
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
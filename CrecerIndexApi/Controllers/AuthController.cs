
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
            if (request == null)
                return Ok(new { isLoginOk = false });

            // 1) Validar reCAPTCHA (estilo “otro back”: en error devolvemos 200 con isLoginOk=false)
            var remoteIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var captcha = await _recaptcha.VerifyAsync(request.scaptchatoken, remoteIp);
            if (!captcha.success)
            {
                _logger.LogWarning("Captcha inválido. Host:{Host} Errores:{Errors}",
                    captcha.hostname, captcha.error_codes is null ? "-" : string.Join(",", captcha.error_codes));

                return Ok(new { isLoginOk = false });   // ← contrato igual al otro back
            }

            // 2) Autenticar
            var token = _authService.Login(request.Usuario, request.Password);
            if (token == null)
                return Ok(new { isLoginOk = false });   // ← contrato igual al otro back

            // 3) Éxito
            return Ok(new { isLoginOk = true, token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var expUnix = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expUnix))
                return BadRequest("Token sin JTI/EXP.");

            var expUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expUnix)).UtcDateTime;
            await _revokedStore.RevokeAsync(jti, expUtc);

            return Ok(new { message = "Sesión cerrada." });
        }
    }
}

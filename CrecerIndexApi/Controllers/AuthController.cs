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

        public AuthController(IAuthService authService, ITokenRevocationStore revokedStore)
        {
            _authService = authService;
            _revokedStore = revokedStore;

        }

        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginRequestDto request)
        //{
        //    if (request == null) return BadRequest("Request nulo");

        //    var token = _authService.Login(request.Usuario, request.Password);
        //    if (token == null)
        //        return Unauthorized("Credenciales inválidas");

        //    return Ok(new { token });
        //}

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request == null) return BadRequest("Request nulo");

            var token = _authService.Login(request.Usuario, request.Password);
            if (token == null) return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }

        // NUEVO: cierre de sesión
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Extrae el JWT del header Authorization
            var bearer = Request.Headers.Authorization.ToString();
            var raw = bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? bearer.Substring("Bearer ".Length).Trim()
                : null;

            if (string.IsNullOrWhiteSpace(raw)) return NoContent();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(raw)) return NoContent();

            var jwt = handler.ReadJwtToken(raw);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expUnix = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (!string.IsNullOrEmpty(jti) && long.TryParse(expUnix, out var exp))
            {
                var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                await _revokedStore.RevokeAsync(jti, expiresAtUtc);   // <— queda inválido en el server
            }

            return NoContent();
        }

    }
}

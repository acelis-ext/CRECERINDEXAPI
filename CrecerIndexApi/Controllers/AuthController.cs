using CrecerIndex.Abstraction.Dtos;
using CrecerIndex.Abstraction.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CrecerIndexApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            if (request == null) return BadRequest("Request nulo");

            var token = _authService.Login(request.Usuario, request.Password);
            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new { token });
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using PrestaFacil.Application.Common;
using PrestaFacil.Application.Interfaces;
using PrestaFacil.Domain.Entities;

namespace PrestaFacil.API.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(ApiResponse<object>.Success(new { token }));
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Email = request.Email,
                    Rol = request.Rol ?? "Admin"
                };
                var resultado = await _authService.RegistrarAsync(usuario, request.Password);
                return Ok(ApiResponse<object>.Success(new { resultado.UsuarioId, resultado.Email }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegistroRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Rol { get; set; }
    }
}
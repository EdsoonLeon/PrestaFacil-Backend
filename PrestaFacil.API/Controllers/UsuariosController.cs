using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.API.DTOs;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public UsuariosController(PrestaFacilContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new {
                    u.UsuarioId,
                    u.Nombre,
                    u.Email,
                    u.Rol,
                    u.Activo,
                    u.FechaRegistro
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return Ok(new
            {
                usuario.UsuarioId,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol,
                usuario.Activo,
                usuario.FechaRegistro
            });
        }

        // PUT: api/usuarios/perfil/{id}
        [HttpPut("perfil/{id}")]
        public async Task<IActionResult> ActualizarPerfil(int id, PerfilDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // Verificar si el email ya está en uso por otro usuario
            var emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email.Trim().ToLower() && u.UsuarioId != id);
            if (emailExiste)
                return BadRequest(new { message = "El email ya está en uso por otro usuario" });

            // Actualizar nombre y email
            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;

            // Cambiar contraseña solo si se proporcionó
            if (!string.IsNullOrWhiteSpace(dto.PasswordNueva))
            {
                if (string.IsNullOrWhiteSpace(dto.PasswordActual))
                    return BadRequest(new { message = "Debes ingresar tu contraseña actual" });

                if (!BCrypt.Net.BCrypt.Verify(dto.PasswordActual, usuario.Password))
                    return BadRequest(new { message = "La contraseña actual es incorrecta" });

                if (dto.PasswordNueva.Length < 8)
                    return BadRequest(new { message = "La nueva contraseña debe tener al menos 8 caracteres" });

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Perfil actualizado exitosamente",
                    usuario = new
                    {
                        usuario.UsuarioId,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Rol
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar perfil", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
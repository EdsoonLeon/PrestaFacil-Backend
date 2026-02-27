using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;
using System.Security.Claims;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public ClientesController(PrestaFacilContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var usuarioId = GetUsuarioId();
            return await _context.Clientes
                .Where(c => c.Activo == true && c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var usuarioId = GetUsuarioId();
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == id && c.UsuarioId == usuarioId);
            if (cliente == null) return NotFound();
            return cliente;
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            try
            {
                var usuarioId = GetUsuarioId();
                cliente.UsuarioId = usuarioId;

                // Verificar si existe con ese DNI para este usuario
                var existente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.DNI == cliente.DNI && c.UsuarioId == usuarioId);

                if (existente != null)
                {
                    if (existente.Activo == false)
                    {
                        existente.Nombre = cliente.Nombre;
                        existente.Apellido = cliente.Apellido;
                        existente.Telefono = cliente.Telefono;
                        existente.Email = cliente.Email;
                        existente.Direccion = cliente.Direccion;
                        existente.Activo = true;
                        await _context.SaveChangesAsync();
                        return Ok(existente);
                    }
                    else
                    {
                        return Conflict(new { message = "Ya existe un cliente activo con ese DNI." });
                    }
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor.",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message
                });
            }
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            var usuarioId = GetUsuarioId();
            var existente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == id && c.UsuarioId == usuarioId);
            if (existente == null) return NotFound();

            existente.Nombre = cliente.Nombre;
            existente.Apellido = cliente.Apellido;
            existente.DNI = cliente.DNI;
            existente.Telefono = cliente.Telefono;
            existente.Email = cliente.Email;
            existente.Direccion = cliente.Direccion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var usuarioId = GetUsuarioId();
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == id && c.UsuarioId == usuarioId);
            if (cliente == null) return NotFound();
            cliente.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
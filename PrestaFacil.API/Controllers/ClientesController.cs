using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;

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

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .Where(c => c.Activo == true)
                .ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return cliente;
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            try
            {
                // Primero verificar si existe con ese DNI (activo o inactivo)
                var existente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.DNI == cliente.DNI);

                if (existente != null)
                {
                    if (existente.Activo == false)
                    {
                        // Reactivar y actualizar datos
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
                        // Ya existe activo con ese DNI
                        return Conflict(new { message = "Ya existe un cliente activo con ese DNI." });
                    }
                }

                // DNI nuevo, crear normalmente
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.ClienteId) return BadRequest();
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
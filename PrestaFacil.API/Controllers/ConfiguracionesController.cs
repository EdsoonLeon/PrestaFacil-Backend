using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionesController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public ConfiguracionesController(PrestaFacilContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Configuracion>>> GetConfiguraciones()
        {
            return await _context.Configuraciones.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Configuracion>> GetConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);
            if (configuracion == null) return NotFound();
            return configuracion;
        }

        [HttpPost]
        public async Task<ActionResult<Configuracion>> PostConfiguracion(Configuracion configuracion)
        {
            _context.Configuraciones.Add(configuracion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetConfiguracion), new { id = configuracion.ConfiguracionId }, configuracion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracion(int id, Configuracion configuracion)
        {
            if (id != configuracion.ConfiguracionId) return BadRequest();
            _context.Entry(configuracion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones.FindAsync(id);
            if (configuracion == null) return NotFound();
            _context.Configuraciones.Remove(configuracion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
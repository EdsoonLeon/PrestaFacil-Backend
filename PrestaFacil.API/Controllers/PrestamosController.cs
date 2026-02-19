using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.API.DTOs;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public PrestamosController(PrestaFacilContext context)
        {
            _context = context;
        }

        // GET: api/Prestamos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPrestamos()
        {
            return await _context.Prestamos.ToListAsync();
        }

        // GET: api/Prestamos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Cliente)
                .Include(p => p.Cuotas)
                .FirstOrDefaultAsync(p => p.PrestamoId == id);
            if (prestamo == null) return NotFound();
            return prestamo;
        }

        [HttpPost]
public async Task<ActionResult<Prestamo>> PostPrestamo(PrestamoDto dto)
{
    decimal tasaMensual = dto.TasaInteres / 100;
    decimal cuotaMensual = dto.Monto * tasaMensual / (1 - (decimal)Math.Pow((double)(1 + tasaMensual), -dto.NumeroCuotas));
    decimal saldoPendiente = Math.Round(cuotaMensual * dto.NumeroCuotas, 2);

    var prestamo = new Prestamo
    {
        ClienteId = dto.ClienteId,
        Monto = dto.Monto,
        TasaInteres = dto.TasaInteres,
        NumeroCuotas = dto.NumeroCuotas,
        CuotaMensual = Math.Round(cuotaMensual, 2),
        SaldoPendiente = saldoPendiente,
        FechaInicio = dto.FechaInicio,
        FechaFin = dto.FechaFin,
        Estado = dto.Estado
    };
    _context.Prestamos.Add(prestamo);
    await _context.SaveChangesAsync();

    // Generar cuotas automáticamente
    for (int i = 1; i <= dto.NumeroCuotas; i++)
    {
        var cuota = new Cuota
        {
            PrestamoId = prestamo.PrestamoId,
            NumeroCuota = i,
            MontoCuota = Math.Round(cuotaMensual, 2),
            FechaVencimiento = dto.FechaInicio.AddMonths(i),
            Estado = "Pendiente",
            MontoPagado = 0
        };
        _context.Cuotas.Add(cuota);
    }
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.PrestamoId }, prestamo);
}

        // PUT: api/Prestamos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrestamo(int id, PrestamoUpdateDto dto)
        {
            if (id != dto.PrestamoId) return BadRequest();
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();

            decimal tasaMensual = dto.TasaInteres / 100;
            decimal cuotaMensual = dto.Monto * tasaMensual / (1 - (decimal)Math.Pow((double)(1 + tasaMensual), -dto.NumeroCuotas));

            prestamo.ClienteId = dto.ClienteId;
            prestamo.Monto = dto.Monto;
            prestamo.TasaInteres = dto.TasaInteres;
            prestamo.NumeroCuotas = dto.NumeroCuotas;
            prestamo.CuotaMensual = Math.Round(cuotaMensual, 2);
            prestamo.SaldoPendiente = Math.Round(cuotaMensual * dto.NumeroCuotas, 2);
            prestamo.FechaInicio = dto.FechaInicio;
            prestamo.FechaFin = dto.FechaFin;
            prestamo.Estado = dto.Estado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Prestamos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrestamo(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();
            prestamo.Estado = "Cancelado";
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
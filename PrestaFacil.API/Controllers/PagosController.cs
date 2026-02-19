using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.API.DTOs;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly PrestaFacilContext _context;
        public PagosController(PrestaFacilContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            return await _context.Pagos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> GetPago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();
            return pago;
        }

        [HttpGet("prestamo/{prestamoId}")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosPorPrestamo(int prestamoId)
        {
            return await _context.Pagos
                .Where(p => p.PrestamoId == prestamoId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Pago>> PostPago(PagoDto dto)
        {
            var pago = new Pago
            {
                PrestamoId = dto.PrestamoId,
                CuotaId = dto.CuotaId,
                MontoPagado = dto.MontoPagado,
                MetodoPago = dto.MetodoPago,
                FechaPago = dto.FechaPago,
                UsuarioId = dto.UsuarioId,
                Observaciones = dto.Observaciones
            };
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPago), new { id = pago.PagoId }, pago);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPago(int id, PagoDto dto)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();

            pago.PrestamoId = dto.PrestamoId;
            pago.CuotaId = dto.CuotaId;
            pago.MontoPagado = dto.MontoPagado;
            pago.MetodoPago = dto.MetodoPago;
            pago.FechaPago = dto.FechaPago;
            pago.UsuarioId = dto.UsuarioId;
            pago.Observaciones = dto.Observaciones;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
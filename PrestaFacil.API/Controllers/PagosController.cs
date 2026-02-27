using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.API.DTOs;
using PrestaFacil.Domain.Entities;
using PrestaFacil.Infrastructure.Data;
using System.Security.Claims;

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

        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // GET: api/pagos
        [HttpGet]
        public async Task<IActionResult> GetPagos()
        {
            var usuarioId = GetUsuarioId();
            var pagos = await _context.Pagos
                .Include(p => p.Prestamo)
                    .ThenInclude(pr => pr.Cliente)
                .Include(p => p.Cuota)
                .Where(p => p.Prestamo.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new
                {
                    p.PagoId,
                    p.PrestamoId,
                    p.CuotaId,
                    p.MontoPagado,
                    p.MetodoPago,
                    p.FechaPago,
                    p.Observaciones,
                    NumeroCuota = p.Cuota != null ? p.Cuota.NumeroCuota : 0,
                    MontoCuota = p.Cuota != null ? p.Cuota.MontoCuota : 0,
                    EstadoCuota = p.Cuota != null ? p.Cuota.Estado : "-",
                    ClienteNombre = p.Prestamo.Cliente.Nombre + " " + p.Prestamo.Cliente.Apellido,
                    ClienteDni = p.Prestamo.Cliente.DNI
                })
                .ToListAsync();

            return Ok(pagos);
        }

        // GET: api/pagos/prestamo/{prestamoId}/cuotas
        [HttpGet("prestamo/{prestamoId}/cuotas")]
        public async Task<IActionResult> GetCuotasPorPrestamo(int prestamoId)
        {
            var usuarioId = GetUsuarioId();

            // Verificar que el préstamo pertenece al usuario
            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId && p.UsuarioId == usuarioId);
            if (prestamo == null) return NotFound(new { message = "Préstamo no encontrado" });

            var cuotas = await _context.Cuotas
                .Where(c => c.PrestamoId == prestamoId)
                .OrderBy(c => c.NumeroCuota)
                .Select(c => new
                {
                    c.CuotaId,
                    c.NumeroCuota,
                    c.FechaVencimiento,
                    c.MontoCuota,
                    c.Estado,
                    c.MontoPagado,
                    SaldoCuota = c.MontoCuota - c.MontoPagado,
                    c.FechaPago
                })
                .ToListAsync();

            return Ok(cuotas);
        }

        // POST: api/pagos
        [HttpPost]
        public async Task<IActionResult> PostPago(PagoDto dto)
        {
            var usuarioId = GetUsuarioId();

            var cuota = await _context.Cuotas.FindAsync(dto.CuotaId);
            if (cuota == null)
                return NotFound(new { message = "Cuota no encontrada" });

            if (cuota.Estado == "Pagado")
                return BadRequest(new { message = "Esta cuota ya fue pagada completamente" });

            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == dto.PrestamoId && p.UsuarioId == usuarioId);
            if (prestamo == null)
                return NotFound(new { message = "Préstamo no encontrado" });

            decimal saldoCuota = cuota.MontoCuota - cuota.MontoPagado;
            if (dto.MontoPagado > saldoCuota)
                return BadRequest(new { message = $"El monto no puede superar el saldo de la cuota: S/ {saldoCuota:F2}" });

            if (dto.MontoPagado <= 0)
                return BadRequest(new { message = "El monto debe ser mayor a cero" });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pago = new Pago
                {
                    PrestamoId = dto.PrestamoId,
                    CuotaId = dto.CuotaId,
                    MontoPagado = dto.MontoPagado,
                    MetodoPago = dto.MetodoPago,
                    FechaPago = DateTime.Now,
                    UsuarioId = dto.UsuarioId,
                    Observaciones = dto.Observaciones
                };
                _context.Pagos.Add(pago);

                cuota.MontoPagado += dto.MontoPagado;
                if (cuota.MontoPagado >= cuota.MontoCuota)
                {
                    cuota.Estado = "Pagado";
                    cuota.FechaPago = DateTime.Now;
                }

                prestamo.SaldoPendiente -= dto.MontoPagado;
                prestamo.TotalPagado += dto.MontoPagado;

                bool todasPagadas = await _context.Cuotas
                    .Where(c => c.PrestamoId == dto.PrestamoId)
                    .AllAsync(c => c.CuotaId == dto.CuotaId
                        ? cuota.Estado == "Pagado"
                        : c.Estado == "Pagado");

                if (todasPagadas)
                    prestamo.Estado = "Cancelado";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Pago registrado exitosamente",
                    pagoId = pago.PagoId,
                    saldoRestanteCuota = cuota.MontoCuota - cuota.MontoPagado,
                    estadoCuota = cuota.Estado,
                    saldoPrestamo = prestamo.SaldoPendiente
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    message = "Error al registrar el pago",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message
                });
            }
        }

        // GET: api/pagos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPago(int id)
        {
            var usuarioId = GetUsuarioId();
            var pago = await _context.Pagos
                .Include(p => p.Prestamo)
                    .ThenInclude(pr => pr.Cliente)
                .Include(p => p.Cuota)
                .Where(p => p.PagoId == id && p.Prestamo.UsuarioId == usuarioId)
                .Select(p => new
                {
                    p.PagoId,
                    p.PrestamoId,
                    p.CuotaId,
                    p.MontoPagado,
                    p.MetodoPago,
                    p.FechaPago,
                    p.Observaciones,
                    NumeroCuota = p.Cuota.NumeroCuota,
                    MontoCuota = p.Cuota.MontoCuota,
                    ClienteNombre = p.Prestamo.Cliente.Nombre + " " + p.Prestamo.Cliente.Apellido
                })
                .FirstOrDefaultAsync();

            if (pago == null) return NotFound();
            return Ok(pago);
        }
    }
}
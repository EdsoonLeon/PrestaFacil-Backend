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
    public class PrestamosController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public PrestamosController(PrestaFacilContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // GET: api/Prestamos
        [HttpGet]
        public async Task<IActionResult> GetPrestamos()
        {
            var usuarioId = GetUsuarioId();
            var prestamos = await _context.Prestamos
                .Include(p => p.Cliente)
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => new
                {
                    p.PrestamoId,
                    p.ClienteId,
                    ClienteNombre = p.Cliente.Nombre + " " + p.Cliente.Apellido,
                    p.Monto,
                    p.TasaInteres,
                    p.NumeroCuotas,
                    p.CuotaMensual,
                    p.SaldoPendiente,
                    p.TotalPagado,
                    p.FechaInicio,
                    p.FechaFin,
                    p.Estado,
                    p.FechaRegistro
                })
                .ToListAsync();

            return Ok(prestamos);
        }

        // GET: api/Prestamos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id)
        {
            var usuarioId = GetUsuarioId();
            var prestamo = await _context.Prestamos
                .Include(p => p.Cliente)
                .Include(p => p.Cuotas)
                .FirstOrDefaultAsync(p => p.PrestamoId == id && p.UsuarioId == usuarioId);
            if (prestamo == null) return NotFound();
            return prestamo;
        }

        // GET: api/Prestamos/{id}/detalle
        [HttpGet("{id}/detalle")]
        public async Task<IActionResult> GetPrestamoDetalle(int id)
        {
            var usuarioId = GetUsuarioId();
            var prestamo = await _context.Prestamos
                .Include(p => p.Cliente)
                .Include(p => p.Cuotas)
                .Include(p => p.Pagos)
                .FirstOrDefaultAsync(p => p.PrestamoId == id && p.UsuarioId == usuarioId);

            if (prestamo == null)
                return NotFound(new { message = "Préstamo no encontrado" });

            var detalle = new
            {
                prestamo.PrestamoId,
                prestamo.Monto,
                prestamo.TasaInteres,
                prestamo.NumeroCuotas,
                prestamo.CuotaMensual,
                prestamo.SaldoPendiente,
                prestamo.TotalPagado,
                prestamo.FechaInicio,
                prestamo.FechaFin,
                prestamo.Estado,
                prestamo.FechaRegistro,
                Cliente = new
                {
                    prestamo.Cliente.ClienteId,
                    prestamo.Cliente.Nombre,
                    prestamo.Cliente.Apellido,
                    prestamo.Cliente.DNI,
                    prestamo.Cliente.Telefono,
                    prestamo.Cliente.Email,
                    prestamo.Cliente.Direccion
                },
                Cuotas = prestamo.Cuotas
                    .OrderBy(c => c.NumeroCuota)
                    .Select(c => new
                    {
                        c.CuotaId,
                        c.NumeroCuota,
                        c.MontoCuota,
                        c.MontoPagado,
                        SaldoCuota = c.MontoCuota - c.MontoPagado,
                        c.FechaVencimiento,
                        c.FechaPago,
                        c.Estado
                    }),
                Pagos = prestamo.Pagos
                    .OrderBy(p => p.FechaPago)
                    .Select(p => new
                    {
                        p.PagoId,
                        p.MontoPagado,
                        p.MetodoPago,
                        p.FechaPago,
                        p.Observaciones
                    })
            };

            return Ok(detalle);
        }

        [HttpPost]
        public async Task<ActionResult<Prestamo>> PostPrestamo(PrestamoDto dto)
        {
            var usuarioId = GetUsuarioId();
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
                Estado = dto.Estado,
                UsuarioId = usuarioId
            };
            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

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
            var usuarioId = GetUsuarioId();
            if (id != dto.PrestamoId) return BadRequest();
            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == id && p.UsuarioId == usuarioId);
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
            var usuarioId = GetUsuarioId();
            var prestamo = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == id && p.UsuarioId == usuarioId);
            if (prestamo == null) return NotFound();
            prestamo.Estado = "Cancelado";
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
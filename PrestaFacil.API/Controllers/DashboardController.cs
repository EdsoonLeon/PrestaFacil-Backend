using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestaFacil.Infrastructure.Data;
using System.Security.Claims;

namespace PrestaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly PrestaFacilContext _context;

        public DashboardController(PrestaFacilContext context)
        {
            _context = context;
        }

        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var usuarioId = GetUsuarioId();

            var totalClientes = await _context.Clientes.CountAsync(c => c.Activo && c.UsuarioId == usuarioId);
            var totalPrestamos = await _context.Prestamos.CountAsync(p => p.UsuarioId == usuarioId);
            var prestamosActivos = await _context.Prestamos.CountAsync(p => p.Estado == "Activo" && p.UsuarioId == usuarioId);
            var prestamosCancelados = await _context.Prestamos.CountAsync(p => p.Estado == "Cancelado" && p.UsuarioId == usuarioId);
            var prestamosVencidos = await _context.Prestamos.CountAsync(p => p.Estado == "Vencido" && p.UsuarioId == usuarioId);
            var montoTotal = await _context.Prestamos.Where(p => p.UsuarioId == usuarioId).SumAsync(p => (decimal?)p.Monto) ?? 0;
            var totalRecaudado = await _context.Pagos.Where(p => p.Prestamo.UsuarioId == usuarioId).SumAsync(p => (decimal?)p.MontoPagado) ?? 0;
            var cuotasVencidas = await _context.Cuotas.CountAsync(c => c.Estado == "Pendiente" && c.FechaVencimiento < DateTime.Today && c.Prestamo.UsuarioId == usuarioId);
            var pagosHoy = await _context.Pagos.CountAsync(p => p.FechaPago.Date == DateTime.Today && p.Prestamo.UsuarioId == usuarioId);

            return Ok(new
            {
                totalClientes,
                totalPrestamos,
                prestamosActivos,
                prestamosCancelados,
                prestamosVencidos,
                montoTotal,
                totalRecaudado,
                cuotasVencidas,
                pagosHoy
            });
        }

        // GET: api/dashboard/prestamos-por-mes
        [HttpGet("prestamos-por-mes")]
        public async Task<IActionResult> GetPrestamosPorMes()
        {
            var usuarioId = GetUsuarioId();
            var hoy = DateTime.Today;
            var hace6Meses = hoy.AddMonths(-5);

            var datos = await _context.Prestamos
                .Where(p => p.UsuarioId == usuarioId && p.FechaRegistro >= new DateTime(hace6Meses.Year, hace6Meses.Month, 1))
                .GroupBy(p => new { p.FechaRegistro.Year, p.FechaRegistro.Month })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    cantidad = g.Count(),
                    monto = g.Sum(p => p.Monto)
                })
                .OrderBy(g => g.anio).ThenBy(g => g.mes)
                .ToListAsync();

            var meses = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var fecha = hoy.AddMonths(-i);
                var dato = datos.FirstOrDefault(d => d.anio == fecha.Year && d.mes == fecha.Month);
                meses.Add(new
                {
                    label = fecha.ToString("MMM", new System.Globalization.CultureInfo("es-PE")),
                    cantidad = dato?.cantidad ?? 0,
                    monto = dato?.monto ?? 0
                });
            }

            return Ok(meses);
        }

        // GET: api/dashboard/ultimos-prestamos
        [HttpGet("ultimos-prestamos")]
        public async Task<IActionResult> GetUltimosPrestamos()
        {
            var usuarioId = GetUsuarioId();
            var prestamos = await _context.Prestamos
                .Include(p => p.Cliente)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaRegistro)
                .Take(5)
                .Select(p => new
                {
                    p.PrestamoId,
                    clienteNombre = p.Cliente.Nombre + " " + p.Cliente.Apellido,
                    p.Monto,
                    p.Estado,
                    p.FechaRegistro,
                    p.SaldoPendiente
                })
                .ToListAsync();

            return Ok(prestamos);
        }

        // GET: api/dashboard/cuotas-vencidas
        [HttpGet("cuotas-vencidas")]
        public async Task<IActionResult> GetCuotasVencidas()
        {
            var usuarioId = GetUsuarioId();
            var cuotas = await _context.Cuotas
                .Include(c => c.Prestamo)
                    .ThenInclude(p => p.Cliente)
                .Where(c => c.Estado == "Pendiente" && c.FechaVencimiento < DateTime.Today && c.Prestamo.UsuarioId == usuarioId)
                .OrderBy(c => c.FechaVencimiento)
                .Take(5)
                .Select(c => new
                {
                    c.CuotaId,
                    c.NumeroCuota,
                    c.FechaVencimiento,
                    c.MontoCuota,
                    saldoPendiente = c.MontoCuota - c.MontoPagado,
                    diasVencidos = (DateTime.Today - c.FechaVencimiento).Days,
                    clienteNombre = c.Prestamo.Cliente.Nombre + " " + c.Prestamo.Cliente.Apellido,
                    prestamoId = c.Prestamo.PrestamoId
                })
                .ToListAsync();

            return Ok(cuotas);
        }
    }
}
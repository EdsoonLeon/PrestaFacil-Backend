using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrestaFacil.Domain.Entities;

namespace PrestaFacil.Domain.Entities
{
    public class Prestamo
    {
        public int PrestamoId { get; set; }
        public int ClienteId { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaInteres { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Activo";
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relaciones
        public Cliente Cliente { get; set; } = null!;
        public ICollection<Cuota> Cuotas { get; set; } = new List<Cuota>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

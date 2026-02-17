using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestaFacil.Domain.Entities
{
    public class Cuota
    {
        public int CuotaId { get; set; }
        public int PrestamoId { get; set; }
        public int NumeroCuota { get; set; }
        public decimal MontoCapital { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = "Pendiente";

        // Relación
        public Prestamo Prestamo { get; set; } = null!;
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

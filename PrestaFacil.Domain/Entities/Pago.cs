using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestaFacil.Domain.Entities
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int PrestamoId { get; set; }
        public int CuotaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public string MetodoPago { get; set; } = "Efectivo";
        public string Observacion { get; set; } = string.Empty;

        // Relaciones
        public Prestamo Prestamo { get; set; } = null!;
        public Cuota Cuota { get; set; } = null!;
    }
}

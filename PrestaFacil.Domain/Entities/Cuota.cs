using System;
using System.Collections.Generic;
namespace PrestaFacil.Domain.Entities
{
    public class Cuota
    {
        public int CuotaId { get; set; }
        public int PrestamoId { get; set; }

        private int _numeroCuota;
        public int NumeroCuota
        {
            get => _numeroCuota;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El número de cuota debe ser mayor a cero.");
                _numeroCuota = value;
            }
        }

        private DateTime _fechaVencimiento;
        public DateTime FechaVencimiento
        {
            get => _fechaVencimiento;
            set
            {
                if (value == default)
                    throw new ArgumentException("La fecha de vencimiento no es válida.");
                _fechaVencimiento = value;
            }
        }

        private decimal _montoCuota;
        public decimal MontoCuota
        {
            get => _montoCuota;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El monto de la cuota debe ser mayor a cero.");
                _montoCuota = value;
            }
        }

        private string _estado = "Pendiente";
        public string Estado
        {
            get => _estado;
            set
            {
                var estadosValidos = new[] { "Pendiente", "Pagado", "Vencido", "Parcial" };
                if (!Array.Exists(estadosValidos, e => e == value))
                    throw new ArgumentException($"Estado inválido. Los estados permitidos son: {string.Join(", ", estadosValidos)}");
                _estado = value;
            }
        }

        public decimal MontoPagado { get; set; } = 0;
        public DateTime? FechaPago { get; set; }

        public Prestamo Prestamo { get; set; } = null!;
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
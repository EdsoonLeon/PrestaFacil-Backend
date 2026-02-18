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

        private decimal _montoCapital;
        public decimal MontoCapital
        {
            get => _montoCapital;
            set
            {
                if (value < 0)
                    throw new ArgumentException("El monto capital no puede ser negativo.");
                _montoCapital = value;
            }
        }

        private decimal _montoInteres;
        public decimal MontoInteres
        {
            get => _montoInteres;
            set
            {
                if (value < 0)
                    throw new ArgumentException("El monto interés no puede ser negativo.");
                _montoInteres = value;
            }
        }

        private decimal _montoTotal;
        public decimal MontoTotal
        {
            get => _montoTotal;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El monto total debe ser mayor a cero.");
                _montoTotal = value;
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

        public Prestamo Prestamo { get; set; } = null!;
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
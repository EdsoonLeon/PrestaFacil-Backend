using System;
namespace PrestaFacil.Domain.Entities
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int PrestamoId { get; set; }
        public int CuotaId { get; set; }

        private decimal _montoPagado;
        public decimal MontoPagado
        {
            get => _montoPagado;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El monto del pago debe ser mayor a cero.");
                _montoPagado = value;
            }
        }

        private DateTime _fechaPago = DateTime.Now;
        public DateTime FechaPago
        {
            get => _fechaPago;
            set
            {
                if (value == default)
                    throw new ArgumentException("La fecha de pago no es válida.");
                if (value > DateTime.Now)
                    throw new ArgumentException("La fecha de pago no puede ser futura.");
                _fechaPago = value;
            }
        }

        private string _metodoPago = "Efectivo";
        public string MetodoPago
        {
            get => _metodoPago;
            set
            {
                var metodosValidos = new[] { "Efectivo", "Transferencia", "Yape", "Plin", "Deposito" };
                if (!Array.Exists(metodosValidos, e => e == value))
                    throw new ArgumentException($"Método de pago inválido. Los métodos permitidos son: {string.Join(", ", metodosValidos)}");
                _metodoPago = value;
            }
        }

        public int UsuarioId { get; set; }

        private string _observaciones = string.Empty;
        public string Observaciones
        {
            get => _observaciones;
            set
            {
                if (value != null && value.Length > 500)
                    throw new ArgumentException("Las observaciones no pueden superar los 500 caracteres.");
                _observaciones = value?.Trim() ?? string.Empty;
            }
        }

        public Prestamo Prestamo { get; set; } = null!;
        public Cuota Cuota { get; set; } = null!;
    }
}
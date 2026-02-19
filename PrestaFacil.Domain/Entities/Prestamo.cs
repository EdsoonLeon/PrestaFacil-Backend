using System;
using System.Collections.Generic;
namespace PrestaFacil.Domain.Entities
{
    public class Prestamo
    {
        public int PrestamoId { get; set; }
        public int ClienteId { get; set; }
        private decimal _monto;
        public decimal Monto
        {
            get => _monto;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El monto debe ser mayor a cero.");
                if (value > 100000)
                    throw new ArgumentException("El monto no puede superar 100,000.");
                _monto = value;
            }
        }
        private decimal _tasaInteres;
        public decimal TasaInteres
        {
            get => _tasaInteres;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("La tasa de interés debe ser mayor a cero.");
                if (value > 100)
                    throw new ArgumentException("La tasa de interés no puede superar el 100%.");
                _tasaInteres = value;
            }
        }
        private int _numeroCuotas;
        public int NumeroCuotas
        {
            get => _numeroCuotas;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El número de cuotas debe ser mayor a cero.");
                if (value > 60)
                    throw new ArgumentException("El número de cuotas no puede superar 60.");
                _numeroCuotas = value;
            }
        }
        
        public decimal CuotaMensual { get; set; }
        public decimal SaldoPendiente { get; set; }

        private DateTime _fechaInicio;
        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set
            {
                if (value == default)
                    throw new ArgumentException("La fecha de inicio no es válida.");
                _fechaInicio = value;
            }
        }
        private DateTime _fechaFin;
        public DateTime FechaFin
        {
            get => _fechaFin;
            set
            {
                if (value == default)
                    throw new ArgumentException("La fecha de fin no es válida.");
                if (value <= _fechaInicio)
                    throw new ArgumentException("La fecha de fin debe ser mayor a la fecha de inicio.");
                _fechaFin = value;
            }
        }
        private string _estado = "Activo";
        public string Estado
        {
            get => _estado;
            set
            {
                var estadosValidos = new[] { "Activo", "Pagado", "Vencido", "Cancelado" };
                if (!Array.Exists(estadosValidos, e => e == value))
                    throw new ArgumentException($"Estado inválido. Los estados permitidos son: {string.Join(", ", estadosValidos)}");
                _estado = value;
            }
        }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public Cliente Cliente { get; set; } = null!;
        public ICollection<Cuota> Cuotas { get; set; } = new List<Cuota>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
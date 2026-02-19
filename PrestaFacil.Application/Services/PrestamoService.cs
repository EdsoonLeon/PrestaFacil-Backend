using System;
using System.Collections.Generic;
using PrestaFacil.Application.Interfaces;
using PrestaFacil.Domain.Entities;
namespace PrestaFacil.Application.Services
{
    public class PrestamoService : IPrestamoService
    {
        public List<Cuota> CalcularCuotas(Prestamo prestamo)
        {
            var cuotas = new List<Cuota>();
            decimal tasaMensual = prestamo.TasaInteres / 100;
            decimal factor = (decimal)Math.Pow((double)(1 + tasaMensual), prestamo.NumeroCuotas);
            decimal cuotaMensual = prestamo.Monto * (tasaMensual * factor) / (factor - 1);
            cuotaMensual = Math.Round(cuotaMensual, 2);

            for (int i = 1; i <= prestamo.NumeroCuotas; i++)
            {
                var cuota = new Cuota
                {
                    PrestamoId = prestamo.PrestamoId,
                    NumeroCuota = i,
                    MontoCuota = cuotaMensual,
                    FechaVencimiento = prestamo.FechaInicio.AddMonths(i),
                    Estado = "Pendiente",
                    MontoPagado = 0
                };
                cuotas.Add(cuota);
            }
            return cuotas;
        }
    }
}
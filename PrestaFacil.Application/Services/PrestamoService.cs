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

            // Tasa mensual
            decimal tasaMensual = prestamo.TasaInteres / 100;

            // Fórmula de amortización francesa
            decimal factor = (decimal)Math.Pow((double)(1 + tasaMensual), prestamo.NumeroCuotas);
            decimal cuotaMensual = prestamo.Monto * (tasaMensual * factor) / (factor - 1);
            cuotaMensual = Math.Round(cuotaMensual, 2);

            decimal saldoRestante = prestamo.Monto;

            for (int i = 1; i <= prestamo.NumeroCuotas; i++)
            {
                decimal interesCuota = Math.Round(saldoRestante * tasaMensual, 2);
                decimal capitalCuota = Math.Round(cuotaMensual - interesCuota, 2);

                // Ajuste en la última cuota para evitar diferencias por redondeo
                if (i == prestamo.NumeroCuotas)
                    capitalCuota = saldoRestante;

                saldoRestante -= capitalCuota;

                var cuota = new Cuota
                {
                    PrestamoId = prestamo.PrestamoId,
                    NumeroCuota = i,
                    MontoCapital = capitalCuota,
                    MontoInteres = interesCuota,
                    MontoTotal = Math.Round(capitalCuota + interesCuota, 2),
                    FechaVencimiento = prestamo.FechaInicio.AddMonths(i),
                    Estado = "Pendiente"
                };

                cuotas.Add(cuota);
            }

            return cuotas;
        }
    }
}
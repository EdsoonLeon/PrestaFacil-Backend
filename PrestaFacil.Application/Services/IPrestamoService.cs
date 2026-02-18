using System.Collections.Generic;
using PrestaFacil.Domain.Entities;

namespace PrestaFacil.Application.Interfaces
{
    public interface IPrestamoService
    {
        List<Cuota> CalcularCuotas(Prestamo prestamo);
    }
}
using PrestaFacil.Domain.Entities;

public class Pago
{
    public int PagoId { get; set; }
    public int PrestamoId { get; set; }
    public int? CuotaId { get; set; }
    public decimal MontoPagado { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.Now;
    public string MetodoPago { get; set; } = "Efectivo";
    public int UsuarioId { get; set; }
    public string? Observaciones { get; set; }
    public Prestamo Prestamo { get; set; } = null!;
    public Cuota? Cuota { get; set; }
}
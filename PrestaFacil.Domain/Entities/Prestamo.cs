using PrestaFacil.Domain.Entities;

public class Prestamo
{
    public int PrestamoId { get; set; }
    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public decimal TasaInteres { get; set; }
    public int NumeroCuotas { get; set; }
    public decimal CuotaMensual { get; set; }
    public decimal SaldoPendiente { get; set; }
    public decimal TotalPagado { get; set; } = 0;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; } = "Activo";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public Cliente Cliente { get; set; } = null!;
    public Usuario? Usuario { get; set; }
    public ICollection<Cuota> Cuotas { get; set; } = new List<Cuota>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
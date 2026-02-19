namespace PrestaFacil.API.DTOs
{
    public class PagoDto
    {
        public int PrestamoId { get; set; }
        public int CuotaId { get; set; }
        public decimal MontoPagado { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public DateTime FechaPago { get; set; }
        public int UsuarioId { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}
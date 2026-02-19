namespace PrestaFacil.API.DTOs
{
    public class PrestamoDto
    {
        public int ClienteId { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaInteres { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Activo";
    }

    public class PrestamoUpdateDto
    {
        public int PrestamoId { get; set; }
        public int ClienteId { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaInteres { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Activo";
    }
}
namespace PrestaFacil.API.DTOs
{
    public class PerfilDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordActual { get; set; }
        public string? PasswordNueva { get; set; }
    }
}
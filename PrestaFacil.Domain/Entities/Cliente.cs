using System;
using System.Collections.Generic;

namespace PrestaFacil.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
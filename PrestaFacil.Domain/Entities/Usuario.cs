using System;
using System.Text.RegularExpressions;

namespace PrestaFacil.Domain.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre no puede estar vacío.");
                if (value.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
                _nombre = value.Trim();
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El email no puede estar vacío.");
                if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new ArgumentException("El email no tiene un formato válido.");
                _email = value.Trim().ToLower();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La contraseña no puede estar vacía.");
                if (value.Length < 8)
                    throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
                if (!Regex.IsMatch(value, @"[A-Z]"))
                    throw new ArgumentException("La contraseña debe tener al menos una letra mayúscula.");
                if (!Regex.IsMatch(value, @"[0-9]"))
                    throw new ArgumentException("La contraseña debe tener al menos un número.");
                _password = value;
            }
        }

        private string _rol = "Usuario";
        public string Rol
        {
            get => _rol;
            set
            {
                var rolesValidos = new[] { "Admin", "Usuario", "Cajero" };
                if (!Array.Exists(rolesValidos, r => r == value))
                    throw new ArgumentException($"Rol inválido. Los roles permitidos son: {string.Join(", ", rolesValidos)}");
                _rol = value;
            }
        }

        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
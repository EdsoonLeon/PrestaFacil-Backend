using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PrestaFacil.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; set; }

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

        private string _apellido = string.Empty;
        public string Apellido
        {
            get => _apellido;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El apellido no puede estar vacío.");
                if (value.Length > 100)
                    throw new ArgumentException("El apellido no puede superar los 100 caracteres.");
                _apellido = value.Trim();
            }
        }

        private string _dni = string.Empty;
        public string DNI
        {
            get => _dni;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El DNI no puede estar vacío.");
                if (!Regex.IsMatch(value, @"^\d{8}$"))
                    throw new ArgumentException("El DNI debe tener exactamente 8 dígitos numéricos.");
                _dni = value.Trim();
            }
        }

        private string _telefono = string.Empty;
        public string Telefono
        {
            get => _telefono;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El teléfono no puede estar vacío.");
                if (!Regex.IsMatch(value, @"^\d{9}$"))
                    throw new ArgumentException("El teléfono debe tener exactamente 9 dígitos.");
                _telefono = value.Trim();
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

        private string _direccion = string.Empty;
        public string Direccion
        {
            get => _direccion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La dirección no puede estar vacía.");
                if (value.Length > 250)
                    throw new ArgumentException("La dirección no puede superar los 250 caracteres.");
                _direccion = value.Trim();
            }
        }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
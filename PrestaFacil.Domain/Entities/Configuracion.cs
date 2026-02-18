using System;

namespace PrestaFacil.Domain.Entities
{
    public class Configuracion
    {
        public int ConfiguracionId { get; set; }

        private string _clave = string.Empty;
        public string Clave
        {
            get => _clave;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La clave no puede estar vacía.");
                if (value.Length > 50)
                    throw new ArgumentException("La clave no puede superar los 50 caracteres.");
                _clave = value.Trim().ToUpper();
            }
        }

        private string _valor = string.Empty;
        public string Valor
        {
            get => _valor;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El valor no puede estar vacío.");
                if (value.Length > 200)
                    throw new ArgumentException("El valor no puede superar los 200 caracteres.");
                _valor = value.Trim();
            }
        }

        private string _descripcion = string.Empty;
        public string Descripcion
        {
            get => _descripcion;
            set
            {
                if (value != null && value.Length > 500)
                    throw new ArgumentException("La descripción no puede superar los 500 caracteres.");
                _descripcion = value?.Trim() ?? string.Empty;
            }
        }
    }
}
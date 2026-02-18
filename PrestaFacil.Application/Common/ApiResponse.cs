namespace PrestaFacil.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errores { get; set; } = new List<string>();

        public static ApiResponse<T> Success(T data, string mensaje = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Exito = true,
                Mensaje = mensaje,
                Data = data
            };
        }

        public static ApiResponse<T> Failure(string mensaje, List<string>? errores = null)
        {
            return new ApiResponse<T>
            {
                Exito = false,
                Mensaje = mensaje,
                Errores = errores ?? new List<string>()
            };
        }
    }
}
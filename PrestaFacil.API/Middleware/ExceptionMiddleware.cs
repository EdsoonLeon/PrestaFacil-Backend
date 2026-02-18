using System.Net;
using System.Text.Json;
using PrestaFacil.Application.Common;

namespace PrestaFacil.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Recurso no encontrado");
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string mensaje)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Failure(mensaje);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
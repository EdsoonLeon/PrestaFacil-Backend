using PrestaFacil.Domain.Entities;

namespace PrestaFacil.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);
        Task<Usuario> RegistrarAsync(Usuario usuario, string password);
    }
}
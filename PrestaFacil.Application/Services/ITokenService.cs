using PrestaFacil.Domain.Entities;

namespace PrestaFacil.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
    }
}
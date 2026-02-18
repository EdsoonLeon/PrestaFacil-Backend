using Microsoft.EntityFrameworkCore;
using PrestaFacil.Application.Interfaces;
using PrestaFacil.Domain.Entities;


namespace PrestaFacil.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly DbContext _context;

        public AuthService(ITokenService tokenService, DbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var usuario = await _context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                throw new ArgumentException("Contraseña incorrecta.");

            return _tokenService.GenerarToken(usuario);
        }

        public async Task<Usuario> RegistrarAsync(Usuario usuario, string password)
        {
            var existe = await _context.Set<Usuario>()
                .AnyAsync(u => u.Email == usuario.Email);

            if (existe)
                throw new ArgumentException("Ya existe un usuario con ese email.");

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Set<Usuario>().Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }
    }
}
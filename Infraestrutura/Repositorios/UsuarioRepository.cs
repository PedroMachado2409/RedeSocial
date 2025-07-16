using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class UsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> CadastrarUsuario(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<List<Usuario>> ListarTodosOsUsuarios()
        {
            return await _context.Usuarios.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<Usuario?> ObterUsuarioPorId(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObterUsuarioPorEmail(string email)
        {
            return await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<Usuario>> ObterUsuarioPorNome(string nome)
        {
            return await _context.Usuarios.Where(u => u.Nome.ToLower().Contains(nome)).ToListAsync();
        }
    }
}

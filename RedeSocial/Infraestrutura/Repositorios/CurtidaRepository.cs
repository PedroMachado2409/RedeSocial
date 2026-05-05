using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class CurtidaRepository : ICurtidaRepository
    {
        private readonly AppDbContext _context;

        public CurtidaRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<Curtida> CadastrarCurtida(Curtida curtida)
        {
             await _context.Curtidas.AddAsync(curtida);
            await _context.SaveChangesAsync();
            return curtida;

        }

        public async Task<Curtida?> ObterCurtidaPorPostEUsuario(int postId, int usuarioId)
        {
            return await _context.Curtidas
                .Include(c => c.Post)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.PostId == postId && c.UsuarioId == usuarioId);
        }

        public async Task RemoverCurtida(Curtida curtida)
        {
            _context.Curtidas.Remove(curtida);
              await _context.SaveChangesAsync(); 
        }
    }
}

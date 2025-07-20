using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class ComentarioRepository
    {
        private readonly AppDbContext _context;

        public ComentarioRepository(AppDbContext context)
        {
            _context = context;
        }   

        public async Task<Comentario> GerarComentario(Comentario comentario)
        {
            await _context.Comentarios.AddAsync(comentario);
            await _context.SaveChangesAsync();
            return comentario;
        }

        public async Task<Comentario?> ObterComentarioPorId(int id)
        {
            return await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task RemoverComentario(Comentario comentario)
        {
             _context.Remove(comentario);
        }
    }
}

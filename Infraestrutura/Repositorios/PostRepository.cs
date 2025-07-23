using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class PostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> ListarPosts()
        {
            return await _context.Posts.
                Include(p => p.Comentarios)
                 .ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)    
                .OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<Post> CadastrarPost(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post?> ObterPostPorId(int id)
        {
           var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            return post;
        }

        public async Task<List<Post>> ListarPostsDosAmigos(List<int> amigosIds)
        {
            return await _context.Posts
                .Include(p => p.Comentarios)
                    .ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)
                .Where(p => amigosIds.Contains(p.UsuarioId))
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

      

    }
}

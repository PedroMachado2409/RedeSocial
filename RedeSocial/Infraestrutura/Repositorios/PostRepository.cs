using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context) => _context = context;

        public async Task<List<Post>> ListarPosts(int usuarioId)
            => await _context.Posts
                .Include(p => p.Comentarios).ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<Post> CadastrarPost(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post?> ObterPostPorId(int id)
            => await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Post>> ListarPostsDosAmigos(List<int> amigosIds)
            => await _context.Posts
                .Include(p => p.Comentarios).ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)
                .Where(p => amigosIds.Contains(p.UsuarioId))
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<List<Post>> ListarPostsDoUsuario(int usuarioId)
            => await _context.Posts
                .Include(p => p.Comentarios).ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<List<Post>> ListarTodosOsPosts(int usuarioId, List<int> amigosIds)
        {
            // Mescla posts do próprio usuário + posts dos amigos, sem duplicatas
            var ids = amigosIds.Union(new[] { usuarioId }).ToList();
            return await _context.Posts
                .Include(p => p.Comentarios).ThenInclude(c => c.Usuario)
                .Include(p => p.Usuario)
                .Where(p => ids.Contains(p.UsuarioId))
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}

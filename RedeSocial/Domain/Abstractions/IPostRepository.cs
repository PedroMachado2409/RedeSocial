using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IPostRepository
    {
        Task<List<Post>> ListarPosts(int usuarioId);
        Task<Post>       CadastrarPost(Post post);
        Task<Post?>      ObterPostPorId(int id);
        Task<List<Post>> ListarPostsDosAmigos(List<int> amigosIds);
        Task<List<Post>> ListarPostsDoUsuario(int usuarioId);
        Task<List<Post>> ListarTodosOsPosts(int usuarioId, List<int> amigosIds);
    }
}

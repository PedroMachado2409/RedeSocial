using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IPostRepository
    {
        public  Task<List<Post>> ListarPosts();
        public  Task<Post> CadastrarPost(Post post);
        public  Task<Post?> ObterPostPorId(int id);
        public  Task<List<Post>> ListarPostsDosAmigos(List<int> amigosIds);
    }
}

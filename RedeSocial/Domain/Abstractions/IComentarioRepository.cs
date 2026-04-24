using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IComentarioRepository
    {
        public  Task<Comentario> GerarComentario(Comentario comentario);
        public  Task<Comentario?> ObterComentarioPorId(int id);
        public  Task RemoverComentario(Comentario comentario);

    }
}

using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface ICurtidaRepository
    {
        public Task<Curtida> CadastrarCurtida(Curtida curtida);
        public Task<Curtida> ObterCurtidaPorPostEUsuario(int postId, int usuarioId);
        public Task RemoverCurtida(Curtida curtida);
    }
}

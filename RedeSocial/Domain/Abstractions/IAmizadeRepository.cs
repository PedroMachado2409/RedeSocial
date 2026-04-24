using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IAmizadeRepository
    {
        public  Task<List<Amizade>> ListarAmigosDoUsuario(int usuarioId);
        public  Task<bool> JaSaoAmigos(int usuario1Id, int usuario2Id);
        public  Task<Amizade> FazerAmizade(Amizade amizade, AmizadePendente pendente);
        public  Task<Amizade?> ObterAmizadePorId(int id);
        public  Task RemoverAmizade(Amizade amizade);
        public  Task<List<int>> ObterIdsDosAmigos(int usuarioId);

    }
}

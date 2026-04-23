using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IAmizadePendenteRepository
    {
        public Task<AmizadePendente> EnviarSolicitacao(AmizadePendente amizadePendente);
        public Task<List<AmizadePendente>> ListarPedidosRecebidos(int destinatarioId);
        public Task<AmizadePendente?> ObterPedidoDeAmizadePorId(int id);
        public Task<List<AmizadePendente>> ListarPedidosEnviados(int solicitanteId);
        public Task<bool> ValidarSeJaTemPedido(int usuario1, int usuario2);
        public Task RejeitarPedido(AmizadePendente pendente);

    }
}

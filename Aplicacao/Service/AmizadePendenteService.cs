using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Repositorios;

namespace RedeSocial.Aplicacao.Service
{
    public class AmizadePendenteService
    {
        private readonly AmizadePendenteRepository _repository;
        private readonly AmizadeRepository _amizadeRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AmizadePendenteService(AmizadePendenteRepository repository,
            AuthService authService, UsuarioRepository usuarioRepository, AmizadeRepository amizadeRepository, IMapper mapper)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _amizadeRepository = amizadeRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<List<AmizadePendenteDTO>> ListarPedidosRecebidos()
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var pedidos = await _repository.ListarPedidosRecebidos(usuario.Id);
            return _mapper.Map<List<AmizadePendenteDTO>>(pedidos);
        }

        public async Task<List<AmizadePendenteDTO>> ListarPedidosEnviados()
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var pedidos = await _repository.ListarPedidosEnviados(usuario.Id);
            return _mapper.Map<List<AmizadePendenteDTO>>(pedidos);
        }

        public async Task<AmizadePendenteDTO> EnviarPedidoDeAmizade(AmizadePendenteDTO dto)
        {
            var usuarioLogado = await _authService.ObterUsuarioAutenticado();

            var destinatario = await _usuarioRepository.ObterUsuarioPorId(dto.DestinatarioId);
            if (destinatario == null)
                throw new Exception(Messages.UsuarioNotFound);

            var pedidoPendente = await _repository.ValidarSeJaTemPedido(usuarioLogado.Id, dto.DestinatarioId);
            if (pedidoPendente)
                throw new Exception(Messages.AmizadeDuplicada);

            var amizadeExiste = await _amizadeRepository.JaSaoAmigos(usuarioLogado.Id, dto.DestinatarioId);
            if (amizadeExiste)
                throw new Exception(Messages.AmizadeExistente);

            var novaSolicitacao = new AmizadePendente().Criar(usuarioLogado.Id, dto.DestinatarioId);
            var entidade = await _repository.EnviarSolicitacao(novaSolicitacao);

            return _mapper.Map<AmizadePendenteDTO>(entidade);
        }

        public async Task RejeitarPedido(int id)
        {
            var pedido = await _repository.ObterPedidoDeAmizadePorId(id);
            await _repository.RejeitarPedido(pedido);
        }
    }
}

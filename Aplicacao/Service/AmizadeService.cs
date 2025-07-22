using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Repositorios;
namespace RedeSocial.Aplicacao.Service
{
    public class AmizadeService
    {
        private readonly AmizadeRepository _repository;
        private readonly AmizadePendenteRepository _pendenteRepository;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AmizadeService(AmizadeRepository repository, IMapper mapper, AmizadePendenteRepository pendenteRepository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
            _mapper = mapper;
            _pendenteRepository = pendenteRepository;
        }

        public async Task<AmizadeDTO> AceitarAmizade(AmizadeDTO dto)
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var pedido = await _pendenteRepository.ObterPedidoDeAmizadePorId(dto.PedidoId);

            if (pedido == null)
                throw new Exception(Messages.PedidoNotFound);

            if (pedido.DestinatarioId != usuario.Id)
                throw new Exception(Messages.ApenasDestinatario);


            var novaAmizade = new Amizade().Criar(pedido.SolicitanteId, pedido.DestinatarioId, pedido.Id);

            var amizade = await _repository.FazerAmizade(novaAmizade, pedido);

            return _mapper.Map<AmizadeDTO>(amizade);
        }

        public async Task<Amizade?> ObterAmizadePorId(int id)
        {
            return await _repository.ObterAmizadePorId(id);
        }


        public async Task<List<AmizadeDTO>> ListarAmizadesDoUsuario()
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var amizades = await _repository.ListarAmigosDoUsuario(usuario.Id);
            return _mapper.Map<List<AmizadeDTO>>(amizades);
        }

        public async Task RemoverAmizade(int id)
        {
            var amizade = await _repository.ObterAmizadePorId(id);
            await _repository.RemoverAmizade(amizade);
        }
    }
}

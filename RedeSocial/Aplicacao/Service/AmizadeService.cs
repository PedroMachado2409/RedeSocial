using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Repositorios;
namespace RedeSocial.Aplicacao.Service
{
    public class AmizadeService
    {
        private readonly IAmizadeRepository _repository;
        private readonly IAmizadePendenteRepository _pendenteRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AmizadeService(IAmizadeRepository repository, IMapper mapper, IAmizadePendenteRepository pendenteRepository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
            _mapper = mapper;
            _pendenteRepository = pendenteRepository;
        }

        public async Task<AmizadeDTO> AceitarAmizade(AceitarAmizadeRequestDTO dto)
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var pedido = await _pendenteRepository.ObterPedidoDeAmizadePorId(dto.PedidoId);

            if (pedido == null)
                throw new BadRequestException(Messages.PedidoNotFound);

            if (pedido.DestinatarioId != usuario.Id)
                throw new BadRequestException(Messages.ApenasDestinatario);


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


            return _mapper.Map<List<AmizadeDTO>>(amizades, opt =>
            {
                opt.Items["usuarioId"] = usuario.Id;
            });
        }

        public async Task RemoverAmizade(int id)
        {
            var amizade = await _repository.ObterAmizadePorId(id);
            await _repository.RemoverAmizade(amizade);
        }
    }
}

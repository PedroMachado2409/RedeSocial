using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;

namespace RedeSocial.Aplicacao.Service
{
    public class CurtidaService
    {
        private readonly ICurtidaRepository _curtidaRepository;
        private readonly IAuthService _authService;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public CurtidaService (ICurtidaRepository curtidaRepository, IAuthService authService, IPostRepository postRepository, IMapper mapper)
        {
            _curtidaRepository = curtidaRepository;
            _authService = authService;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<CurtidaDTO> CadastrarCurtida(int postId)
        {
            var post = await _postRepository.ObterPostPorId(postId);
            if (post == null)
                throw new BadRequestException("Post não encontrado");

            var usuario = await _authService.ObterUsuarioAutenticado();

            var curtidaExistente = await _curtidaRepository
                .ObterCurtidaPorPostEUsuario(post.Id, usuario.Id);

            if (curtidaExistente is not null)
                throw new BadRequestException("Você ja curtiu esse Post !");

            var novaCurtida = new Curtida(usuario.Id, post.Id);

            post.AdicionarCurtida();

            await _curtidaRepository.CadastrarCurtida(novaCurtida);

            return _mapper.Map<CurtidaDTO>(novaCurtida);
        }

        public async Task RemoverCurtida(int postId)
        {
            var post = await _postRepository.ObterPostPorId(postId);
            if (post == null)
                throw new BadRequestException("Post não encontrado");

            var usuario = await _authService.ObterUsuarioAutenticado();

            var curtidaExistente = await _curtidaRepository
                .ObterCurtidaPorPostEUsuario(post.Id, usuario.Id);

            if (curtidaExistente is null)
                throw new BadRequestException("Você não curtiu esse Post !");

            post.RemoverCurtida();

            await _curtidaRepository.RemoverCurtida(curtidaExistente);
        }



    }
}

using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Service
{
    public class PostService
    {
        private readonly IPostRepository       _repository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IAmizadeRepository    _amizadeRepository;
        private readonly IAuthService          _authService;
        private readonly IMapper               _mapper;

        public PostService(
            IPostRepository       postRepository,
            IComentarioRepository comentarioRepository,
            IAuthService          authService,
            IMapper               mapper,
            IAmizadeRepository    amizadeRepository)
        {
            _repository           = postRepository;
            _comentarioRepository = comentarioRepository;
            _amizadeRepository    = amizadeRepository;
            _authService          = authService;
            _mapper               = mapper;
        }

        public async Task<PostRequestDTO> CadastrarPost(PostRequestDTO dto)
        {
            var usuario     = await _authService.ObterUsuarioAutenticado();
            var comentarios = _mapper.Map<List<Comentario>>(dto.Comentarios ?? new());

            byte[]? imagemBytes = null;
            if (!string.IsNullOrWhiteSpace(dto.ImagemBase64))
                imagemBytes = Convert.FromBase64String(dto.ImagemBase64);

            var post = Post.Criar(dto.Titulo, comentarios, usuario!.Id, 0, imagemBytes);
            await _repository.CadastrarPost(post);
            return _mapper.Map<PostRequestDTO>(post);
        }

        public async Task<List<PostResponseDTO>> ListarPosts()
        {
            var usuario = await _authService.ObterUsuarioAutenticado();
            var posts   = await _repository.ListarPosts(usuario!.Id);
            return _mapper.Map<List<PostResponseDTO>>(posts);
        }

        public async Task<ComentarioRequestDTO> AdicionarComentarioAoPost(ComentarioRequestDTO dto)
        {
            var usuario    = await _authService.ObterUsuarioAutenticado();
            var post       = await _repository.ObterPostPorId(dto.PostId);
            var comentario = Comentario.Criar(usuario!.Id, dto.Descricao, post!.Id);
            await _comentarioRepository.GerarComentario(comentario);
            return _mapper.Map<ComentarioRequestDTO>(comentario);
        }

        public async Task<List<PostResponseDTO>> ListarPostApenasDosAmigos()
        {
            var usuario      = await _authService.ObterUsuarioAutenticado();
            var idsDosAmigos = await _amizadeRepository.ObterIdsDosAmigos(usuario!.Id);
            var posts        = await _repository.ListarPostsDosAmigos(idsDosAmigos);
            return _mapper.Map<List<PostResponseDTO>>(posts);
        }

        public async Task<List<PostResponseDTO>> ListarPostsDoUsuario(int usuarioId)
        {
            var posts = await _repository.ListarPostsDoUsuario(usuarioId);
            return _mapper.Map<List<PostResponseDTO>>(posts);
        }

        public async Task<List<PostResponseDTO>> ListarTodosOsPosts()
        {
            var usuario      = await _authService.ObterUsuarioAutenticado();
            var idsDosAmigos = await _amizadeRepository.ObterIdsDosAmigos(usuario!.Id);
            var posts        = await _repository.ListarTodosOsPosts(usuario.Id, idsDosAmigos);
            return _mapper.Map<List<PostResponseDTO>>(posts);
        }
    }
}

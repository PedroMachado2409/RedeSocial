using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Repositorios;

namespace RedeSocial.Aplicacao.Service
{
    public class PostService
    {
        private readonly PostRepository _repository;
        private readonly ComentarioRepository _comentarioRepository;
        private readonly AuthService _AuthService;
        private readonly IMapper _mapper;
        
        public PostService (PostRepository postRepository, ComentarioRepository comentarioRepository, AuthService authService, IMapper mapper)
        {
            _repository = postRepository;
            _comentarioRepository = comentarioRepository;   
            _AuthService = authService;
            _mapper = mapper;
        }

    

        public async Task<PostRequestDTO> CadastrarPost(PostRequestDTO dto)
        {
            var usuario = await _AuthService.ObterUsuarioAutenticado();
            var comentarios = _mapper.Map<List<Comentario>>(dto.Comentarios);
            var post = Post.Criar(dto.Titulo, comentarios, usuario.Id);

            await _repository.CadastrarPost(post);
            return _mapper.Map<PostRequestDTO>(post);

        }

        public async Task<List<PostResponseDTO>> ListarPosts()
        {
            var posts = await _repository.ListarPosts();
            return _mapper.Map<List<PostResponseDTO>>(posts);
        }

        public async Task<ComentarioRequestDTO> AdicionarComentarioAoPost(ComentarioRequestDTO dto)
        {
            var usuario = await _AuthService.ObterUsuarioAutenticado();
            var post = await _repository.ObterPostPorId(dto.PostId);
            var comentario = Comentario.Criar(usuario.Id, dto.Descricao, post.Id);

            await _comentarioRepository.GerarComentario(comentario);

            return _mapper.Map<ComentarioRequestDTO>(comentario);

        }

    }
}

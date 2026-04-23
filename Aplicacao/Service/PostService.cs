using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Repositorios;

namespace RedeSocial.Aplicacao.Service
{
    public class PostService
    {
        private readonly IPostRepository _repository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IAmizadeRepository _amizadeRepository;
        private readonly AuthService _AuthService;
        private readonly IMapper _mapper;
        
        public PostService (IPostRepository postRepository, ComentarioRepository comentarioRepository, AuthService authService, IMapper mapper, IAmizadeRepository amizadeRepository)
        {
            _repository = postRepository;
            _comentarioRepository = comentarioRepository;   
            _amizadeRepository = amizadeRepository;
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

        public async Task<List<PostResponseDTO>> ListarPostApenasDosAmigos()
        {
            var usuario = await _AuthService.ObterUsuarioAutenticado();

            var idsDosAmigos = await _amizadeRepository.ObterIdsDosAmigos(usuario.Id);

            var posts = await _repository.ListarPostsDosAmigos(idsDosAmigos);

            return _mapper.Map<List<PostResponseDTO>>(posts);
        }
    }
}

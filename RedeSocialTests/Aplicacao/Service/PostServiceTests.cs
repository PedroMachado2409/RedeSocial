using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Aplicacao.Dto;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _repositoryMock;
    private readonly Mock<IComentarioRepository> _comentarioRepositoryMock;
    private readonly Mock<IAmizadeRepository> _amizadeRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock; 
    private readonly Mock<IMapper> _mapperMock;

    private readonly PostService _service;

    public PostServiceTests()
    {
        _repositoryMock = new Mock<IPostRepository>();
        _comentarioRepositoryMock = new Mock<IComentarioRepository>();
        _amizadeRepositoryMock = new Mock<IAmizadeRepository>();
        _mapperMock = new Mock<IMapper>();

        _authServiceMock = new Mock<IAuthService>();

        _service = new PostService(
            _repositoryMock.Object,
            _comentarioRepositoryMock.Object,
            _authServiceMock.Object, 
            _mapperMock.Object,
            _amizadeRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CadastrarPost_DeveCriarPost()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var dto = new PostRequestDTO
        {
            Titulo = "Meu post",
            Comentarios = new List<ComentarioRequestDTO>()
        };

        var comentarios = new List<Comentario>();

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _mapperMock.Setup(m => m.Map<List<Comentario>>(dto.Comentarios))
            .Returns(comentarios);

        _mapperMock.Setup(m => m.Map<PostRequestDTO>(It.IsAny<Post>()))
            .Returns(dto);

        var result = await _service.CadastrarPost(dto);

        result.Should().NotBeNull();

        _repositoryMock.Verify(r => r.CadastrarPost(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task ListarPosts_DeveRetornarListaMapeada()
    {
        var posts = new List<Post>
        {
            Post.Criar("Titulo", new List<Comentario>(), 1)
        };

        var dto = new List<PostResponseDTO>
        {
            new PostResponseDTO()
        };

        _repositoryMock.Setup(r => r.ListarPosts())
            .ReturnsAsync(posts);

        _mapperMock.Setup(m => m.Map<List<PostResponseDTO>>(posts))
            .Returns(dto);

        var result = await _service.ListarPosts();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdicionarComentarioAoPost_DeveAdicionarComentario()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var post = Post.Criar("Titulo", new List<Comentario>(), usuario.Id);

        var dto = new ComentarioRequestDTO
        {
            PostId = post.Id,
            Descricao = "Comentário"
        };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _repositoryMock.Setup(r => r.ObterPostPorId(dto.PostId))
            .ReturnsAsync(post);

        _mapperMock.Setup(m => m.Map<ComentarioRequestDTO>(It.IsAny<Comentario>()))
            .Returns(dto);

        var result = await _service.AdicionarComentarioAoPost(dto);

        result.Should().NotBeNull();

        _comentarioRepositoryMock.Verify(c => c.GerarComentario(It.IsAny<Comentario>()), Times.Once);
    }

    [Fact]
    public async Task ListarPostApenasDosAmigos_DeveRetornarPostsDosAmigos()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var idsAmigos = new List<int> { 2, 3 };

        var posts = new List<Post>
        {
            Post.Criar("Titulo", new List<Comentario>(), 2)
        };

        var dto = new List<PostResponseDTO>
        {
            new PostResponseDTO()
        };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _amizadeRepositoryMock.Setup(a => a.ObterIdsDosAmigos(usuario.Id))
            .ReturnsAsync(idsAmigos);

        _repositoryMock.Setup(r => r.ListarPostsDosAmigos(idsAmigos))
            .ReturnsAsync(posts);

        _mapperMock.Setup(m => m.Map<List<PostResponseDTO>>(posts))
            .Returns(dto);

        var result = await _service.ListarPostApenasDosAmigos();

        result.Should().HaveCount(1);
    }
}
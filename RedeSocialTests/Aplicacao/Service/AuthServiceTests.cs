using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Seguranca;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository>    _repositoryMock;
    private readonly Mock<IPostRepository>       _postRepositoryMock;
    private readonly Mock<IAmizadeRepository>    _amizadeRepositoryMock;
    private readonly Mock<IMapper>               _mapperMock;
    private readonly Mock<IHttpContextAccessor>  _httpContextAccessorMock;
    private readonly IConfiguration              _configuration;
    private readonly IAuthService                _service;

    public AuthServiceTests()
    {
        _repositoryMock          = new Mock<IUsuarioRepository>();
        _postRepositoryMock      = new Mock<IPostRepository>();
        _amizadeRepositoryMock   = new Mock<IAmizadeRepository>();
        _mapperMock              = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key",      "super_secret_key_123456_super_secret_key_123456"},
            {"Jwt:Issuer",   "test_issuer"},
            {"Jwt:Audience", "test_audience"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _service = new AuthService(
            _configuration,
            _repositoryMock.Object,
            _postRepositoryMock.Object,
            _amizadeRepositoryMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object
        );
    }

    // ===================== REGISTRAR =====================

    [Fact]
    public async Task RegistrarUsuario_DeveCriarUsuario_QuandoEmailNaoExiste()
    {
        var usuario = new Usuario("Pedro", "teste@email.com", "123456");

        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync((Usuario?)null);

        _repositoryMock.Setup(r => r.CadastrarUsuario(It.IsAny<Usuario>()))
            .ReturnsAsync(usuario);

        var result = await _service.RegistrarUsuario(usuario);

        result.Should().NotBeNull();
        result.Senha.Should().NotBe("123456");
    }

    [Fact]
    public async Task RegistrarUsuario_DeveLancarExcecao_QuandoEmailJaExiste()
    {
        var usuario = new Usuario("Pedro", "teste@email.com", "123456");

        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync(new Usuario("Outro", "teste@email.com", "123456"));

        var act = async () => await _service.RegistrarUsuario(usuario);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    // ===================== LOGIN =====================

    [Fact]
    public async Task Autenticar_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var senha   = "123456";
        var usuario = new Usuario("Pedro", "teste@email.com", PasswordHelper.CriptografarSenha(senha));

        var dto = new LoginRequestDTO { Email = usuario.Email, Senha = senha };

        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(dto.Email))
            .ReturnsAsync(usuario);

        var result = await _service.Autenticar(dto);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(usuario.Email);
    }

    [Fact]
    public async Task Autenticar_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        var dto = new LoginRequestDTO { Email = "teste@email.com", Senha = "123456" };

        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(dto.Email))
            .ReturnsAsync((Usuario?)null);

        var act = async () => await _service.Autenticar(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Autenticar_DeveLancarExcecao_QuandoSenhaInvalida()
    {
        var usuario = new Usuario("Pedro", "teste@email.com", PasswordHelper.CriptografarSenha("senha_correta"));

        var dto = new LoginRequestDTO { Email = usuario.Email, Senha = "senha_errada" };

        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(dto.Email))
            .ReturnsAsync(usuario);

        var act = async () => await _service.Autenticar(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    // ===================== USUARIO AUTENTICADO =====================

    [Fact]
    public async Task ObterUsuarioAutenticado_DeveRetornarUsuario_QuandoClaimExiste()
    {
        var email  = "teste@email.com";
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, email) };
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var usuario = new Usuario("Pedro", email, "123456");
        _repositoryMock.Setup(r => r.ObterUsuarioPorEmail(email)).ReturnsAsync(usuario);

        var result = await _service.ObterUsuarioAutenticado();

        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    // ===================== BUSCA POR NOME =====================

    [Fact]
    public async Task ObterUsuarioPorNome_DeveRetornarListaMapeada()
    {
        var usuarios    = new List<Usuario> { new Usuario("Pedro", "pedro@email.com", "123456") };
        var usuariosDto = new List<UsuarioDTO> { new UsuarioDTO { Nome = "Pedro" } };

        _repositoryMock.Setup(r => r.ObterUsuarioPorNome("Pedro")).ReturnsAsync(usuarios);
        _mapperMock.Setup(m => m.Map<List<UsuarioDTO>>(usuarios)).Returns(usuariosDto);

        var result = await _service.ObterUsuarioPorNome("Pedro");

        result.Should().HaveCount(1);
        result[0].Nome.Should().Be("Pedro");
    }
}

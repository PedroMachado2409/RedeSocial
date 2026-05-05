using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Exceptions;

public class AmizadePendenteServiceTests
{
    private readonly Mock<IAmizadePendenteRepository> _repositoryMock;
    private readonly Mock<IAmizadeRepository>         _amizadeRepositoryMock;
    private readonly Mock<IUsuarioRepository>         _usuarioRepositoryMock;
    private readonly Mock<IAuthService>               _authServiceMock;
    private readonly Mock<IMapper>                    _mapperMock;
    private readonly AmizadePendenteService           _service;

    public AmizadePendenteServiceTests()
    {
        _repositoryMock        = new Mock<IAmizadePendenteRepository>();
        _amizadeRepositoryMock = new Mock<IAmizadeRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _mapperMock            = new Mock<IMapper>();
        _authServiceMock       = new Mock<IAuthService>();

        _service = new AmizadePendenteService(
            _repositoryMock.Object,
            _authServiceMock.Object,
            _usuarioRepositoryMock.Object,
            _amizadeRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task ListarPedidosRecebidos_DeveRetornarListaMapeada()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");
        var pedidos = new List<AmizadePendente> { new AmizadePendente().Criar(2, usuario.Id) };
        var dto     = new List<AmizadePendenteDTO> { new AmizadePendenteDTO() };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _repositoryMock.Setup(r => r.ListarPedidosRecebidos(usuario.Id)).ReturnsAsync(pedidos);
        _mapperMock.Setup(m => m.Map<List<AmizadePendenteDTO>>(pedidos)).Returns(dto);

        var result = await _service.ListarPedidosRecebidos();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task ListarPedidosEnviados_DeveRetornarListaMapeada()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");
        var pedidos = new List<AmizadePendente> { new AmizadePendente().Criar(usuario.Id, 2) };
        var dto     = new List<AmizadePendenteDTO> { new AmizadePendenteDTO() };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _repositoryMock.Setup(r => r.ListarPedidosEnviados(usuario.Id)).ReturnsAsync(pedidos);
        _mapperMock.Setup(m => m.Map<List<AmizadePendenteDTO>>(pedidos)).Returns(dto);

        var result = await _service.ListarPedidosEnviados();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task EnviarPedidoDeAmizade_DeveCriarPedido_QuandoValido()
    {
        var usuario     = new Usuario("Pedro", "email@email.com", "123");
        var destinatario = new Usuario("Joao", "joao@email.com", "123");

        // usa o DTO correto: EnviarAmizadeRequestDTO
        var dto     = new EnviarAmizadeRequestDTO { DestinatarioId = 2 };
        var entidade = new AmizadePendente().Criar(usuario.Id, dto.DestinatarioId);
        var resultDto = new AmizadePendenteDTO();

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(u => u.ObterUsuarioPorId(dto.DestinatarioId)).ReturnsAsync(destinatario);
        _repositoryMock.Setup(r => r.ValidarSeJaTemPedido(usuario.Id, dto.DestinatarioId)).ReturnsAsync(false);
        _amizadeRepositoryMock.Setup(a => a.JaSaoAmigos(usuario.Id, dto.DestinatarioId)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.EnviarSolicitacao(It.IsAny<AmizadePendente>())).ReturnsAsync(entidade);
        _mapperMock.Setup(m => m.Map<AmizadePendenteDTO>(entidade)).Returns(resultDto);

        var result = await _service.EnviarPedidoDeAmizade(dto);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task EnviarPedido_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");
        var dto     = new EnviarAmizadeRequestDTO { DestinatarioId = 2 };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(u => u.ObterUsuarioPorId(dto.DestinatarioId))
            .ReturnsAsync((Usuario?)null);

        var act = async () => await _service.EnviarPedidoDeAmizade(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task EnviarPedido_DeveLancarExcecao_QuandoJaExistePedido()
    {
        var usuario      = new Usuario("Pedro", "email@email.com", "123");
        var destinatario = new Usuario("Joao", "joao@email.com", "123");
        var dto          = new EnviarAmizadeRequestDTO { DestinatarioId = 2 };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(u => u.ObterUsuarioPorId(dto.DestinatarioId)).ReturnsAsync(destinatario);
        _repositoryMock.Setup(r => r.ValidarSeJaTemPedido(usuario.Id, dto.DestinatarioId)).ReturnsAsync(true);

        var act = async () => await _service.EnviarPedidoDeAmizade(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task EnviarPedido_DeveLancarExcecao_QuandoJaSaoAmigos()
    {
        var usuario      = new Usuario("Pedro", "email@email.com", "123");
        var destinatario = new Usuario("Joao", "joao@email.com", "123");
        var dto          = new EnviarAmizadeRequestDTO { DestinatarioId = 2 };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado()).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(u => u.ObterUsuarioPorId(dto.DestinatarioId)).ReturnsAsync(destinatario);
        _repositoryMock.Setup(r => r.ValidarSeJaTemPedido(usuario.Id, dto.DestinatarioId)).ReturnsAsync(false);
        _amizadeRepositoryMock.Setup(a => a.JaSaoAmigos(usuario.Id, dto.DestinatarioId)).ReturnsAsync(true);

        var act = async () => await _service.EnviarPedidoDeAmizade(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task RejeitarPedido_DeveChamarRepositorio()
    {
        var pedido = new AmizadePendente().Criar(1, 2);
        _repositoryMock.Setup(r => r.ObterPedidoDeAmizadePorId(1)).ReturnsAsync(pedido);

        await _service.RejeitarPedido(1);

        _repositoryMock.Verify(r => r.RejeitarPedido(pedido), Times.Once);
    }
}

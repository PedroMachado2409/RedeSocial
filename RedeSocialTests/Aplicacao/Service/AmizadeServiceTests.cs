using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Exceptions;

public class AmizadeServiceTests
{
    private readonly Mock<IAmizadeRepository> _repositoryMock;
    private readonly Mock<IAmizadePendenteRepository> _pendenteRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly AmizadeService _service;

    public AmizadeServiceTests()
    {
        _repositoryMock = new Mock<IAmizadeRepository>();
        _pendenteRepositoryMock = new Mock<IAmizadePendenteRepository>();
        _mapperMock = new Mock<IMapper>();

        _authServiceMock = new Mock<IAuthService>();

        _service = new AmizadeService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _pendenteRepositoryMock.Object,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task AceitarAmizade_DeveCriarAmizade_QuandoValido()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");
        var pedido = new AmizadePendente().Criar(2, usuario.Id);

        var dto = new AmizadeDTO
        {
            PedidoId = pedido.Id
        };

        var amizade = new Amizade().Criar(pedido.SolicitanteId, pedido.DestinatarioId, pedido.Id);

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _pendenteRepositoryMock.Setup(p => p.ObterPedidoDeAmizadePorId(dto.PedidoId))
            .ReturnsAsync(pedido);

        _repositoryMock.Setup(r => r.FazerAmizade(It.IsAny<Amizade>(), pedido))
            .ReturnsAsync(amizade);

        _mapperMock.Setup(m => m.Map<AmizadeDTO>(amizade))
            .Returns(dto);

        var result = await _service.AceitarAmizade(dto);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AceitarAmizade_DeveLancarExcecao_QuandoPedidoNaoExiste()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var dto = new AmizadeDTO
        {
            PedidoId = 1
        };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _pendenteRepositoryMock.Setup(p => p.ObterPedidoDeAmizadePorId(dto.PedidoId))
            .ReturnsAsync((AmizadePendente?)null);

        var act = async () => await _service.AceitarAmizade(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task AceitarAmizade_DeveLancarExcecao_QuandoNaoForDestinatario()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var pedido = new AmizadePendente().Criar(2, 999); // não é o usuário

        var dto = new AmizadeDTO
        {
            PedidoId = pedido.Id
        };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _pendenteRepositoryMock.Setup(p => p.ObterPedidoDeAmizadePorId(dto.PedidoId))
            .ReturnsAsync(pedido);

        var act = async () => await _service.AceitarAmizade(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task ObterAmizadePorId_DeveRetornarAmizade()
    {
        var amizade = new Amizade().Criar(1, 2, 10);

        _repositoryMock.Setup(r => r.ObterAmizadePorId(1))
            .ReturnsAsync(amizade);

        var result = await _service.ObterAmizadePorId(1);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ListarAmizadesDoUsuario_DeveRetornarListaMapeada()
    {
        var usuario = new Usuario("Pedro", "email@email.com", "123");

        var amizades = new List<Amizade>
        {
            new Amizade().Criar(usuario.Id, 2, 1)
        };

        var dto = new List<AmizadeDTO>
        {
            new AmizadeDTO()
        };

        _authServiceMock.Setup(a => a.ObterUsuarioAutenticado())
            .ReturnsAsync(usuario);

        _repositoryMock.Setup(r => r.ListarAmigosDoUsuario(usuario.Id))
            .ReturnsAsync(amizades);

        _mapperMock.Setup(m => m.Map<List<AmizadeDTO>>(amizades))
            .Returns(dto);

        var result = await _service.ListarAmizadesDoUsuario();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task RemoverAmizade_DeveChamarRepositorio()
    {
        var amizade = new Amizade().Criar(1, 2, 10);

        _repositoryMock.Setup(r => r.ObterAmizadePorId(1))
            .ReturnsAsync(amizade);

        await _service.RemoverAmizade(1);

        _repositoryMock.Verify(r => r.RemoverAmizade(amizade), Times.Once);
    }
}
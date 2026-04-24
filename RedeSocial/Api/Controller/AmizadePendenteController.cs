using Microsoft.AspNetCore.Mvc;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Aplicacao.Service;

namespace RedeSocial.Api.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AmizadePendenteController : ControllerBase
    {
        private readonly AmizadePendenteService _service;

        public AmizadePendenteController (AmizadePendenteService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSolicitacao([FromBody] AmizadePendenteDTO dto)
        {
            var pedido = await _service.EnviarPedidoDeAmizade(dto);
            return Ok(pedido);
        }

        [HttpGet("enviados")]
        public async Task<ActionResult<List<AmizadePendenteDTO>>> ListarPedidosEnviados()
        {
            var pedidos = await _service.ListarPedidosEnviados();
            return Ok(pedidos);
        }

        [HttpGet("Recebidos")]
        public async Task<ActionResult<List<AmizadePendenteDTO>>> ListarPedidosRecebidos()
        {
            var pedidos = await _service.ListarPedidosRecebidos();
            return Ok(pedidos);
        }


        [HttpDelete("Enviados/{id}")]
        public async Task RejeitarPedido(int id)
        {
            await _service.RejeitarPedido(id);
        }
    }
}

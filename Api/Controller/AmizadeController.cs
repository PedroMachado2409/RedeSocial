using Microsoft.AspNetCore.Mvc;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Aplicacao.Service;

namespace RedeSocial.Api.Controller
{

    [ApiController]
    [Route("/api/[controller]")]
    public class AmizadeController : ControllerBase
    {
        private readonly AmizadeService _service;

        public AmizadeController(AmizadeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> ObterAmizadesDoUsuario()
        {
            var amizades = await _service.ListarAmizadesDoUsuario();
            return Ok(amizades);
        }

        [HttpPost("aceitar")]
        public async Task<ActionResult<AmizadeDTO>> AceitarAmizade([FromBody] AmizadeDTO dto)
        {
            var amizade = await _service.AceitarAmizade(dto);
            return Ok(amizade);
        }

        [HttpDelete("Remover/{id}")]
        public async Task RemoverAmizade(int id)
        {
            await _service.RemoverAmizade(id);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Api.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly AuthService _service;

        public UsuarioController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar([FromBody] LoginRequestDTO dto)
        {
            var response = await _service.Autenticar(dto);
            return Ok(response);
        }

        [HttpPost("Registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            var response = await _service.RegistrarUsuario(usuario);
            return Ok(response);
        }

        [HttpGet("Autenticado")]
        public async Task<ActionResult<UsuarioDTO>> ObterUsuarioAutenticado()
        {
            var usuario = await _service.ObterUsuarioAutenticado();
            return Ok(usuario);
        }

        [HttpGet("nome")]
        public async Task<ActionResult<List<UsuarioDTO>>> ListarUsuariosPorNome([FromQuery] string nome)
        {
            var usuarios = await _service.ObterUsuarioPorNome(nome);
            return Ok(usuarios);
        }
        


    }
}

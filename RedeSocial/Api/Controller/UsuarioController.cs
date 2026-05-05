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
        private readonly IAuthService _service;

        public UsuarioController(IAuthService service) => _service = service;

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> Autenticar([FromBody] LoginRequestDTO dto)
            => Ok(await _service.Autenticar(dto));

        [HttpPost("Registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
            => Ok(await _service.RegistrarUsuario(usuario));

        [HttpGet("Autenticado")]
        public async Task<ActionResult<UsuarioDTO>> ObterUsuarioAutenticado()
            => Ok(await _service.ObterUsuarioAutenticado());

        [HttpGet("nome")]
        public async Task<ActionResult<List<UsuarioDTO>>> ListarUsuariosPorNome([FromQuery] string nome)
            => Ok(await _service.ObterUsuarioPorNome(nome));

        [HttpGet("perfil/{id:int}")]
        public async Task<ActionResult<PerfilResponseDTO>> ObterPerfil(int id)
            => Ok(await _service.ObterPerfil(id));

        [HttpPut("perfil")]
        public async Task<ActionResult<PerfilResponseDTO>> AtualizarPerfil(
            [FromBody] AtualizarPerfilRequestDTO dto)
            => Ok(await _service.AtualizarPerfil(dto));

        [HttpPut("senha")]
        public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequestDTO dto)
        {
            await _service.TrocarSenha(dto);
            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }
    }
}

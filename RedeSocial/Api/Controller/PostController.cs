using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Aplicacao.Service;

namespace RedeSocial.Api.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService    _service;
        private readonly CurtidaService _curtidaService;

        public PostController(PostService service, CurtidaService curtidaService)
        {
            _service        = service;
            _curtidaService = curtidaService;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarPost([FromBody] PostRequestDTO dto)
            => Ok(await _service.CadastrarPost(dto));

        [HttpGet]
        public async Task<ActionResult> ListarPosts()
            => Ok(await _service.ListarTodosOsPosts());

        [HttpGet("meus")]
        public async Task<ActionResult> ListarMeusPosts()
            => Ok(await _service.ListarPosts());

        [HttpPost("comentar")]
        public async Task<IActionResult> RegistrarComentario([FromBody] ComentarioRequestDTO dto)
            => Ok(await _service.AdicionarComentarioAoPost(dto));

        [HttpGet("PostsAmigos")]
        public async Task<ActionResult> ObterPostsDosAmigos()
            => Ok(await _service.ListarPostApenasDosAmigos());

        [HttpPost("{postId}/curtir")]
        public async Task<IActionResult> CurtirPost(int postId)
            => Ok(await _curtidaService.CadastrarCurtida(postId));

        [HttpDelete("{postId}/removerCurtida")]
        public async Task<IActionResult> RetirarCurtida(int postId)
        {
            await _curtidaService.RemoverCurtida(postId);
            return Ok();
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<ActionResult<List<PostResponseDTO>>> ListarPostsDoUsuario(int usuarioId)
            => Ok(await _service.ListarPostsDoUsuario(usuarioId));
    }
}

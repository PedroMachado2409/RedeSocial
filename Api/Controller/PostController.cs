using Microsoft.AspNetCore.Mvc;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Aplicacao.Service;

namespace RedeSocial.Api.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly PostService _service;

        public PostController(PostService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarPost([FromBody] PostRequestDTO dto)
        {
            var novoPost = await _service.CadastrarPost(dto);
            return Ok(novoPost);
        }

        [HttpGet]
        public async Task<ActionResult> ListarPosts()
        {
            var posts = await _service.ListarPosts();
            return Ok(posts);
        }

        [HttpPost("comentar")]
        public async Task<IActionResult> RegistrarComentario([FromBody] ComentarioRequestDTO dto)
        {
            var comentario = await _service.AdicionarComentarioAoPost(dto);
            return Ok(comentario);
        }


    }
}

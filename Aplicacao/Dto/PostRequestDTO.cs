namespace RedeSocial.Aplicacao.Dto
{
    public class PostRequestDTO
    {
        public string Titulo { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public List<ComentarioRequestDTO>? Comentarios { get; set; }
    }

    public class ComentarioRequestDTO 
    {
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public int PostId { get; set; }

        public DateTime DtComentario { get; set; }
        public string Descricao { get; set; }

    }



}

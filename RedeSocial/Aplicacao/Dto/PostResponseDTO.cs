namespace RedeSocial.Aplicacao.Dto
{
    public class PostResponseDTO
    {
        public int    Id           { get; set; }
        public int    UsuarioId    { get; set; }
        public string UsuarioNome  { get; set; } = string.Empty;
        public string? UsuarioFotoPerfilBase64 { get; set; }
        public string Titulo       { get; set; } = string.Empty;
        public int    Curtidas     { get; set; }
        public List<ComentarioResponseDTO> Comentarios { get; set; } = new();
        public string? ImagemBase64 { get; set; }
    }

    public class ComentarioResponseDTO
    {
        public int      Id           { get; set; }
        public int      UsuarioId    { get; set; }
        public string   UsuarioNome  { get; set; } = string.Empty;
        public string?  UsuarioFotoPerfilBase64 { get; set; }
        public int      PostId       { get; set; }
        public DateTime DtComentario { get; set; }
        public string   Descricao    { get; set; } = string.Empty;
    }
}

namespace RedeSocial.Aplicacao.Dto
{
    public class PostResponseDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string Titulo { get; set; }
        public List<ComentarioResponseDTO> Comentarios { get; set; }
    }

    public class ComentarioResponseDTO 
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public int PostId { get; set; }
        public DateTime DtComentario {  get; set; }
        public string Descricao { get; set; }
    }

}

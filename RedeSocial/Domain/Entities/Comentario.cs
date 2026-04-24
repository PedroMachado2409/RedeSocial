namespace RedeSocial.Domain.Entities
{
    public class Comentario
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public DateTime DtComentario {  get; set; } = DateTime.UtcNow;
        public string Descricao { get; set; } = string.Empty;

        public static Comentario Criar(int usuarioId, string descricao, int postId)
        {
            return new Comentario 
            { 
                UsuarioId = usuarioId,
                Descricao = descricao,
                PostId = postId
            };

        }

    }
}

namespace RedeSocial.Domain.Entities
{
    public class Curtida
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }

        protected Curtida() { }

        public Curtida (int usuarioId,int postId)
        {
            UsuarioId = usuarioId;
            PostId = postId;
        }

    }
}

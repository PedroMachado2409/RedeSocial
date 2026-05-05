namespace RedeSocial.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public Usuario Usuario { get; set; } = null!;
        public int UsuarioId { get; set; }
        public int Curtidas { get; set; } = 0;
        public List<Comentario> Comentarios { get; set; } = new();
        public byte[]? ImagemPost { get; set; }

        public static Post Criar(
            string titulo,
            List<Comentario> comentarios,
            int usuarioId,
            int curtidas,
            byte[]? imagemPost = null)
        {
            return new Post
            {
                UsuarioId   = usuarioId,
                Titulo      = titulo,
                Comentarios = comentarios,
                Curtidas    = curtidas,
                ImagemPost  = imagemPost,
            };
        }

        public void AdicionarCurtida() => Curtidas++;
        public void RemoverCurtida()   => Curtidas--;
    }
}

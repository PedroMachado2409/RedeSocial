namespace RedeSocial.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();


        public static Post Criar(string titulo, List<Comentario> itens, int usarioId)
        {
            return new Post
            {
                UsuarioId = usarioId,
                Titulo = titulo,
                Comentarios = itens
            };
        }


    }

}

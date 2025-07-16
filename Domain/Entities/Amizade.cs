using System.ComponentModel.DataAnnotations;

namespace RedeSocial.Domain.Entities
{
    public class Amizade
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int AmigoId { get; set; }
        public Usuario Amigo { get; set; }

        public int PedidoId { get; set; }
        public DateTime DataConfirmacao { get; set; } = DateTime.UtcNow;

        public Amizade Criar(int usuarioId, int amigoId, int pedidoId)
        {
            return new Amizade
            { 
                UsuarioId = usuarioId,
                AmigoId = amigoId,
                PedidoId = pedidoId,
            };


        }

    }
}

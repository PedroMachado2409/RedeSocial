using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Dto
{
    public class AmizadeDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AmigoId { get; set; }
        public int PedidoId { get; set; }
        public DateTime DataConfirmacao { get; set; } = DateTime.UtcNow;
    }
}

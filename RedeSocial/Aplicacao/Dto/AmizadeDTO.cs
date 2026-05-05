using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Dto
{
    public class AmizadeDTO
    {
        public int      Id                    { get; set; }
        public int      UsuarioId             { get; set; }
        public int      AmigoId               { get; set; }
        public string   AmigoNome             { get; set; } = string.Empty;
        public string?  AmigoFotoPerfilBase64 { get; set; }
        public int      PedidoId              { get; set; }
        public DateTime DataConfirmacao       { get; set; } = DateTime.UtcNow;
    }
}

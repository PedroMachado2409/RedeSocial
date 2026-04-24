using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Dto
{
    public class AmizadePendenteDTO
    {
        public int Id { get; set; }
        public int SolicitanteId { get; set; }
        public int DestinatarioId { get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    }
}

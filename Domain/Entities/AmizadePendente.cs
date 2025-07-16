using System.ComponentModel.DataAnnotations;

namespace RedeSocial.Domain.Entities
{
    public class AmizadePendente
    {
        [Key]
        public int Id { get; set; }
        public int SolicitanteId { get; set; }
        public Usuario Solicitante { get; set; }
        public int DestinatarioId { get; set; }
        public Usuario Destinatario{ get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

        public AmizadePendente Criar(int solicitanteId, int destinatarioId)
        {
            return new AmizadePendente
            {
                SolicitanteId = solicitanteId,
                DestinatarioId = destinatarioId
            };
        }
    }
}

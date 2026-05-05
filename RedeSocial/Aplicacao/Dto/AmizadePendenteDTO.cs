using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Dto
{
    public class AmizadePendenteDTO
    {
        public int      Id                          { get; set; }
        public int      SolicitanteId               { get; set; }
        public string   SolicitanteNome             { get; set; } = string.Empty;
        public string?  SolicitanteFotoPerfilBase64 { get; set; }
        public int      DestinatarioId              { get; set; }
        public string   DestinatarioNome            { get; set; } = string.Empty;
        public string?  DestinatarioFotoPerfilBase64{ get; set; }
        public DateTime DataSolicitacao             { get; set; } = DateTime.UtcNow;
    }
}

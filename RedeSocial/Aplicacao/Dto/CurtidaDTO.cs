using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Dto
{
    public class CurtidaDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PostId { get; set; }
    }

    public class CurtidaRequestDTO
    {
        public int UsuarioId { get; set; }
        public int PostId { get; set; }
    }
}

namespace RedeSocial.Aplicacao.Dto
{
    public class PerfilResponseDTO
    {
        public int     Id               { get; set; }
        public string  Nome             { get; set; } = string.Empty;
        public string  Email            { get; set; } = string.Empty;
        public string? DescricaoPerfil  { get; set; }
        public string? FotoPerfilBase64 { get; set; }
        public string? FotoBannerBase64 { get; set; }
        public int     TotalAmigos      { get; set; }
        public int     TotalPosts       { get; set; }
    }

    public class AtualizarPerfilRequestDTO
    {
        public string? DescricaoPerfil  { get; set; }
        public string? FotoPerfilBase64 { get; set; }
        public string? FotoBannerBase64 { get; set; }
    }

    public class TrocarSenhaRequestDTO
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha  { get; set; } = string.Empty;
    }
}

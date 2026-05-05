namespace RedeSocial.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public byte[]? FotoPerfil { get; set; }
        public byte[]? FotoBanner { get; set; }
        public string? DescricaoPerfil { get; set; }

        protected Usuario() { }

        public Usuario(string nome, string email, string senha)
        {
            Nome = nome;
            Email = email;
            Senha = senha;
        }

        public void Atualizar(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public void AtualizarPerfil(string? descricaoPerfil, byte[]? fotoPerfil, byte[]? fotoBanner)
        {
            if (descricaoPerfil is not null)
                DescricaoPerfil = descricaoPerfil;

            if (fotoPerfil is not null && fotoPerfil.Length > 0)
                FotoPerfil = fotoPerfil;

            if (fotoBanner is not null && fotoBanner.Length > 0)
                FotoBanner = fotoBanner;
        }

        public void TrocarSenha(string novaSenhaCriptografada)
        {
            Senha = novaSenhaCriptografada;
        }

        public void Ativar()   => Ativo = true;
        public void Inativar() => Ativo = false;
    }
}

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace RedeSocial.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
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
        public void Ativar() => Ativo = true;
        public void Inativar() => Ativo = false;
    }
}

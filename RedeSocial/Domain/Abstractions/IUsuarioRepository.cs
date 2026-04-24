using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IUsuarioRepository
    {
        public Task<Usuario> CadastrarUsuario(Usuario usuario);
        public Task<List<Usuario>> ListarTodosOsUsuarios();
        public Task<Usuario?> ObterUsuarioPorId(int id);
        public Task<Usuario?> ObterUsuarioPorEmail(string email);
        public Task<List<Usuario>>ObterUsuarioPorNome(string nome);
    }
}

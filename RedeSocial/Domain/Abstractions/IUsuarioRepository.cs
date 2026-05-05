using RedeSocial.Domain.Entities;

namespace RedeSocial.Domain.Abstractions
{
    public interface IUsuarioRepository
    {
        Task<Usuario>       CadastrarUsuario(Usuario usuario);
        Task<List<Usuario>> ListarTodosOsUsuarios();
        Task<Usuario?>      ObterUsuarioPorId(int id);
        Task<Usuario?>      ObterUsuarioPorEmail(string email);
        Task<List<Usuario>> ObterUsuarioPorNome(string nome);
        Task<Usuario>       AtualizarPerfil(Usuario usuario);
        Task                TrocarSenha(Usuario usuario);
    }
}

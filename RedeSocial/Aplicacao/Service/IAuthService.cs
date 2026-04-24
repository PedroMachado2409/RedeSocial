using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Service
{
    public interface IAuthService
    {
        Task<Usuario> RegistrarUsuario(Usuario usuario);
        Task<LoginResponseDTO> Autenticar(LoginRequestDTO dto);
        Task<Usuario?> ObterUsuarioAutenticado();
        Task<List<UsuarioDTO>> ObterUsuarioPorNome(string nome);
    }
}
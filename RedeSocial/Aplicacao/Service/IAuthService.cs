using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Service
{
    public interface IAuthService
    {
        Task<Usuario>           RegistrarUsuario(Usuario usuario);
        Task<LoginResponseDTO>  Autenticar(LoginRequestDTO dto);
        Task<Usuario?>          ObterUsuarioAutenticado();
        Task<List<UsuarioDTO>>  ObterUsuarioPorNome(string nome);
        Task<PerfilResponseDTO> ObterPerfil(int usuarioId);
        Task<PerfilResponseDTO> AtualizarPerfil(AtualizarPerfilRequestDTO dto);
        Task                    TrocarSenha(TrocarSenhaRequestDTO dto);
    }
}

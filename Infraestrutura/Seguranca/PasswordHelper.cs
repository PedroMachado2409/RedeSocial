using BCrypt.Net;

namespace RedeSocial.Infraestrutura.Seguranca
{
    public class PasswordHelper
    {
        public static string CriptografarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public static bool VerificarSenha(string senha, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
    }
}

using FluentValidation;
using RedeSocial.Aplicacao.Dto;

namespace RedeSocial.Aplicacao.Validators
{
    public class AtualizarPerfilValidator : AbstractValidator<AtualizarPerfilRequestDTO>
    {
        private const int MaxBase64Length = 3_600_000; // ~2 MB

        public AtualizarPerfilValidator()
        {
            RuleFor(x => x.DescricaoPerfil)
                .MaximumLength(300)
                .WithMessage("A descrição pode ter no máximo 300 caracteres.")
                .When(x => x.DescricaoPerfil != null);

            RuleFor(x => x.FotoPerfilBase64)
                .Must(b64 => b64 == null || b64.Length <= MaxBase64Length)
                .WithMessage("A foto de perfil deve ter no máximo 2 MB.")
                .Must(SerBase64Valido)
                .WithMessage("Foto de perfil inválida.")
                .When(x => !string.IsNullOrWhiteSpace(x.FotoPerfilBase64));

            RuleFor(x => x.FotoBannerBase64)
                .Must(b64 => b64 == null || b64.Length <= MaxBase64Length)
                .WithMessage("O banner deve ter no máximo 2 MB.")
                .Must(SerBase64Valido)
                .WithMessage("Banner inválido.")
                .When(x => !string.IsNullOrWhiteSpace(x.FotoBannerBase64));
        }

        private static bool SerBase64Valido(string? b64)
        {
            if (string.IsNullOrWhiteSpace(b64)) return true;
            try { Convert.FromBase64String(b64); return true; }
            catch { return false; }
        }
    }

    public class TrocarSenhaValidator : AbstractValidator<TrocarSenhaRequestDTO>
    {
        public TrocarSenhaValidator()
        {
            RuleFor(x => x.SenhaAtual)
                .NotEmpty().WithMessage("Informe a senha atual.");

            RuleFor(x => x.NovaSenha)
                .NotEmpty().WithMessage("Informe a nova senha.")
                .MinimumLength(6).WithMessage("A nova senha deve ter pelo menos 6 caracteres.")
                .NotEqual(x => x.SenhaAtual).WithMessage("A nova senha deve ser diferente da atual.");
        }
    }
}

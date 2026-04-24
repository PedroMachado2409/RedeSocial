using FluentValidation;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;

namespace RedeSocial.Aplicacao.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(u => u.Nome)
                .NotEmpty().WithMessage(Messages.NomeObrigatorio)
                .MinimumLength(3).WithMessage(Messages.NomeMinimo);

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage(Messages.EmailObrigatorio)
                .EmailAddress().WithMessage(Messages.EmailInvalido);

            RuleFor(u => u.Senha)
                .NotEmpty().WithMessage(Messages.SenhaObrigatoria)
                .MinimumLength(6).WithMessage(Messages.SenhaMinima);
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.EmailObrigatorio)
                .EmailAddress().WithMessage(Messages.EmailInvalido);

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage(Messages.SenhaObrigatoria);
   
        }
    }
}

using FluentValidation;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;

namespace RedeSocial.Aplicacao.Validators
{
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.Titulo)
                .NotEmpty().WithMessage(Messages.TituloObrigatorio)
                .MinimumLength(3).WithMessage(Messages.TituloMinimo)
                .MaximumLength(100).WithMessage(Messages.TituloMaximo);

            RuleForEach(p => p.Comentarios)
                .NotNull().WithMessage(Messages.ComentarioNulo);
        }
    }
}

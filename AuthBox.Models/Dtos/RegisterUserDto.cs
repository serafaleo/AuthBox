using FluentValidation;

namespace AuthBox.Models.Dtos;
public class RegisterUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmation { get; set; }
}

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(registerInfo => registerInfo.Email)
            .NotEmpty().WithMessage("Email precisa ser preenchido.")
            .EmailAddress().WithMessage("Email preenchido é inválido.");

        RuleFor(registerInfo => registerInfo.Password)
            .NotEmpty().WithMessage("Senha precisa ser preenchida.")
            .MinimumLength(8).WithMessage("A senha precisa ter no mínimo 8 caracteres.")
            .Matches(@"[A-Z]+").WithMessage("A senha precisa conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]+").WithMessage("A senha precisa conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]+").WithMessage("A senha precisa conter pelo menos um dígito.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("A senha precisa conter pelo menos um caractere especial.");

        RuleFor(registerInfo => registerInfo.PasswordConfirmation)
            .Equal(registerInfo => registerInfo.Password).WithMessage("As senhas são diferentes.");
    }
}

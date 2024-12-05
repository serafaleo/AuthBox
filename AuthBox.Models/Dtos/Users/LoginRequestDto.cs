using FluentValidation;

namespace AuthBox.Models.Dtos.Users;
public class LoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(registerInfo => registerInfo.Email)
            .NotEmpty().WithMessage("Email precisa ser preenchido.")
            .EmailAddress().WithMessage("Email preenchido é inválido.");

        RuleFor(registerInfo => registerInfo.Password)
            .NotEmpty().WithMessage("Senha precisa ser preenchida.");
    }
}
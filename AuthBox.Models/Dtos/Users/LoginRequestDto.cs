using FluentValidation;
using FluentValidation.Results;

namespace AuthBox.Models.Dtos.Users;
public class LoginRequestDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
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

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        ValidationResult result = await ValidateAsync(ValidationContext<LoginRequestDto>.CreateWithOptions((LoginRequestDto)model, x => x.IncludeProperties(propertyName)));

        if (result.IsValid)
            return Array.Empty<string>();

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
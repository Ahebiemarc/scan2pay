using FluentValidation;
using scan2pay.DTOs;

namespace scan2pay.Validation;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Le mot de passe doit contenir au moins une majuscule.")
            .Matches("[a-z]").WithMessage("Le mot de passe doit contenir au moins une minuscule.")
            .Matches("[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Les mots de passe ne correspondent pas.");
        RuleFor(x => x.UserType).NotEmpty().Must(type => type.ToLower() == "client" || type.ToLower() == "marchand")
            .WithMessage("UserType doit être 'Client' ou 'Marchand'.");
    }
}
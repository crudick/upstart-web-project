using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class LoginApiRequestValidator : AbstractValidator<LoginApiRequest>
{
    public LoginApiRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
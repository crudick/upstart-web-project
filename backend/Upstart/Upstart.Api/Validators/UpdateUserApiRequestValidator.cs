using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class UpdateUserApiRequestValidator : AbstractValidator<UpdateUserApiRequest>
{
    public UpdateUserApiRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));
    }
}
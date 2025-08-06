using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class UpdatePollApiRequestValidator : AbstractValidator<UpdatePollApiRequest>
{
    public UpdatePollApiRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty()
            .WithMessage("Question is required")
            .MaximumLength(500)
            .WithMessage("Question cannot exceed 500 characters");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future")
            .When(x => x.ExpiresAt.HasValue);
    }
}
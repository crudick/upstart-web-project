using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class CreatePollAnswerApiRequestValidator : AbstractValidator<CreatePollAnswerApiRequest>
{
    public CreatePollAnswerApiRequestValidator()
    {
        RuleFor(x => x.PollId)
            .GreaterThan(0)
            .WithMessage("Poll ID must be greater than 0");

        RuleFor(x => x.AnswerText)
            .NotEmpty()
            .WithMessage("Answer text is required")
            .MaximumLength(500)
            .WithMessage("Answer text cannot exceed 500 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0)
            .WithMessage("Display order must be greater than 0");
    }
}
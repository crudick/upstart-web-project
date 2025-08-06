using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class CreatePollStatApiRequestValidator : AbstractValidator<CreatePollStatApiRequest>
{
    public CreatePollStatApiRequestValidator()
    {
        RuleFor(x => x.PollId)
            .GreaterThan(0)
            .WithMessage("Poll ID must be greater than 0");

        RuleFor(x => x.PollAnswerId)
            .GreaterThan(0)
            .WithMessage("Poll Answer ID must be greater than 0");

        // UserId validation removed - it's set by the backend from JWT token
    }
}
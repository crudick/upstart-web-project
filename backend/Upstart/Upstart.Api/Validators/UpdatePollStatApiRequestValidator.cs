using FluentValidation;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class UpdatePollStatApiRequestValidator : AbstractValidator<UpdatePollStatApiRequest>
{
    public UpdatePollStatApiRequestValidator()
    {
        RuleFor(x => x.PollAnswerId)
            .GreaterThan(0)
            .WithMessage("Poll Answer ID must be greater than 0");
    }
}
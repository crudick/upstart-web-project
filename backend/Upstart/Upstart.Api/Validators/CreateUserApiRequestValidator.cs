using FluentValidation;
using Upstart.Api.Endpoints;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class CreateUserApiRequestValidator : AbstractValidator<CreateUserApiRequest>
{
    public CreateUserApiRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today)
            .WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.SocialSecurityNumber)
            .Length(9, 11)
            .WithMessage("Social Security Number must be between 9 and 11 characters")
            .When(x => !string.IsNullOrEmpty(x.SocialSecurityNumber));

        RuleFor(x => x.AddressLine1)
            .MaximumLength(100)
            .WithMessage("Address line 1 cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AddressLine1));

        RuleFor(x => x.AddressLine2)
            .MaximumLength(100)
            .WithMessage("Address line 2 cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AddressLine2));

        RuleFor(x => x.City)
            .MaximumLength(50)
            .WithMessage("City cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.City));

        RuleFor(x => x.State)
            .Length(2)
            .WithMessage("State must be 2 characters")
            .When(x => !string.IsNullOrEmpty(x.State));

        RuleFor(x => x.ZipCode)
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Invalid zip code format")
            .When(x => !string.IsNullOrEmpty(x.ZipCode));

        RuleFor(x => x.AnnualIncome)
            .GreaterThan(0)
            .WithMessage("Annual income must be greater than 0")
            .When(x => x.AnnualIncome.HasValue);

        RuleFor(x => x.EmploymentStatus)
            .MaximumLength(50)
            .WithMessage("Employment status cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.EmploymentStatus));

        RuleFor(x => x.CreditScore)
            .InclusiveBetween(300, 850)
            .WithMessage("Credit score must be between 300 and 850")
            .When(x => x.CreditScore.HasValue);
    }
}
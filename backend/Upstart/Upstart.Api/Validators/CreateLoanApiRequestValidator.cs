using FluentValidation;
using Upstart.Api.Endpoints;
using Upstart.Api.Models;

namespace Upstart.Api.Validators;

public class CreateLoanApiRequestValidator : AbstractValidator<CreateLoanApiRequest>
{
    public CreateLoanApiRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.LoanAmount)
            .GreaterThan(0)
            .WithMessage("Loan amount must be greater than 0")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Loan amount cannot exceed $1,000,000");

        RuleFor(x => x.InterestRate)
            .GreaterThan(0)
            .WithMessage("Interest rate must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Interest rate cannot exceed 50%");

        RuleFor(x => x.TermMonths)
            .GreaterThan(0)
            .WithMessage("Term must be greater than 0 months")
            .LessThanOrEqualTo(480)
            .WithMessage("Term cannot exceed 480 months (40 years)");

        RuleFor(x => x.MonthlyPayment)
            .GreaterThan(0)
            .WithMessage("Monthly payment must be greater than 0");

        RuleFor(x => x.LoanPurpose)
            .NotEmpty()
            .WithMessage("Loan purpose is required")
            .MaximumLength(100)
            .WithMessage("Loan purpose cannot exceed 100 characters");

        RuleFor(x => x.LoanStatus)
            .NotEmpty()
            .WithMessage("Loan status is required")
            .Must(BeValidLoanStatus)
            .WithMessage("Invalid loan status. Valid values are: Pending, Approved, Rejected, Disbursed, Active, Closed");

        RuleFor(x => x.ApplicationDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Application date cannot be in the future");

        RuleFor(x => x.ApprovalDate)
            .GreaterThanOrEqualTo(x => x.ApplicationDate)
            .WithMessage("Approval date must be on or after application date")
            .When(x => x.ApprovalDate.HasValue);

        RuleFor(x => x.DisbursementDate)
            .GreaterThanOrEqualTo(x => x.ApprovalDate)
            .WithMessage("Disbursement date must be on or after approval date")
            .When(x => x.DisbursementDate.HasValue && x.ApprovalDate.HasValue);

        RuleFor(x => x.MaturityDate)
            .GreaterThan(x => x.ApplicationDate)
            .WithMessage("Maturity date must be after application date")
            .When(x => x.MaturityDate.HasValue);

        RuleFor(x => x.OutstandingBalance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Outstanding balance cannot be negative");

        RuleFor(x => x.TotalPaymentsMade)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total payments made cannot be negative");

        RuleFor(x => x.NextPaymentDueDate)
            .GreaterThan(DateTime.Today)
            .WithMessage("Next payment due date must be in the future")
            .When(x => x.NextPaymentDueDate.HasValue);

        RuleFor(x => x.PaymentFrequency)
            .NotEmpty()
            .WithMessage("Payment frequency is required")
            .Must(BeValidPaymentFrequency)
            .WithMessage("Invalid payment frequency. Valid values are: Monthly, BiWeekly, Weekly");

        RuleFor(x => x.LateFees)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Late fees cannot be negative");

        RuleFor(x => x.OriginationFee)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Origination fee cannot be negative");

        RuleFor(x => x.APR)
            .GreaterThan(0)
            .WithMessage("APR must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("APR cannot exceed 50%")
            .When(x => x.APR.HasValue);

        RuleFor(x => x.LoanOfficerNotes)
            .MaximumLength(1000)
            .WithMessage("Loan officer notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.LoanOfficerNotes));
    }

    private static bool BeValidLoanStatus(string status)
    {
        var validStatuses = new[] { "Pending", "Approved", "Rejected", "Disbursed", "Active", "Closed" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPaymentFrequency(string frequency)
    {
        var validFrequencies = new[] { "Monthly", "BiWeekly", "Weekly" };
        return validFrequencies.Contains(frequency, StringComparer.OrdinalIgnoreCase);
    }
}
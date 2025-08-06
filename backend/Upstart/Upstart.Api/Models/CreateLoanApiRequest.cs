namespace Upstart.Api.Models;

public record CreateLoanApiRequest(
    int UserId,
    decimal LoanAmount,
    decimal InterestRate,
    int TermMonths,
    decimal MonthlyPayment,
    string LoanPurpose,
    string LoanStatus,
    DateTime ApplicationDate,
    DateTime? ApprovalDate,
    DateTime? DisbursementDate,
    DateTime? MaturityDate,
    decimal OutstandingBalance,
    decimal TotalPaymentsMade,
    DateTime? NextPaymentDueDate,
    string PaymentFrequency,
    decimal LateFees,
    decimal OriginationFee,
    decimal? APR,
    string? LoanOfficerNotes
);
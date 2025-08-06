using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoansRepository _loansRepository;
    private readonly ILogger<LoanService> _logger;

    public LoanService(ILoansRepository loansRepository, ILogger<LoanService> logger)
    {
        _loansRepository = loansRepository;
        _logger = logger;
    }

    public async Task<LoanModel> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating {loan}", request);
        var loan = new LoanModel
        {
            UserId = request.UserId,
            LoanAmount = request.LoanAmount,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths,
            MonthlyPayment = request.MonthlyPayment,
            LoanPurpose = request.LoanPurpose,
            LoanStatus = request.LoanStatus,
            ApplicationDate = DateTime.SpecifyKind(request.ApplicationDate, DateTimeKind.Utc),
            ApprovalDate = request.ApprovalDate.HasValue ? DateTime.SpecifyKind(request.ApprovalDate.Value, DateTimeKind.Utc) : null,
            DisbursementDate = request.DisbursementDate.HasValue ? DateTime.SpecifyKind(request.DisbursementDate.Value, DateTimeKind.Utc) : null,
            MaturityDate = request.MaturityDate.HasValue ? DateTime.SpecifyKind(request.MaturityDate.Value, DateTimeKind.Utc) : null,
            OutstandingBalance = request.OutstandingBalance,
            TotalPaymentsMade = request.TotalPaymentsMade,
            NextPaymentDueDate = request.NextPaymentDueDate.HasValue ? DateTime.SpecifyKind(request.NextPaymentDueDate.Value, DateTimeKind.Utc) : null,
            PaymentFrequency = request.PaymentFrequency,
            LateFees = request.LateFees,
            OriginationFee = request.OriginationFee,
            APR = request.APR,
            LoanOfficerNotes = request.LoanOfficerNotes,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _loansRepository.CreateAsync(loan);
    }
}

public record CreateLoanRequest(
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
namespace Upstart.Domain.Models;

public class LoanModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public string LoanPurpose { get; set; } = string.Empty;
    public string LoanStatus { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public DateTime? DisbursementDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal TotalPaymentsMade { get; set; }
    public DateTime? NextPaymentDueDate { get; set; }
    public string PaymentFrequency { get; set; } = "Monthly";
    public decimal LateFees { get; set; }
    public decimal OriginationFee { get; set; }
    public decimal? APR { get; set; }
    public string? LoanOfficerNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public UserModel User { get; set; } = null!;
}
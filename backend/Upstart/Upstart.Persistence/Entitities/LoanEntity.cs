using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upstart.Persistence.Entitities;

[Table("loans")]
public class LoanEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("loan_amount", TypeName = "decimal(18,2)")]
    [Required]
    public decimal LoanAmount { get; set; }

    [Column("interest_rate", TypeName = "decimal(5,4)")]
    [Required]
    public decimal InterestRate { get; set; }

    [Column("term_months")]
    [Required]
    public int TermMonths { get; set; }

    [Column("monthly_payment", TypeName = "decimal(18,2)")]
    [Required]
    public decimal MonthlyPayment { get; set; }

    [Column("loan_purpose")]
    [MaxLength(100)]
    [Required]
    public string LoanPurpose { get; set; } = string.Empty;

    [Column("loan_status")]
    [MaxLength(50)]
    [Required]
    public string LoanStatus { get; set; } = string.Empty;

    [Column("application_date")]
    [Required]
    public DateTime ApplicationDate { get; set; }

    [Column("approval_date")]
    public DateTime? ApprovalDate { get; set; }

    [Column("disbursement_date")]
    public DateTime? DisbursementDate { get; set; }

    [Column("maturity_date")]
    public DateTime? MaturityDate { get; set; }

    [Column("outstanding_balance", TypeName = "decimal(18,2)")]
    public decimal OutstandingBalance { get; set; }

    [Column("total_payments_made", TypeName = "decimal(18,2)")]
    public decimal TotalPaymentsMade { get; set; } = 0;

    [Column("next_payment_due_date")]
    public DateTime? NextPaymentDueDate { get; set; }

    [Column("payment_frequency")]
    [MaxLength(20)]
    public string PaymentFrequency { get; set; } = "Monthly";

    [Column("late_fees", TypeName = "decimal(18,2)")]
    public decimal LateFees { get; set; } = 0;

    [Column("origination_fee", TypeName = "decimal(18,2)")]
    public decimal OriginationFee { get; set; } = 0;

    [Column("apr", TypeName = "decimal(5,4)")]
    public decimal? APR { get; set; }

    [Column("loan_officer_notes")]
    [MaxLength(1000)]
    public string? LoanOfficerNotes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public UserEntity User { get; set; } = null!;
}
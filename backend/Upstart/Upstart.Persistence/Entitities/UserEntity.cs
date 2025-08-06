using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upstart.Persistence.Entitities;

[Table("users")]
public class UserEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("first_name")]
    [MaxLength(100)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    [MaxLength(100)]
    [Required]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    [MaxLength(255)]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Column("phone_number")]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("social_security_number")]
    [MaxLength(11)]
    public string? SocialSecurityNumber { get; set; }

    [Column("address_line_1")]
    [MaxLength(255)]
    public string? AddressLine1 { get; set; }

    [Column("address_line_2")]
    [MaxLength(255)]
    public string? AddressLine2 { get; set; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [MaxLength(2)]
    public string? State { get; set; }

    [Column("zip_code")]
    [MaxLength(10)]
    public string? ZipCode { get; set; }

    [Column("annual_income", TypeName = "decimal(18,2)")]
    public decimal? AnnualIncome { get; set; }

    [Column("employment_status")]
    [MaxLength(50)]
    public string? EmploymentStatus { get; set; }

    [Column("credit_score")]
    public int? CreditScore { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LoanEntity> Loans { get; set; } = new List<LoanEntity>();
}
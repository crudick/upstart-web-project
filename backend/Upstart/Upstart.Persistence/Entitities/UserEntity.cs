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
    public string? FirstName { get; set; }

    [Column("last_name")]
    [MaxLength(100)]
    public string? LastName { get; set; }

    [Column("email")]
    [MaxLength(255)]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Column("password_hash")]
    [MaxLength(255)]
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("phone_number")]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }


    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PollEntity> Polls { get; set; } = new List<PollEntity>();
    
    public ICollection<PollStatEntity> PollStats { get; set; } = new List<PollStatEntity>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upstart.Persistence.Entitities;

[Table("polls")]
public class PollEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    
    [Column("poll_guid")]
    public string PollGuid { get; set; } = string.Empty;

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("session_id")]
    [MaxLength(255)]
    public string? SessionId { get; set; }

    [Column("question")]
    [MaxLength(500)]
    [Required]
    public string Question { get; set; } = string.Empty;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_multiple_choice")]
    public bool IsMultipleChoice { get; set; } = false;

    [Column("requires_authentication")]
    public bool RequiresAuthentication { get; set; } = false;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }

    public ICollection<PollAnswerEntity> Answers { get; set; } = new List<PollAnswerEntity>();

    public ICollection<PollStatEntity> Stats { get; set; } = new List<PollStatEntity>();
}
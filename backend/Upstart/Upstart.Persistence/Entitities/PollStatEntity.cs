using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upstart.Persistence.Entitities;

[Table("poll_stats")]
public class PollStatEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("poll_id")]
    [Required]
    public int PollId { get; set; }

    [Column("poll_answer_id")]
    [Required]
    public int PollAnswerId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("session_id")]
    [MaxLength(255)]
    public string? SessionId { get; set; }

    [Column("selected_at")]
    public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("PollId")]
    public PollEntity Poll { get; set; } = null!;

    [ForeignKey("PollAnswerId")]
    public PollAnswerEntity PollAnswer { get; set; } = null!;

    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }
}
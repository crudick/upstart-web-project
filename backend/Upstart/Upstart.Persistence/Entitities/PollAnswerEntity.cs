using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upstart.Persistence.Entitities;

[Table("poll_answers")]
public class PollAnswerEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("poll_id")]
    [Required]
    public int PollId { get; set; }

    [Column("answer_text")]
    [MaxLength(500)]
    [Required]
    public string AnswerText { get; set; } = string.Empty;

    [Column("display_order")]
    public int DisplayOrder { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("PollId")]
    public PollEntity Poll { get; set; } = null!;

    public ICollection<PollStatEntity> Stats { get; set; } = new List<PollStatEntity>();
}
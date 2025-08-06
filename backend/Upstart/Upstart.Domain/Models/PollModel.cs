namespace Upstart.Domain.Models;

public class PollModel
{
    public int Id { get; set; }
    public string PollGuid { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsMultipleChoice { get; set; } = false;
    public bool RequiresAuthentication { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public UserModel? User { get; set; }
    public ICollection<PollAnswerModel> Answers { get; set; } = new List<PollAnswerModel>();
    public ICollection<PollStatModel> Stats { get; set; } = new List<PollStatModel>();
}
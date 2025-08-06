namespace Upstart.Domain.Models;

public class PollAnswerModel
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PollModel? Poll { get; set; }
    public ICollection<PollStatModel> Stats { get; set; } = new List<PollStatModel>();
}
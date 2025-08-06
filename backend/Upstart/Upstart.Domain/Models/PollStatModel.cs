namespace Upstart.Domain.Models;

public class PollStatModel
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public int PollAnswerId { get; set; }
    public int? UserId { get; set; }
    public DateTime SelectedAt { get; set; }
    public PollModel? Poll { get; set; }
    public PollAnswerModel? PollAnswer { get; set; }
    public UserModel? User { get; set; }
}
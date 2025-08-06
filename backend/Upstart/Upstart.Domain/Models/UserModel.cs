namespace Upstart.Domain.Models;

public class UserModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<PollModel> Polls { get; set; } = new List<PollModel>();
    public ICollection<PollStatModel> PollStats { get; set; } = new List<PollStatModel>();
}
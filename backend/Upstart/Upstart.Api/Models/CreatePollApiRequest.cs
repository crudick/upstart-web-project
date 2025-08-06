namespace Upstart.Api.Models;

public record CreatePollApiRequest(
    int UserId,
    string Question,
    bool IsActive = true,
    bool IsMultipleChoice = false,
    DateTime? ExpiresAt = null
);
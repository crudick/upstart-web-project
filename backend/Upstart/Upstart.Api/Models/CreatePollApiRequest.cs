namespace Upstart.Api.Models;

public record CreatePollApiRequest(
    string Question,
    bool IsActive = true,
    bool IsMultipleChoice = false,
    DateTime? ExpiresAt = null
);
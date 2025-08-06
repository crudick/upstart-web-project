namespace Upstart.Api.Models;

public record UpdatePollApiRequest(
    string Question,
    bool IsActive,
    bool IsMultipleChoice,
    DateTime? ExpiresAt
);
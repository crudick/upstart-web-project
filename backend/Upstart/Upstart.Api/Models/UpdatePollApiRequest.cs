namespace Upstart.Api.Models;

public record UpdatePollApiRequest(
    string Question,
    bool IsActive,
    bool IsMultipleChoice,
    bool RequiresAuthentication,
    DateTime? ExpiresAt
);
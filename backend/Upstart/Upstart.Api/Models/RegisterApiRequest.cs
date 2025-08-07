namespace Upstart.Api.Models;

public record RegisterApiRequest(
    string Email,
    string Password,
    string? SessionId = null
);
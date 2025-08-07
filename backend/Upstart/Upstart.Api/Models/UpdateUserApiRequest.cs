namespace Upstart.Api.Models;

public record UpdateUserApiRequest(
    string? FirstName,
    string? LastName
);
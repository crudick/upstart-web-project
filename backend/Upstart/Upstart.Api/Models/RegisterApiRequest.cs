namespace Upstart.Api.Models;

public record RegisterApiRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
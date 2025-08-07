namespace Upstart.Api.Models;

public record CreateUserApiRequest(
    string? FirstName,
    string? LastName,
    string Email,
    string? PhoneNumber
);
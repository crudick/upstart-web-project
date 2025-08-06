namespace Upstart.Api.Models;

public record LoginApiRequest(
    string Email,
    string Password
);
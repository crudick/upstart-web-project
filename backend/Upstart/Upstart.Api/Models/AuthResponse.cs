using Upstart.Domain.Models;

namespace Upstart.Api.Models;

public record AuthResponse(
    string Token,
    UserModel User
);
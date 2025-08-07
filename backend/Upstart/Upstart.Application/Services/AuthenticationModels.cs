using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public record RegisterRequest(
    string Email,
    string Password,
    string? SessionId = null
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResult(
    bool Success,
    string? Token,
    UserModel? User,
    DateTime? ExpiresAt,
    string? ErrorMessage
);
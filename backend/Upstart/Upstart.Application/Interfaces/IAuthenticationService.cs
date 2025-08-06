using Upstart.Application.Services;

namespace Upstart.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    string GenerateJwtToken(string email, int userId, string firstName, string lastName);
}
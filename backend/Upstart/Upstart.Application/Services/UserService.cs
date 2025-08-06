using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersRepository usersRepository, ILogger<UserService> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<UserModel> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating {user}", request);

        // Check if user with this email already exists
        var existingUser = await _usersRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email '{request.Email}' already exists.");
        }

        var user = new UserModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _usersRepository.CreateAsync(user);
    }
}

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber
);
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
            FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? null : request.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim(),
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _usersRepository.CreateAsync(user);
    }

    public async Task<UserModel> UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating user {userId} with data {request}", userId, request);

        var existingUser = await _usersRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }

        // Update only the fields that are provided
        if (request.FirstName != null)
        {
            existingUser.FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? null : request.FirstName.Trim();
        }

        if (request.LastName != null)
        {
            existingUser.LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim();
        }

        existingUser.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        return await _usersRepository.UpdateAsync(existingUser);
    }

    public async Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting user by ID: {userId}", id);
        return await _usersRepository.GetByIdAsync(id);
    }
}

public record CreateUserRequest(
    string? FirstName,
    string? LastName,
    string Email,
    string? PhoneNumber
);

public record UpdateUserRequest(
    string? FirstName,
    string? LastName
);
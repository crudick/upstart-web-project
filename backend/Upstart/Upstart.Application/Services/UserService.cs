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
            DateOfBirth = request.DateOfBirth.HasValue ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) : null,
            SocialSecurityNumber = request.SocialSecurityNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            AnnualIncome = request.AnnualIncome,
            EmploymentStatus = request.EmploymentStatus,
            CreditScore = request.CreditScore,
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
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? SocialSecurityNumber,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? ZipCode,
    decimal? AnnualIncome,
    string? EmploymentStatus,
    int? CreditScore
);
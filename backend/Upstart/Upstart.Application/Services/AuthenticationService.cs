using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using BCrypt.Net;

namespace Upstart.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUsersRepository usersRepository, ILogger<AuthenticationService> logger, IConfiguration configuration)
    {
        _usersRepository = usersRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Registering user with email: {Email}", request.Email);

        try
        {
            // Check if user already exists
            var existingUser = await _usersRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    User: null,
                    ExpiresAt: null,
                    ErrorMessage: "User with this email already exists"
                );
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new user
            var newUser = new UserModel
            {
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = null,
                LastName = null,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };

            var createdUser = await _usersRepository.CreateAsync(newUser);

            // Generate JWT token
            var token = GenerateJwtToken(createdUser.Email, createdUser.Id, createdUser.FirstName, createdUser.LastName);
            var expiresAt = DateTime.UtcNow.AddDays(7); // Token expires in 7 days

            _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

            return new AuthResult(
                Success: true,
                Token: token,
                User: createdUser,
                ExpiresAt: expiresAt,
                ErrorMessage: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
            return new AuthResult(
                Success: false,
                Token: null,
                User: null,
                ExpiresAt: null,
                ErrorMessage: "An error occurred during registration"
            );
        }
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Attempting login for email: {Email}", request.Email);

        try
        {
            // Get user by email
            var user = await _usersRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
            if (user == null)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    User: null,
                    ExpiresAt: null,
                    ErrorMessage: "Invalid email or password"
                );
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password attempt for email: {Email}", request.Email);
                return new AuthResult(
                    Success: false,
                    Token: null,
                    User: null,
                    ExpiresAt: null,
                    ErrorMessage: "Invalid email or password"
                );
            }

            // Generate JWT token
            var token = GenerateJwtToken(user.Email, user.Id, user.FirstName, user.LastName);
            var expiresAt = DateTime.UtcNow.AddDays(7); // Token expires in 7 days

            _logger.LogInformation("User logged in successfully with ID: {UserId}", user.Id);

            return new AuthResult(
                Success: true,
                Token: token,
                User: user,
                ExpiresAt: expiresAt,
                ErrorMessage: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new AuthResult(
                Success: false,
                Token: null,
                User: null,
                ExpiresAt: null,
                ErrorMessage: "An error occurred during login"
            );
        }
    }

    public string GenerateJwtToken(string email, int userId, string? firstName, string? lastName)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured");
        }

        var key = Encoding.ASCII.GetBytes(secretKey);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("userId", userId.ToString()) // Custom claim for easier access
        };

        if (!string.IsNullOrEmpty(firstName))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, firstName));
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            claims.Add(new Claim(ClaimTypes.Surname, lastName));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
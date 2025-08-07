using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Upstart.Api.Models;
using Upstart.Domain.Models;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;

namespace Upstart.IntegrationTests;

/// <summary>
/// Integration Tests for Auth Register API Endpoint
/// Tests the /api/auth/register endpoint for user registration functionality
/// </summary>
public class AuthRegisterIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public AuthRegisterIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task PostAuthRegister_WithValidRequest_ShouldReturn201AndAuthResponse()
    {
        // Arrange
        ClearDatabase();
        
        var validRequest = new RegisterApiRequest(
            Email: "newuser@example.com",
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", validRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeEmpty();
        
        // Verify response structure
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(validRequest.Email.ToLowerInvariant());
        authResponse.User.FirstName.Should().BeNull();
        authResponse.User.LastName.Should().BeNull();
    }

    [Fact]
    public async Task PostAuthRegister_WithValidRequest_ShouldCreateUserInDatabase()
    {
        // Arrange
        ClearDatabase();
        
        var uniqueEmail = $"dbtest{Guid.NewGuid()}@example.com";
        var validRequest = new RegisterApiRequest(
            Email: uniqueEmail,
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", validRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify user was created in database
        using var context = _factory.GetDbContext();
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == uniqueEmail.ToLowerInvariant());
        
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be(uniqueEmail.ToLowerInvariant());
        createdUser.FirstName.Should().BeNull();
        createdUser.LastName.Should().BeNull();
        createdUser.PasswordHash.Should().NotBeNullOrEmpty();
        createdUser.PasswordHash.Should().NotBe(validRequest.Password); // Should be hashed
        createdUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        createdUser.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PostAuthRegister_WithValidRequest_ShouldReturnValidJwtToken()
    {
        // Arrange
        ClearDatabase();
        
        var validRequest = new RegisterApiRequest(
            Email: "jwttest@example.com",
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", validRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        
        // JWT token should have 3 parts separated by dots
        var tokenParts = authResponse!.Token.Split('.');
        tokenParts.Should().HaveCount(3);
        
        // Each part should be base64-like string (not empty)
        tokenParts.Should().OnlyContain(part => !string.IsNullOrWhiteSpace(part));
    }

    [Theory]
    [InlineData("", "SecurePass123", "Email is required")]
    [InlineData("   ", "SecurePass123", "Email is required")]
    [InlineData(null, "SecurePass123", "Email is required")]
    public async Task PostAuthRegister_WithInvalidEmail_ShouldReturn400(string invalidEmail, string password, string expectedErrorMessage)
    {
        // Arrange
        ClearDatabase();
        
        var invalidRequest = new RegisterApiRequest(
            Email: invalidEmail!,
            Password: password
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf(expectedErrorMessage);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public async Task PostAuthRegister_WithInvalidEmailFormat_ShouldReturn400(string invalidEmail)
    {
        // Arrange
        ClearDatabase();
        
        var invalidRequest = new RegisterApiRequest(
            Email: invalidEmail,
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("email");
    }

    [Theory]
    [InlineData("", "Password is required")]
    [InlineData("   ", "Password is required")]
    [InlineData(null, "Password is required")]
    [InlineData("short", "Password must be at least 8 characters long")]
    [InlineData("1234567", "Password must be at least 8 characters long")]
    [InlineData("lowercase", "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    [InlineData("UPPERCASE", "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    [InlineData("NoNumbers", "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    [InlineData("nonumbers123", "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    [InlineData("NOLOWERCASE123", "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
    public async Task PostAuthRegister_WithInvalidPassword_ShouldReturn400(string invalidPassword, string expectedErrorMessage)
    {
        // Arrange
        ClearDatabase();
        
        var invalidRequest = new RegisterApiRequest(
            Email: "test@example.com",
            Password: invalidPassword!
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf(expectedErrorMessage);
    }

    [Fact]
    public async Task PostAuthRegister_WithDuplicateEmail_ShouldReturn400()
    {
        // Arrange
        ClearDatabase();
        
        var email = "duplicate@example.com";
        var firstRequest = new RegisterApiRequest(
            Email: email,
            Password: "FirstPass123"
        );
        
        // Register first user
        var firstResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", firstRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Try to register with same email
        var duplicateRequest = new RegisterApiRequest(
            Email: email,
            Password: "SecondPass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("already exists");
    }

    [Fact]
    public async Task PostAuthRegister_WithDuplicateEmailDifferentCase_ShouldReturn400()
    {
        // Arrange
        ClearDatabase();
        
        var firstRequest = new RegisterApiRequest(
            Email: "CaseTest@Example.Com",
            Password: "FirstPass123"
        );
        
        // Register first user
        var firstResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", firstRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Try to register with same email but different case
        var duplicateRequest = new RegisterApiRequest(
            Email: "casetest@example.com",
            Password: "SecondPass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("already exists");
    }

    [Fact]
    public async Task PostAuthRegister_WithEmailAtMaxLength_ShouldSucceed()
    {
        // Arrange
        ClearDatabase();
        
        // Create email at maximum length (255 characters)  
        var longEmail = new string('a', 243) + "@example.com"; // 243 + 12 = 255 chars
        longEmail.Length.Should().Be(255);
        
        var validRequest = new RegisterApiRequest(
            Email: longEmail,
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", validRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostAuthRegister_WithEmailTooLong_ShouldReturn400()
    {
        // Arrange
        ClearDatabase();
        
        // Create email longer than maximum length (> 255 characters)
        var tooLongEmail = new string('a', 250) + "@example.com"; // 250 + 12 + 3 = 265 chars
        tooLongEmail.Length.Should().BeGreaterThan(255);
        
        var invalidRequest = new RegisterApiRequest(
            Email: tooLongEmail,
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("cannot exceed 255 characters");
    }

    [Fact]
    public async Task PostAuthRegister_WithValidPasswordVariations_ShouldSucceed()
    {
        // Arrange & Act & Assert - Test different valid password patterns
        var validPasswords = new[]
        {
            "SimplePass1",
            "Complex123Password",
            "MySecure2023Pass",
            "Test1234Password",
            "Abcd1234efgh"
        };

        ClearDatabase();

        foreach (var (password, index) in validPasswords.Select((p, i) => (p, i)))
        {
            var request = new RegisterApiRequest(
                Email: $"testuser{index}@example.com",
                Password: password
            );

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            
            response.StatusCode.Should().Be(HttpStatusCode.Created, 
                $"Password '{password}' should be valid but got {response.StatusCode}");
        }
    }

    [Fact]
    public async Task PostAuthRegister_ShouldNormalizeEmailToLowerCase()
    {
        // Arrange
        ClearDatabase();
        
        var mixedCaseEmail = "MixedCase@Example.COM";
        var request = new RegisterApiRequest(
            Email: mixedCaseEmail,
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.User.Email.Should().Be(mixedCaseEmail.ToLowerInvariant());
        
        // Verify in database
        using var context = _factory.GetDbContext();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == mixedCaseEmail.ToLowerInvariant());
        user.Should().NotBeNull();
        user!.Email.Should().Be(mixedCaseEmail.ToLowerInvariant());
    }

    [Fact]
    public async Task PostAuthRegister_ShouldReturnLocationHeader()
    {
        // Arrange
        ClearDatabase();
        
        var request = new RegisterApiRequest(
            Email: "locationtest@example.com",
            Password: "SecurePass123"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/users/{authResponse!.User.Id}");
    }

    [Fact]
    public async Task PostAuthRegister_ConcurrentRequests_ShouldHandleGracefully()
    {
        // Arrange
        ClearDatabase();
        
        var requests = Enumerable.Range(1, 5).Select(i => new RegisterApiRequest(
            Email: $"concurrent{i}@example.com",
            Password: "SecurePass123"
        )).ToArray();

        // Act
        var tasks = requests.Select(request => 
            _httpClient.PostAsJsonAsync("/api/auth/register", request)
        ).ToArray();
        
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().OnlyContain(response => response.StatusCode == HttpStatusCode.Created);
        
        // Verify all users were created
        using var context = _factory.GetDbContext();
        var userCount = await context.Users.CountAsync();
        userCount.Should().Be(5);
    }

    [Fact]
    public async Task PostAuthRegister_WithSessionCookie_ShouldMigratePollsToUser()
    {
        // Arrange
        ClearDatabase();
        
        var sessionId = Guid.NewGuid().ToString();
        
        // First create a poll with session ID (simulate unauthenticated poll creation)
        var pollRequest = new CreatePollApiRequest(
            Question: "Test poll for migration",
            IsMultipleChoice: false,
            ExpiresAt: null
        );
        
        var pollClient = _factory.CreateClient();
        pollClient.DefaultRequestHeaders.Add("Cookie", $"upstart_session={sessionId}");
        
        var pollResponse = await pollClient.PostAsJsonAsync("/api/polls", pollRequest);
        pollResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPoll = await pollResponse.Content.ReadFromJsonAsync<PollModel>();
        createdPoll.Should().NotBeNull();
        
        // Verify poll has session ID
        using (var contextForCheck = _factory.GetDbContext())
        {
            var pollEntity = await contextForCheck.Polls.FirstOrDefaultAsync(p => p.Id == createdPoll!.Id);
            pollEntity.Should().NotBeNull();
            pollEntity!.SessionId.Should().Be(sessionId);
            pollEntity.UserId.Should().BeNull();
        }
        
        // Now register a user with the same session ID
        var registerRequest = new RegisterApiRequest(
            Email: "migration@example.com",
            Password: "SecurePass123"
        );
        
        var registerClient = _factory.CreateClient();
        registerClient.DefaultRequestHeaders.Add("Cookie", $"upstart_session={sessionId}");
        
        // Act
        var registerResponse = await registerClient.PostAsJsonAsync("/api/auth/register", registerRequest);
        
        // Assert
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        
        // Verify poll was migrated to the user
        using var context = _factory.GetDbContext();
        var migratedPoll = await context.Polls.FirstOrDefaultAsync(p => p.Id == createdPoll!.Id);
        migratedPoll.Should().NotBeNull();
        migratedPoll!.UserId.Should().Be(authResponse!.User.Id);
        migratedPoll.SessionId.Should().BeNull(); // Should be cleared after migration
    }

    private void ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
        context.Users.RemoveRange(context.Users);
        context.Polls.RemoveRange(context.Polls);
        context.PollAnswers.RemoveRange(context.PollAnswers);
        context.PollStats.RemoveRange(context.PollStats);
        context.SaveChanges();
    }
}
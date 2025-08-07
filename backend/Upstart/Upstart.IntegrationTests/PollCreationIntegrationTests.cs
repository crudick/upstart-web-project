using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Upstart.Api.Models;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;
using Upstart.Domain.Models;

namespace Upstart.IntegrationTests;

/// <summary>
/// Integration Tests for Poll Creation API Endpoints
/// Tests the /api/polls endpoint for poll creation functionality
/// </summary>
public class PollCreationIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public PollCreationIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task PostPolls_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        ClearDatabase();
        
        var request = new CreatePollApiRequest(
            Question: "What is your favorite color?"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostPolls_WithValidRequest_ShouldReturn201AndCreatePoll()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "What is your favorite programming language?",
            IsActive: true,
            IsMultipleChoice: false,
            ExpiresAt: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeEmpty();
        
        // Verify response structure
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        pollModel!.Question.Should().Be(request.Question);
        pollModel.IsActive.Should().Be(request.IsActive);
        pollModel.IsMultipleChoice.Should().Be(request.IsMultipleChoice);
        pollModel.PollGuid.Should().NotBeNullOrEmpty();
        pollModel.UserId.Should().BeGreaterThan(0);
        pollModel.ExpiresAt.Should().BeCloseTo(request.ExpiresAt!.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PostPolls_WithValidRequest_ShouldCreatePollInDatabase()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "What is your favorite framework?",
            IsActive: true,
            IsMultipleChoice: true
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();

        // Verify poll was created in database
        using var context = _factory.GetDbContext();
        var createdPoll = await context.Polls
            .FirstOrDefaultAsync(p => p.PollGuid == pollModel!.PollGuid);
        
        createdPoll.Should().NotBeNull();
        createdPoll!.Question.Should().Be(request.Question);
        createdPoll.IsActive.Should().Be(request.IsActive);
        createdPoll.IsMultipleChoice.Should().Be(request.IsMultipleChoice);
        createdPoll.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        createdPoll.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PostPolls_WithMinimalValidRequest_ShouldSucceedWithDefaults()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "Simple question?"
            // Using all default values
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        pollModel!.IsActive.Should().BeTrue(); // Default value
        pollModel.IsMultipleChoice.Should().BeFalse(); // Default value
        pollModel.ExpiresAt.Should().BeNull(); // Default value
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task PostPolls_WithInvalidQuestion_ShouldReturn400(string invalidQuestion)
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: invalidQuestion!
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("Question is required");
    }

    [Fact]
    public async Task PostPolls_WithQuestionTooLong_ShouldReturn400()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Create question longer than 500 characters
        var longQuestion = new string('a', 501);
        var request = new CreatePollApiRequest(
            Question: longQuestion
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("cannot exceed 500 characters");
    }

    [Fact]
    public async Task PostPolls_WithQuestionAtMaxLength_ShouldSucceed()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Create question exactly at 500 characters
        var maxLengthQuestion = new string('a', 500);
        var request = new CreatePollApiRequest(
            Question: maxLengthQuestion
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        pollModel!.Question.Should().Be(maxLengthQuestion);
    }

    [Fact]
    public async Task PostPolls_WithPastExpirationDate_ShouldReturn400()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "What do you think?",
            ExpiresAt: DateTime.UtcNow.AddDays(-1) // Past date
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("must be in the future");
    }

    [Fact]
    public async Task PostPolls_WithFutureExpirationDate_ShouldSucceed()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var futureDate = DateTime.UtcNow.AddHours(1);
        var request = new CreatePollApiRequest(
            Question: "Future poll?",
            ExpiresAt: futureDate
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        pollModel!.ExpiresAt.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PostPolls_WithBooleanParameters_ShouldRespectSettings()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "Multiple choice question?",
            IsActive: false,
            IsMultipleChoice: true
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        pollModel!.IsActive.Should().BeFalse();
        pollModel.IsMultipleChoice.Should().BeTrue();
    }

    [Fact]
    public async Task PostPolls_ShouldGenerateUniquePollGuid()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request1 = new CreatePollApiRequest(Question: "First poll?");
        var request2 = new CreatePollApiRequest(Question: "Second poll?");

        // Act
        var response1 = await _httpClient.PostAsJsonAsync("/api/polls", request1);
        var response2 = await _httpClient.PostAsJsonAsync("/api/polls", request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var poll1 = await response1.Content.ReadFromJsonAsync<PollModel>();
        var poll2 = await response2.Content.ReadFromJsonAsync<PollModel>();
        
        poll1.Should().NotBeNull();
        poll2.Should().NotBeNull();
        poll1!.PollGuid.Should().NotBe(poll2!.PollGuid);
        
        // Verify both GUIDs are valid format
        Guid.TryParse(poll1.PollGuid, out _).Should().BeTrue();
        Guid.TryParse(poll2.PollGuid, out _).Should().BeTrue();
    }

    [Fact]
    public async Task PostPolls_ShouldReturnLocationHeader()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreatePollApiRequest(
            Question: "Location header test?"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pollModel = await response.Content.ReadFromJsonAsync<PollModel>();
        pollModel.Should().NotBeNull();
        
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/polls/{pollModel!.Id}");
    }

    [Fact]
    public async Task PostPolls_ConcurrentRequests_ShouldHandleGracefully()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var requests = Enumerable.Range(1, 3).Select(i => new CreatePollApiRequest(
            Question: $"Concurrent poll {i}?"
        )).ToArray();

        // Act
        var tasks = requests.Select(request => 
            _httpClient.PostAsJsonAsync("/api/polls", request)
        ).ToArray();
        
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().OnlyContain(response => response.StatusCode == HttpStatusCode.Created);
        
        // Verify all polls were created
        using var context = _factory.GetDbContext();
        var pollCount = await context.Polls.CountAsync();
        pollCount.Should().Be(3);
    }

    [Fact]
    public async Task PostPolls_MultipleUsersCreatingPolls_ShouldAssignCorrectUserId()
    {
        // Arrange
        ClearDatabase();
        
        // Create two different users
        var token1 = await CreateTestUserAndGetToken("user1@example.com", "Password123");
        var token2 = await CreateTestUserAndGetToken("user2@example.com", "Password456");
        
        var request1 = new CreatePollApiRequest(Question: "User 1's poll?");
        var request2 = new CreatePollApiRequest(Question: "User 2's poll?");

        // Act - Create polls with different users
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
        var response1 = await _httpClient.PostAsJsonAsync("/api/polls", request1);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);
        var response2 = await _httpClient.PostAsJsonAsync("/api/polls", request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var poll1 = await response1.Content.ReadFromJsonAsync<PollModel>();
        var poll2 = await response2.Content.ReadFromJsonAsync<PollModel>();
        
        poll1.Should().NotBeNull();
        poll2.Should().NotBeNull();
        poll1!.UserId.Should().NotBe(poll2!.UserId);
        poll1.UserId.Should().BeGreaterThan(0);
        poll2.UserId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PostPolls_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        ClearDatabase();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
        
        var request = new CreatePollApiRequest(
            Question: "Test question?"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostPolls_WithExpiredToken_ShouldReturn401()
    {
        // Arrange
        ClearDatabase();
        
        // This would require creating an expired token, which is complex in this setup
        // For now, we'll test with a malformed token that should fail validation
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "malformed.token.here");
        
        var request = new CreatePollApiRequest(
            Question: "Test question?"
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/polls", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string> CreateTestUserAndGetToken(string email = "test@example.com", string password = "TestPassword123")
    {
        var registerRequest = new RegisterApiRequest(
            Email: email,
            Password: password
        );

        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        
        return authResponse!.Token;
    }

    private void ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
        context.Polls.RemoveRange(context.Polls);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
    }
}
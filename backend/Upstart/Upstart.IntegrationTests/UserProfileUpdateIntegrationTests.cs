using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Upstart.Api.Models;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;

namespace Upstart.IntegrationTests;

/// <summary>
/// Integration Tests for User Profile Update API Endpoint
/// Tests the PUT /api/users/me endpoint for user profile update functionality
/// </summary>
public class UserProfileUpdateIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public UserProfileUpdateIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task PutUsersMe_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        ClearDatabase();
        
        var request = new UpdateUserApiRequest(
            FirstName: "John",
            LastName: "Doe"
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutUsersMe_WithValidRequest_ShouldReturn200AndUpdateUser()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateUserApiRequest(
            FirstName: "Jane",
            LastName: "Smith"
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeEmpty();
        
        // Verify response structure
        var userModel = await response.Content.ReadFromJsonAsync<Upstart.Domain.Models.UserModel>();
        userModel.Should().NotBeNull();
        userModel!.FirstName.Should().Be(request.FirstName);
        userModel.LastName.Should().Be(request.LastName);
    }

    [Fact]
    public async Task PutUsersMe_WithValidRequest_ShouldUpdateUserInDatabase()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateUserApiRequest(
            FirstName: "UpdatedFirst",
            LastName: "UpdatedLast"
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userModel = await response.Content.ReadFromJsonAsync<Upstart.Domain.Models.UserModel>();
        userModel.Should().NotBeNull();

        // Verify user was updated in database
        using var context = _factory.GetDbContext();
        var updatedUser = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userModel!.Id);
        
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(request.FirstName);
        updatedUser.LastName.Should().Be(request.LastName);
        updatedUser.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PutUsersMe_WithNullValues_ShouldAllowNullableFields()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateUserApiRequest(
            FirstName: null,
            LastName: null
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userModel = await response.Content.ReadFromJsonAsync<Upstart.Domain.Models.UserModel>();
        userModel.Should().NotBeNull();
        userModel!.FirstName.Should().BeNull();
        userModel.LastName.Should().BeNull();
    }

    [Fact]
    public async Task PutUsersMe_WithEmptyStrings_ShouldSetToNull()
    {
        // Arrange
        ClearDatabase();
        var token = await CreateTestUserAndGetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateUserApiRequest(
            FirstName: "",
            LastName: "   "
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userModel = await response.Content.ReadFromJsonAsync<Upstart.Domain.Models.UserModel>();
        userModel.Should().NotBeNull();
        userModel!.FirstName.Should().BeNull();
        userModel.LastName.Should().BeNull();
    }

    [Fact]
    public async Task PutUsersMe_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        ClearDatabase();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
        
        var request = new UpdateUserApiRequest(
            FirstName: "Test",
            LastName: "User"
        );

        // Act
        var response = await _httpClient.PutAsJsonAsync("/api/users/me", request);

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
        context.PollAnswers.RemoveRange(context.PollAnswers);
        context.Polls.RemoveRange(context.Polls);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Upstart.Api.Models;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;

namespace Upstart.IntegrationTests;

/// <summary>
/// Simple Integration Tests for User Creation API
/// Demonstrates TDD approach with WebApplicationFactory and reusable steps
/// </summary>
public class UserIntegrationTests_Simple : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public UserIntegrationTests_Simple(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task PostUsers_WithValidRequest_ShouldReturn201()
    {
        // Arrange - TDD Red Phase: Create a failing test first
        var validUserRequest = new CreateUserApiRequest(
            FirstName: "John",
            LastName: "Doe", 
            Email: "john.doe@example.com",
            PhoneNumber: null,
            DateOfBirth: null,
            SocialSecurityNumber: null,
            AddressLine1: null,
            AddressLine2: null,
            City: null,
            State: null,
            ZipCode: null,
            AnnualIncome: null,
            EmploymentStatus: null,
            CreditScore: null
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/users", validUserRequest);

        // Assert - TDD Green Phase: Verify the test passes with implementation
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("john.doe@example.com");
    }

    [Fact]
    public async Task PostUsers_WithMissingRequiredFields_ShouldReturn400()
    {
        // Arrange - Test validation behavior
        var invalidRequest = new CreateUserApiRequest(
            FirstName: "", // Invalid - required field
            LastName: "", // Invalid - required field  
            Email: "", // Invalid - required field
            PhoneNumber: null,
            DateOfBirth: null,
            SocialSecurityNumber: null,
            AddressLine1: null,
            AddressLine2: null,
            City: null,
            State: null,
            ZipCode: null,
            AnnualIncome: null,
            EmploymentStatus: null,
            CreditScore: null
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/users", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostUsers_WithInvalidEmail_ShouldReturn400()
    {
        // Arrange
        var invalidEmailRequest = new CreateUserApiRequest(
            FirstName: "John",
            LastName: "Doe",
            Email: "invalid-email-format", // Invalid email format
            PhoneNumber: null,
            DateOfBirth: null,
            SocialSecurityNumber: null,
            AddressLine1: null,
            AddressLine2: null,
            City: null,
            State: null,
            ZipCode: null,
            AnnualIncome: null,
            EmploymentStatus: null,
            CreditScore: null
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/users", invalidEmailRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf("email");
    }

    [Fact]
    public async Task PostUsers_WithValidRequest_ShouldCreateUserInDatabase()
    {
        // Arrange
        var uniqueEmail = $"jane.smith.{Guid.NewGuid()}@example.com";
        var userRequest = new CreateUserApiRequest(
            FirstName: "Jane",
            LastName: "Smith",
            Email: uniqueEmail,
            PhoneNumber: "555-1234",
            DateOfBirth: DateTime.Now.AddYears(-25),
            SocialSecurityNumber: "123-45-6789",
            AddressLine1: "123 Main St",
            AddressLine2: null,
            City: "Springfield",
            State: "CA",
            ZipCode: "12345",
            AnnualIncome: 50000m,
            EmploymentStatus: "Full-time",
            CreditScore: 700
        );

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/users", userRequest);

        // Assert - Verify API response
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(uniqueEmail);

        // Assert - Verify user was created in database using factory's GetDbContext method
        // Wait a moment to ensure async operations complete
        await Task.Delay(100);
        
        using var context = _factory.GetDbContext();
        
        // Force Entity Framework to reload data from database
        context.ChangeTracker.Clear();
        
        // Debug: Check total user count
        var totalUsers = await context.Users.CountAsync();
        var allUsers = await context.Users.ToListAsync();
        
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == uniqueEmail);
        createdUser.Should().NotBeNull($"User should be created in database. Total users: {totalUsers}. All emails: {string.Join(", ", allUsers.Select(u => u.Email))}");
        createdUser!.FirstName.Should().Be(userRequest.FirstName);
        createdUser!.LastName.Should().Be(userRequest.LastName);
        createdUser!.Email.Should().Be(userRequest.Email);
    }
}
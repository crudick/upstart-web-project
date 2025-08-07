using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Upstart.Api.Models;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.IntegrationTests.Steps;

public class TestSteps
{
    private readonly HttpClient _httpClient;

    private readonly TestWebApplicationFactory<Program> _factory;
    private HttpResponseMessage? _lastResponse;
    private readonly JsonSerializerOptions _jsonOptions;

    public TestSteps(HttpClient httpClient, TestWebApplicationFactory<Program> factory)
    {
        _httpClient = httpClient;
        _factory = factory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    // GIVEN steps
    public TestSteps GivenNoUsersExist()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
        return this;
    }

    public TestSteps GivenUserExists(string firstName, string lastName, string email)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();

        var user = new UserEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.SaveChanges();
        return this;
    }

    public TestSteps GivenValidUserRequest(out CreateUserApiRequest request)
    {
        request = new CreateUserApiRequest(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            PhoneNumber: "555-1234"
        );
        return this;
    }

    public TestSteps GivenInvalidUserRequest(out CreateUserApiRequest request, string validationError = "missing required fields")
    {
        request = validationError switch
        {
            "missing required fields" => new CreateUserApiRequest(
                FirstName: "", // Optional field - allowed to be empty
                LastName: "", // Optional field - allowed to be empty
                Email: "", // Required field - this will cause validation error
                PhoneNumber: null
            ),
            "invalid email" => new CreateUserApiRequest(
                FirstName: "John",
                LastName: "Doe",
                Email: "invalid-email",
                PhoneNumber: null
            ),
            "duplicate email" => new CreateUserApiRequest(
                FirstName: "Jane",
                LastName: "Smith",
                Email: "john.doe@example.com",
                PhoneNumber: null
            ),
            _ => throw new ArgumentException($"Unknown validation error type: {validationError}")
        };
        return this;
    }

    // WHEN steps
    public async Task<TestSteps> WhenIPostToUsersEndpoint(CreateUserApiRequest request)
    {
        _lastResponse = await _httpClient.PostAsJsonAsync("/api/users", request);
        return this;
    }

    public async Task<TestSteps> WhenIGetAllUsers()
    {
        _lastResponse = await _httpClient.GetAsync("/api/users");
        return this;
    }

    public async Task<TestSteps> WhenIGetUserById(int userId)
    {
        _lastResponse = await _httpClient.GetAsync($"/api/users/{userId}");
        return this;
    }

    // THEN steps
    public TestSteps ThenTheResponseShouldBeSuccessful()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeTrue(
            $"Expected success status code, but got {_lastResponse.StatusCode}. Response: {_lastResponse.Content.ReadAsStringAsync().Result}");
        return this;
    }

    public TestSteps ThenTheResponseShouldHaveStatusCode(System.Net.HttpStatusCode expectedStatusCode)
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.StatusCode.Should().Be(expectedStatusCode);
        return this;
    }

    public async Task<TestSteps> ThenTheResponseShouldContainUserWithEmail(string email)
    {
        _lastResponse.Should().NotBeNull();
        var responseContent = await _lastResponse!.Content.ReadAsStringAsync();
        responseContent.Should().Contain(email);
        return this;
    }

    public async Task<TestSteps> ThenTheUserShouldBeCreatedInDatabase(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        user.Should().NotBeNull($"User with email {email} should exist in database");
        return this;
    }

    public async Task<TestSteps> ThenTheResponseShouldContainValidationError(string expectedError)
    {
        _lastResponse.Should().NotBeNull();
        var responseContent = await _lastResponse!.Content.ReadAsStringAsync();
        responseContent.Should().ContainEquivalentOf(expectedError);
        return this;
    }

    public async Task<T> GetResponseAs<T>()
    {
        _lastResponse.Should().NotBeNull();
        var responseContent = await _lastResponse!.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseContent)!;
    }

    public async Task<string> GetResponseContent()
    {
        _lastResponse.Should().NotBeNull();
        return await _lastResponse!.Content.ReadAsStringAsync();
    }

    // Helper method to get database context for custom assertions
    public UpstartDbContext GetDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
    }
}

// Extension method to add missing async method
public static class DbSetExtensions
{
    public static async Task<T?> FirstOrDefaultAsync<T>(this Microsoft.EntityFrameworkCore.DbSet<T> dbSet, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(dbSet, predicate);
    }
}
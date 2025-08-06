using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Upstart.Api.Models;
using Upstart.IntegrationTests.Steps;
using Upstart.IntegrationTests.TestSetup;
using Upstart.Persistence.Data;

namespace Upstart.IntegrationTests;

public class UserIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private readonly TestSteps _steps;

    public UserIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
        _steps = new TestSteps(_httpClient, _factory);
    }

    [Fact]
    public async Task PostUsers_WithValidRequest_ShouldCreateUserAndReturn201()
    {
        // Arrange - TDD Red Phase: Write failing test first
        _steps.GivenNoUsersExist()
               .GivenValidUserRequest(out var validRequest);

        // Act
        await _steps.WhenIPostToUsersEndpoint(validRequest);

        // Assert - TDD Green Phase: Verify expected behavior
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.Created);
        await _steps.ThenTheResponseShouldContainUserWithEmail(validRequest.Email);
        await _steps.ThenTheUserShouldBeCreatedInDatabase(validRequest.Email);
    }

    [Fact]
    public async Task PostUsers_WithValidRequest_ShouldReturnCorrectUserData()
    {
        // Arrange
        _steps.GivenNoUsersExist()
               .GivenValidUserRequest(out var validRequest);

        // Act
        await _steps.WhenIPostToUsersEndpoint(validRequest);

        // Assert - Verify response structure and content
        var responseContent = await _steps.GetResponseContent();
        responseContent.Should().Contain(validRequest.FirstName);
        responseContent.Should().Contain(validRequest.LastName);
        responseContent.Should().Contain(validRequest.Email);

        // Verify in database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
        var createdUser = context.Users.First(u => u.Email == validRequest.Email);

        createdUser.FirstName.Should().Be(validRequest.FirstName);
        createdUser.LastName.Should().Be(validRequest.LastName);
        createdUser.Email.Should().Be(validRequest.Email);
        createdUser.PhoneNumber.Should().Be(validRequest.PhoneNumber);
        createdUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        createdUser.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task PostUsers_WithMissingRequiredFields_ShouldReturn400()
    {
        // Arrange - Test validation
        _steps.GivenNoUsersExist()
               .GivenInvalidUserRequest(out var invalidRequest, "missing required fields");

        // Act
        await _steps.WhenIPostToUsersEndpoint(invalidRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
        await _steps.ThenTheResponseShouldContainValidationError("FirstName");
        await _steps.ThenTheResponseShouldContainValidationError("LastName");
        await _steps.ThenTheResponseShouldContainValidationError("Email");
    }

    [Fact]
    public async Task PostUsers_WithInvalidEmail_ShouldReturn400()
    {
        // Arrange
        _steps.GivenNoUsersExist()
               .GivenInvalidUserRequest(out var invalidRequest, "invalid email");

        // Act
        await _steps.WhenIPostToUsersEndpoint(invalidRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
        await _steps.ThenTheResponseShouldContainValidationError("Email");
    }

    [Fact]
    public async Task PostUsers_WithDuplicateEmail_ShouldReturn400()
    {
        // Arrange - First create a user
        _steps.GivenNoUsersExist()
               .GivenUserExists("John", "Doe", "john.doe@example.com");

        _steps.GivenInvalidUserRequest(out var duplicateRequest, "duplicate email");

        // Act
        await _steps.WhenIPostToUsersEndpoint(duplicateRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostUsers_WithOnlyRequiredFields_ShouldCreateUserAndReturn201()
    {
        // Arrange - Test minimal valid request
        _steps.GivenNoUsersExist();

        var minimalRequest = new CreateUserApiRequest(
            FirstName: "Jane",
            LastName: "Smith",
            Email: "jane.smith@example.com",
            PhoneNumber: null
        );

        // Act
        await _steps.WhenIPostToUsersEndpoint(minimalRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.Created);
        await _steps.ThenTheResponseShouldContainUserWithEmail(minimalRequest.Email);
        await _steps.ThenTheUserShouldBeCreatedInDatabase(minimalRequest.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task PostUsers_WithInvalidFirstName_ShouldReturn400(string invalidFirstName)
    {
        // Arrange
        _steps.GivenNoUsersExist();

        var invalidRequest = new CreateUserApiRequest(
            FirstName: invalidFirstName,
            LastName: "Doe",
            Email: "test@example.com",
            PhoneNumber: null
        );

        // Act
        await _steps.WhenIPostToUsersEndpoint(invalidRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
        await _steps.ThenTheResponseShouldContainValidationError("FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task PostUsers_WithInvalidLastName_ShouldReturn400(string invalidLastName)
    {
        // Arrange
        _steps.GivenNoUsersExist();

        var invalidRequest = new CreateUserApiRequest(
            FirstName: "John",
            LastName: invalidLastName,
            Email: "test@example.com",
            PhoneNumber: null
        );

        // Act
        await _steps.WhenIPostToUsersEndpoint(invalidRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
        await _steps.ThenTheResponseShouldContainValidationError("LastName");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task PostUsers_WithInvalidEmailFormats_ShouldReturn400(string invalidEmail)
    {
        // Arrange
        _steps.GivenNoUsersExist();

        var invalidRequest = new CreateUserApiRequest(
            FirstName: "John",
            LastName: "Doe",
            Email: invalidEmail,
            PhoneNumber: null
        );

        // Act
        await _steps.WhenIPostToUsersEndpoint(invalidRequest);

        // Assert
        _steps.ThenTheResponseShouldHaveStatusCode(HttpStatusCode.BadRequest);
        await _steps.ThenTheResponseShouldContainValidationError("Email");
    }



    [Fact]
    public async Task PostUsers_MultipleValidRequests_ShouldCreateMultipleUsers()
    {
        // Arrange - Test concurrent user creation
        _steps.GivenNoUsersExist();

        var requests = new[]
        {
            new CreateUserApiRequest("John", "Doe", "john@example.com", null),
            new CreateUserApiRequest("Jane", "Smith", "jane@example.com", null),
            new CreateUserApiRequest("Bob", "Johnson", "bob@example.com", null)
        };

        // Act
        var tasks = requests.Select(request => _steps.WhenIPostToUsersEndpoint(request)).ToArray();
        await Task.WhenAll(tasks);

        // Assert - Verify all users were created
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();

        var userCount = context.Users.Count();
        userCount.Should().Be(3);

        foreach (var request in requests)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == request.Email);
            user.Should().NotBeNull($"User with email {request.Email} should exist");
        }
    }
}
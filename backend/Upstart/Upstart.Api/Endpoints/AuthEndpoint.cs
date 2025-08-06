using AutoMapper;
using FluentValidation;
using Upstart.Api.Models;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user account")
            .Produces<AuthResponse>(201)
            .Produces(400);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Login with email and password")
            .Produces<AuthResponse>(200)
            .Produces(400)
            .Produces(401);
    }

    private static async Task<IResult> Register(RegisterApiRequest request, IAuthenticationService authService, IMapper mapper, ILogger<IAuthenticationService> logger)
    {
        logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var serviceRequest = new RegisterRequest(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName
        );

        var result = await authService.RegisterAsync(serviceRequest);

        if (!result.Success)
        {
            logger.LogWarning("Registration failed for email: {Email}. Error: {Error}", request.Email, result.ErrorMessage);
            return Results.BadRequest(new { error = result.ErrorMessage });
        }

        var response = new AuthResponse(
            result.Token!,
            result.User!
        );

        logger.LogInformation("User registered successfully with ID: {UserId}", result.User!.Id);
        return Results.Created($"/api/users/{result.User!.Id}", response);
    }

    private static async Task<IResult> Login(LoginApiRequest request, IAuthenticationService authService, IMapper mapper, ILogger<IAuthenticationService> logger)
    {
        logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var serviceRequest = new LoginRequest(
            request.Email,
            request.Password
        );

        var result = await authService.LoginAsync(serviceRequest);

        if (!result.Success)
        {
            logger.LogWarning("Login failed for email: {Email}. Error: {Error}", request.Email, result.ErrorMessage);
            return Results.Unauthorized();
        }

        var response = new AuthResponse(
            result.Token!,
            result.User!
        );

        logger.LogInformation("User logged in successfully with ID: {UserId}", result.User!.Id);
        return Results.Ok(response);
    }
}
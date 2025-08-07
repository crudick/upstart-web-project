using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current user information")
            .RequireAuthorization()
            .Produces(200)
            .Produces(401)
            .Produces(404);
    }

    private static async Task<IResult> Register(RegisterApiRequest request, IAuthenticationService authService, IValidator<RegisterApiRequest> validator, IMapper mapper, ILogger<IAuthenticationService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        // Validate the request
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Registration validation failed for email: {Email}. Errors: {@ValidationErrors}",
                request.Email, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        // Get session ID from request body, falling back to cookies if not provided
        var sessionId = request.SessionId ?? httpContext.Request.Cookies["upstart_session"];

        var serviceRequest = new RegisterRequest(
            request.Email,
            request.Password,
            sessionId
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

    private static async Task<IResult> GetCurrentUser(
        IUserService userService,
        HttpContext context,
        ILogger<IAuthenticationService> logger)
    {
        var userIdClaim = context.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            logger.LogWarning("User ID not found in claims or invalid");
            return Results.Unauthorized();
        }

        logger.LogInformation("Getting current user for ID: {UserId}", userId);

        try
        {
            var user = await userService.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User not found for ID: {UserId}", userId);
                return Results.NotFound();
            }

            return Results.Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting current user for ID: {UserId}", userId);
            return Results.Problem("An error occurred while retrieving user information");
        }
    }
}
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Upstart.Api.Models;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class UsersEndpoint
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .Produces(201)
            .Produces(400);

        group.MapPut("/me", UpdateCurrentUser)
            .WithName("UpdateCurrentUser")
            .WithSummary("Update current user's profile")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(404);
    }

    private static async Task<IResult> CreateUser(CreateUserApiRequest request, IUserService userService, IValidator<CreateUserApiRequest> validator, IMapper mapper, ILogger<UserService> logger)
    {
        logger.LogInformation("Creating user with email: {Email}", request.Email);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("User creation validation failed for email: {Email}. Errors: {@ValidationErrors}",
                request.Email, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = mapper.Map<CreateUserRequest>(request);
        var result = await userService.CreateUserAsync(serviceRequest);

        logger.LogInformation("User created successfully with ID: {UserId} and email: {Email}",
            result.Id, result.Email);

        return Results.Created($"/api/users/{result.Id}", result);
    }

    private static async Task<IResult> UpdateCurrentUser(
        UpdateUserApiRequest request, 
        IUserService userService, 
        IValidator<UpdateUserApiRequest> validator,
        IMapper mapper,
        HttpContext context,
        ILogger<UserService> logger)
    {
        var userIdClaim = context.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            logger.LogWarning("User ID not found in claims or invalid");
            return Results.Unauthorized();
        }

        logger.LogInformation("Updating profile for user ID: {UserId}", userId);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Profile update validation failed for user ID: {UserId}. Errors: {@ValidationErrors}",
                userId, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        try
        {
            var serviceRequest = mapper.Map<UpdateUserRequest>(request);
            var result = await userService.UpdateUserAsync(userId, serviceRequest);

            logger.LogInformation("Profile updated successfully for user ID: {UserId}", userId);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            logger.LogWarning("User not found for ID: {UserId}", userId);
            return Results.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating profile for user ID: {UserId}", userId);
            return Results.Problem("An error occurred while updating the profile");
        }
    }
}


using AutoMapper;
using FluentValidation;
using Upstart.Api.Models;
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
    }

    private static async Task<IResult> CreateUser(CreateUserApiRequest request, UserService userService, IValidator<CreateUserApiRequest> validator, IMapper mapper, ILogger<UserService> logger)
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
}


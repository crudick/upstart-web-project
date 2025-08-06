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

    private static async Task<IResult> CreateUser(CreateUserApiRequest request, UserService userService, IValidator<CreateUserApiRequest> validator, IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = mapper.Map<CreateUserRequest>(request);
        var result = await userService.CreateUserAsync(serviceRequest);
        return Results.Created($"/api/users/{result.Id}", result);
    }
}


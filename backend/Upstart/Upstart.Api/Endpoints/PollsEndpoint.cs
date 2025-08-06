using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Upstart.Api.Extensions;
using Upstart.Api.Models;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class PollsEndpoint
{
    public static void MapPollsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/polls").WithTags("Polls");

        group.MapPost("/", CreatePoll)
            .WithName("CreatePoll")
            .WithSummary("Create a new poll")
            .Produces(201)
            .Produces(400)
            .RequireAuthorization();

        group.MapGet("/{id:int}", GetPollById)
            .WithName("GetPollById")
            .WithSummary("Get a poll by ID")
            .Produces(200)
            .Produces(404);

        group.MapGet("/guid/{guid}", GetPollByGuid)
            .WithName("GetPollByGuid")
            .WithSummary("Get a poll by GUID")
            .Produces(200)
            .Produces(404);

        group.MapGet("/", GetAllPolls)
            .WithName("GetAllPolls")
            .WithSummary("Get all polls")
            .Produces(200);

        group.MapGet("/user/{userId:int}", GetPollsByUserId)
            .WithName("GetPollsByUserId")
            .WithSummary("Get polls by user ID")
            .Produces(200);

        group.MapGet("/active", GetActivePolls)
            .WithName("GetActivePolls")
            .WithSummary("Get all active polls")
            .Produces(200);

        group.MapGet("/public", GetPublicPolls)
            .WithName("GetPublicPolls")
            .WithSummary("Get all public polls")
            .Produces(200);

        group.MapGet("/user", GetUserPolls)
            .WithName("GetUserPolls")
            .WithSummary("Get polls for current user")
            .Produces(200)
            .RequireAuthorization();

        group.MapPut("/{id:int}", UpdatePoll)
            .WithName("UpdatePoll")
            .WithSummary("Update a poll")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .RequireAuthorization();

        group.MapDelete("/{id:int}", DeletePoll)
            .WithName("DeletePoll")
            .WithSummary("Delete a poll")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreatePoll(CreatePollApiRequest request, IPollService pollService, IMapper mapper, ILogger<IPollService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Creating poll: {Question}", request.Question);

        var userId = httpContext.GetUserId();
        var serviceRequest = new CreatePollRequest(
            userId,
            request.Question,
            request.IsActive,
            request.IsMultipleChoice,
            request.ExpiresAt
        );
        
        var result = await pollService.CreatePollAsync(serviceRequest);

        logger.LogInformation("Poll created successfully with ID: {PollId} and GUID: {PollGuid}",
            result.Id, result.PollGuid);

        return Results.Created($"/api/polls/{result.Id}", result);
    }

    private static async Task<IResult> GetPollById(int id, IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting poll by ID: {PollId}", id);

        var result = await pollService.GetPollByIdAsync(id);
        if (result == null)
        {
            logger.LogWarning("Poll not found with ID: {PollId}", id);
            return Results.NotFound($"Poll with ID {id} not found");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollByGuid(string guid, IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting poll by GUID: {PollGuid}", guid);

        var result = await pollService.GetPollByGuidAsync(guid);
        if (result == null)
        {
            logger.LogWarning("Poll not found with GUID: {PollGuid}", guid);
            return Results.NotFound($"Poll with GUID {guid} not found");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllPolls(IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting all polls");

        var result = await pollService.GetAllPollsAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollsByUserId(int userId, IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting polls by user ID: {UserId}", userId);

        var result = await pollService.GetPollsByUserIdAsync(userId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetActivePolls(IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting active polls");

        var result = await pollService.GetActivePollsAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPublicPolls(IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Getting public polls");

        var result = await pollService.GetActivePollsAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserPolls(IPollService pollService, ILogger<IPollService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Getting user polls");

        var userId = httpContext.GetUserId();
        var result = await pollService.GetPollsByUserIdAsync(userId);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePoll(int id, UpdatePollApiRequest request, IPollService pollService, IValidator<UpdatePollApiRequest> validator, IMapper mapper, ILogger<IPollService> logger)
    {
        logger.LogInformation("Updating poll with ID: {PollId}", id);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll update validation failed for ID: {PollId}. Errors: {@ValidationErrors}",
                id, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = new UpdatePollRequest(id, request.Question, request.IsActive, request.IsMultipleChoice, request.ExpiresAt);
        
        try
        {
            var result = await pollService.UpdatePollAsync(serviceRequest);
            logger.LogInformation("Poll updated successfully with ID: {PollId}", result.Id);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll update failed: {ErrorMessage}", ex.Message);
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> DeletePoll(int id, IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Deleting poll with ID: {PollId}", id);

        await pollService.DeletePollAsync(id);
        logger.LogInformation("Poll deleted successfully with ID: {PollId}", id);

        return Results.NoContent();
    }
}
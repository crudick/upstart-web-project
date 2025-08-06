using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Upstart.Api.Extensions;
using Upstart.Api.Models;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class PollStatsEndpoint
{
    public static void MapPollStatsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/poll-stats").WithTags("Poll Stats");

        group.MapPost("/", CreatePollStat)
            .WithName("CreatePollStat")
            .WithSummary("Submit a poll response")
            .Produces(201)
            .Produces(400)
            .RequireAuthorization();

        group.MapPost("/anonymous", CreateAnonymousPollStat)
            .WithName("CreateAnonymousPollStat")
            .WithSummary("Submit an anonymous poll response")
            .Produces(201)
            .Produces(400);

        group.MapGet("/{id:int}", GetPollStatById)
            .WithName("GetPollStatById")
            .WithSummary("Get a poll stat by ID")
            .Produces(200)
            .Produces(404);

        group.MapGet("/poll/{pollId:int}", GetPollStatsByPollId)
            .WithName("GetPollStatsByPollId")
            .WithSummary("Get poll stats by poll ID")
            .Produces(200);

        group.MapGet("/user/{userId:int}", GetPollStatsByUserId)
            .WithName("GetPollStatsByUserId")
            .WithSummary("Get poll stats by user ID")
            .Produces(200);

        group.MapGet("/poll/{pollId:int}/user/{userId:int}", GetUserPollResponse)
            .WithName("GetUserPollResponse")
            .WithSummary("Get user's response to a specific poll")
            .Produces(200)
            .Produces(404)
            .RequireAuthorization();

        group.MapGet("/poll/{pollId:int}/user/me", GetCurrentUserPollResponse)
            .WithName("GetCurrentUserPollResponse")
            .WithSummary("Get current user's response to a specific poll")
            .Produces(200)
            .Produces(404)
            .RequireAuthorization();

        group.MapGet("/poll/{pollId:int}/results", GetPollResults)
            .WithName("GetPollResults")
            .WithSummary("Get poll results")
            .Produces(200);

        group.MapDelete("/{id:int}", DeletePollStat)
            .WithName("DeletePollStat")
            .WithSummary("Delete a poll stat")
            .Produces(204)
            .Produces(404);
    }

    private static async Task<IResult> CreatePollStat(CreatePollStatApiRequest request, IPollStatService pollStatService, IValidator<CreatePollStatApiRequest> validator, IMapper mapper, ILogger<IPollStatService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Creating poll stat for poll ID: {PollId}, user ID: {UserId}", request.PollId, request.UserId);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll stat creation validation failed for poll ID: {PollId}, user ID: {UserId}. Errors: {@ValidationErrors}",
                request.PollId, request.UserId, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        try
        {
            var userId = httpContext.GetUserId();
            var serviceRequest = new CreatePollStatRequest(
                request.PollId,
                request.PollAnswerId,
                userId
            );
            
            var result = await pollStatService.CreatePollStatAsync(serviceRequest);

            logger.LogInformation("Poll stat created successfully with ID: {PollStatId} for poll ID: {PollId}, user ID: {UserId}",
                result.Id, result.PollId, result.UserId);

            return Results.Created($"/api/poll-stats/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll stat creation failed: {ErrorMessage}", ex.Message);
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetPollStatById(int id, IPollStatService pollStatService, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Getting poll stat by ID: {PollStatId}", id);

        var result = await pollStatService.GetPollStatByIdAsync(id);
        if (result == null)
        {
            logger.LogWarning("Poll stat not found with ID: {PollStatId}", id);
            return Results.NotFound($"Poll stat with ID {id} not found");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollStatsByPollId(int pollId, IPollStatService pollStatService, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Getting poll stats by poll ID: {PollId}", pollId);

        var result = await pollStatService.GetPollStatsByPollIdAsync(pollId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollStatsByUserId(int userId, IPollStatService pollStatService, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Getting poll stats by user ID: {UserId}", userId);

        var result = await pollStatService.GetPollStatsByUserIdAsync(userId);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserPollResponse(int pollId, int userId, IPollStatService pollStatService, ILogger<IPollStatService> logger, HttpContext httpContext)
    {
        var authenticatedUserId = httpContext.GetUserId();
        logger.LogInformation("Getting user poll response for poll ID: {PollId}, user ID: {UserId}", pollId, authenticatedUserId);

        var result = await pollStatService.GetUserPollResponseAsync(pollId, authenticatedUserId);
        if (result == null)
        {
            logger.LogWarning("Poll response not found for poll ID: {PollId}, user ID: {UserId}", pollId, authenticatedUserId);
            return Results.NotFound($"Poll response not found for poll {pollId} and user {authenticatedUserId}");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetCurrentUserPollResponse(int pollId, IPollStatService pollStatService, ILogger<IPollStatService> logger, HttpContext httpContext)
    {
        var userId = httpContext.GetUserId();
        logger.LogInformation("Getting current user poll response for poll ID: {PollId}, user ID: {UserId}", pollId, userId);

        var result = await pollStatService.GetUserPollResponseAsync(pollId, userId);
        if (result == null)
        {
            logger.LogWarning("Poll response not found for poll ID: {PollId}, user ID: {UserId}", pollId, userId);
            return Results.NotFound($"Poll response not found for poll {pollId} and user {userId}");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollResults(int pollId, IPollStatService pollStatService, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Getting poll results for poll ID: {PollId}", pollId);

        var result = await pollStatService.GetPollResultsAsync(pollId);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeletePollStat(int id, IPollStatService pollStatService, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Deleting poll stat with ID: {PollStatId}", id);

        await pollStatService.DeletePollStatAsync(id);
        logger.LogInformation("Poll stat deleted successfully with ID: {PollStatId}", id);

        return Results.NoContent();
    }

    private static async Task<IResult> CreateAnonymousPollStat(CreatePollStatApiRequest request, IPollStatService pollStatService, IValidator<CreatePollStatApiRequest> validator, IMapper mapper, ILogger<IPollStatService> logger)
    {
        logger.LogInformation("Creating anonymous poll stat for poll ID: {PollId}", request.PollId);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Anonymous poll stat creation validation failed for poll ID: {PollId}. Errors: {@ValidationErrors}",
                request.PollId, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        try
        {
            var serviceRequest = new CreatePollStatRequest(
                request.PollId,
                request.PollAnswerId,
                null // Anonymous user (no user ID)
            );
            
            var result = await pollStatService.CreatePollStatAsync(serviceRequest);

            logger.LogInformation("Anonymous poll stat created successfully with ID: {PollStatId} for poll ID: {PollId}",
                result.Id, result.PollId);

            return Results.Created($"/api/poll-stats/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Anonymous poll stat creation failed: {ErrorMessage}", ex.Message);
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
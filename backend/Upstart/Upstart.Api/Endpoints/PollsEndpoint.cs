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
            .Produces(400);

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

        group.MapPut("/{id:int}/answers", ReplaceAnswersForPoll)
            .WithName("ReplaceAnswersForPoll")
            .WithSummary("Replace all answers for a poll")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreatePoll(CreatePollApiRequest request, IPollService pollService, IValidator<CreatePollApiRequest> validator, IMapper mapper, ILogger<IPollService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Creating poll: {Question}", request.Question);

        // Try to get user ID if authenticated, otherwise null
        int? userId = null;
        try
        {
            userId = httpContext.GetUserId();
        }
        catch
        {
            // User is not authenticated, which is fine for unauthenticated polls
        }

        // For unauthenticated users, get or create session ID
        string? sessionId = null;
        if (userId == null)
        {
            sessionId = httpContext.Request.Cookies["upstart_session"];
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = httpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Domain = httpContext.Request.IsHttps ? ".onrender.com" : null, // Share across subdomains in production
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                };
                
                logger.LogInformation("Setting upstart_session cookie for host: {Host}, IsHttps: {IsHttps}, Domain: {Domain}", 
                    httpContext.Request.Host, httpContext.Request.IsHttps, cookieOptions.Domain ?? "<not set>");
                
                httpContext.Response.Cookies.Append("upstart_session", sessionId, cookieOptions);
            }
        }

        // Validate the request
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll creation validation failed for user: {UserId}, session: {SessionId}. Errors: {@ValidationErrors}",
                userId, sessionId, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = new CreatePollRequest(
            userId,
            sessionId,
            request.Question,
            request.IsActive,
            request.IsMultipleChoice,
            request.ExpiresAt
        );
        
        var result = await pollService.CreatePollAsync(serviceRequest);

        logger.LogInformation("Poll created successfully with ID: {PollId} and GUID: {PollGuid}, UserId: {UserId}, SessionId: {SessionId}",
            result.Id, result.PollGuid, userId, sessionId);

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

    private static async Task<IResult> UpdatePoll(int id, UpdatePollApiRequest request, IPollService pollService, IValidator<UpdatePollApiRequest> validator, IMapper mapper, ILogger<IPollService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Updating poll with ID: {PollId}", id);

        var userId = httpContext.GetUserId();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll update validation failed for ID: {PollId}. Errors: {@ValidationErrors}",
                id, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = new UpdatePollRequest(id, request.Question, request.IsActive, request.IsMultipleChoice, request.RequiresAuthentication, request.ExpiresAt);
        
        try
        {
            var result = await pollService.UpdatePollAsync(serviceRequest, userId);
            logger.LogInformation("Poll updated successfully with ID: {PollId} by user: {UserId}", result.Id, userId);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll update failed: {ErrorMessage}", ex.Message);
            return Results.NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Poll update unauthorized: {ErrorMessage}", ex.Message);
            return Results.Forbid();
        }
    }

    private static async Task<IResult> DeletePoll(int id, IPollService pollService, ILogger<IPollService> logger)
    {
        logger.LogInformation("Deleting poll with ID: {PollId}", id);

        await pollService.DeletePollAsync(id);
        logger.LogInformation("Poll deleted successfully with ID: {PollId}", id);

        return Results.NoContent();
    }

    private static async Task<IResult> ReplaceAnswersForPoll(int id, List<string> answers, IPollService pollService, ILogger<IPollService> logger, HttpContext httpContext)
    {
        logger.LogInformation("Replacing answers for poll with ID: {PollId}", id);

        var userId = httpContext.GetUserId();

        try
        {
            await pollService.ReplaceAnswersForPollAsync(id, userId, answers);
            logger.LogInformation("Answers replaced successfully for poll ID: {PollId} by user: {UserId}", id, userId);
            return Results.Ok();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll answers replace failed: {ErrorMessage}", ex.Message);
            return Results.NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Poll answers replace unauthorized: {ErrorMessage}", ex.Message);
            return Results.Forbid();
        }
    }
}
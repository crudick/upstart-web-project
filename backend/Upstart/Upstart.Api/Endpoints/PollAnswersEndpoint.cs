using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Upstart.Api.Extensions;
using Upstart.Api.Models;
using Upstart.Application.Interfaces;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class PollAnswersEndpoint
{
    public static void MapPollAnswersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/poll-answers").WithTags("Poll Answers");

        group.MapPost("/", CreatePollAnswer)
            .WithName("CreatePollAnswer")
            .WithSummary("Create a new poll answer")
            .Produces(201)
            .Produces(400)
            .RequireAuthorization();

        group.MapGet("/{id:int}", GetPollAnswerById)
            .WithName("GetPollAnswerById")
            .WithSummary("Get a poll answer by ID")
            .Produces(200)
            .Produces(404);

        group.MapGet("/poll/{pollId:int}", GetPollAnswersByPollId)
            .WithName("GetPollAnswersByPollId")
            .WithSummary("Get poll answers by poll ID")
            .Produces(200);

        group.MapPut("/{id:int}", UpdatePollAnswer)
            .WithName("UpdatePollAnswer")
            .WithSummary("Update a poll answer")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .RequireAuthorization();

        group.MapDelete("/{id:int}", DeletePollAnswer)
            .WithName("DeletePollAnswer")
            .WithSummary("Delete a poll answer")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreatePollAnswer(CreatePollAnswerApiRequest request, IPollAnswerService pollAnswerService, IValidator<CreatePollAnswerApiRequest> validator, IMapper mapper, ILogger<IPollAnswerService> logger)
    {
        logger.LogInformation("Creating poll answer for poll ID: {PollId}", request.PollId);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll answer creation validation failed for poll ID: {PollId}. Errors: {@ValidationErrors}",
                request.PollId, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = mapper.Map<CreatePollAnswerRequest>(request);
        
        try
        {
            var result = await pollAnswerService.CreatePollAnswerAsync(serviceRequest);

            logger.LogInformation("Poll answer created successfully with ID: {PollAnswerId} for poll ID: {PollId}",
                result.Id, result.PollId);

            return Results.Created($"/api/poll-answers/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll answer creation failed: {ErrorMessage}", ex.Message);
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetPollAnswerById(int id, IPollAnswerService pollAnswerService, ILogger<IPollAnswerService> logger)
    {
        logger.LogInformation("Getting poll answer by ID: {PollAnswerId}", id);

        var result = await pollAnswerService.GetPollAnswerByIdAsync(id);
        if (result == null)
        {
            logger.LogWarning("Poll answer not found with ID: {PollAnswerId}", id);
            return Results.NotFound($"Poll answer with ID {id} not found");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPollAnswersByPollId(int pollId, IPollAnswerService pollAnswerService, ILogger<IPollAnswerService> logger)
    {
        logger.LogInformation("Getting poll answers by poll ID: {PollId}", pollId);

        var result = await pollAnswerService.GetPollAnswersByPollIdAsync(pollId);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePollAnswer(int id, UpdatePollAnswerApiRequest request, IPollAnswerService pollAnswerService, IValidator<UpdatePollAnswerApiRequest> validator, IMapper mapper, ILogger<IPollAnswerService> logger)
    {
        logger.LogInformation("Updating poll answer with ID: {PollAnswerId}", id);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Poll answer update validation failed for ID: {PollAnswerId}. Errors: {@ValidationErrors}",
                id, validationResult.Errors);
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = new UpdatePollAnswerRequest(id, request.AnswerText, request.DisplayOrder);
        
        try
        {
            var result = await pollAnswerService.UpdatePollAnswerAsync(serviceRequest);
            logger.LogInformation("Poll answer updated successfully with ID: {PollAnswerId}", result.Id);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Poll answer update failed: {ErrorMessage}", ex.Message);
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> DeletePollAnswer(int id, IPollAnswerService pollAnswerService, ILogger<IPollAnswerService> logger)
    {
        logger.LogInformation("Deleting poll answer with ID: {PollAnswerId}", id);

        await pollAnswerService.DeletePollAnswerAsync(id);
        logger.LogInformation("Poll answer deleted successfully with ID: {PollAnswerId}", id);

        return Results.NoContent();
    }
}
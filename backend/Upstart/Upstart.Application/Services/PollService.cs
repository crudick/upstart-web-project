using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class PollService : IPollService
{
    private readonly IPollsRepository _pollsRepository;
    private readonly IPollAnswersRepository _pollAnswersRepository;
    private readonly ILogger<PollService> _logger;

    public PollService(IPollsRepository pollsRepository, IPollAnswersRepository pollAnswersRepository, ILogger<PollService> logger)
    {
        _pollsRepository = pollsRepository;
        _pollAnswersRepository = pollAnswersRepository;
        _logger = logger;
    }

    public async Task<PollModel> CreatePollAsync(CreatePollRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating poll: {Question}", request.Question);

        var poll = new PollModel
        {
            PollGuid = Guid.NewGuid().ToString(),
            UserId = request.UserId,
            SessionId = request.SessionId,
            Question = request.Question,
            IsActive = request.IsActive,
            IsMultipleChoice = request.IsMultipleChoice,
            ExpiresAt = request.ExpiresAt?.ToUniversalTime(),
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _pollsRepository.CreateAsync(poll);
    }

    public async Task<PollModel?> GetPollByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetByIdAsync(id);
    }

    public async Task<PollModel?> GetPollByGuidAsync(string guid, CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetByGuidAsync(guid);
    }

    public async Task<IEnumerable<PollModel>> GetAllPollsAsync(CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetAllAsync();
    }

    public async Task<IEnumerable<PollModel>> GetPollsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<PollModel>> GetActivePollsAsync(CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetActiveAsync();
    }

    public async Task<PollModel> UpdatePollAsync(UpdatePollRequest request, int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating poll with ID: {PollId} by user: {UserId}", request.Id, userId);

        var existingPoll = await _pollsRepository.GetByIdAsync(request.Id);
        if (existingPoll == null)
        {
            _logger.LogWarning("Poll with ID {PollId} not found", request.Id);
            throw new InvalidOperationException($"Poll with ID '{request.Id}' not found.");
        }

        _logger.LogDebug("Found poll with ID: {PollId}, UserId: {PollUserId}, requesting user: {RequestingUserId}", 
            existingPoll.Id, existingPoll.UserId, userId);

        // Check ownership - user can only edit their own polls
        if (existingPoll.UserId != userId)
        {
            _logger.LogWarning("User {UserId} is not authorized to edit poll {PollId} (owned by user {PollUserId})", 
                userId, request.Id, existingPoll.UserId);
            throw new UnauthorizedAccessException($"User {userId} is not authorized to edit poll {request.Id}.");
        }

        existingPoll.Question = request.Question;
        existingPoll.IsActive = request.IsActive;
        existingPoll.IsMultipleChoice = request.IsMultipleChoice;
        existingPoll.RequiresAuthentication = request.RequiresAuthentication;
        existingPoll.ExpiresAt = request.ExpiresAt?.ToUniversalTime();
        existingPoll.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        return await _pollsRepository.UpdateAsync(existingPoll);
    }

    public async Task DeletePollAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting poll with ID: {PollId}", id);
        await _pollsRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PollModel>> GetPollsBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _pollsRepository.GetBySessionIdAsync(sessionId);
    }

    public async Task<int> MigratePollsFromSessionToUserAsync(string sessionId, int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Migrating polls from session {SessionId} to user {UserId}", sessionId, userId);
        return await _pollsRepository.MigratePollsFromSessionToUserAsync(sessionId, userId);
    }

    public async Task ReplaceAnswersForPollAsync(int pollId, int userId, List<string> newAnswers, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Replacing answers for poll ID: {PollId} by user: {UserId}", pollId, userId);

        // First verify the user owns the poll
        var poll = await _pollsRepository.GetByIdAsync(pollId);
        if (poll == null)
        {
            throw new InvalidOperationException($"Poll with ID '{pollId}' not found.");
        }

        if (poll.UserId != userId)
        {
            throw new UnauthorizedAccessException($"User {userId} is not authorized to edit poll {pollId}.");
        }

        // Get current answers and delete them
        var currentAnswers = await _pollAnswersRepository.GetByPollIdAsync(pollId);
        foreach (var answer in currentAnswers)
        {
            await _pollAnswersRepository.DeleteAsync(answer.Id);
        }

        // Create new answers
        for (int i = 0; i < newAnswers.Count; i++)
        {
            var newAnswer = new PollAnswerModel
            {
                PollId = pollId,
                AnswerText = newAnswers[i].Trim(),
                DisplayOrder = i + 1,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };
            
            await _pollAnswersRepository.CreateAsync(newAnswer);
        }
    }
}

public record CreatePollRequest(
    int? UserId,
    string? SessionId,
    string Question,
    bool IsActive = true,
    bool IsMultipleChoice = false,
    DateTime? ExpiresAt = null
);

public record UpdatePollRequest(
    int Id,
    string Question,
    bool IsActive,
    bool IsMultipleChoice,
    bool RequiresAuthentication,
    DateTime? ExpiresAt
);
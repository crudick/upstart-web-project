using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class PollService : IPollService
{
    private readonly IPollsRepository _pollsRepository;
    private readonly ILogger<PollService> _logger;

    public PollService(IPollsRepository pollsRepository, ILogger<PollService> logger)
    {
        _pollsRepository = pollsRepository;
        _logger = logger;
    }

    public async Task<PollModel> CreatePollAsync(CreatePollRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating poll: {Question}", request.Question);

        var poll = new PollModel
        {
            PollGuid = Guid.NewGuid().ToString(),
            UserId = request.UserId,
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

    public async Task<PollModel> UpdatePollAsync(UpdatePollRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating poll with ID: {PollId}", request.Id);

        var existingPoll = await _pollsRepository.GetByIdAsync(request.Id);
        if (existingPoll == null)
        {
            throw new InvalidOperationException($"Poll with ID '{request.Id}' not found.");
        }

        existingPoll.Question = request.Question;
        existingPoll.IsActive = request.IsActive;
        existingPoll.IsMultipleChoice = request.IsMultipleChoice;
        existingPoll.ExpiresAt = request.ExpiresAt?.ToUniversalTime();
        existingPoll.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        return await _pollsRepository.UpdateAsync(existingPoll);
    }

    public async Task DeletePollAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting poll with ID: {PollId}", id);
        await _pollsRepository.DeleteAsync(id);
    }
}

public record CreatePollRequest(
    int UserId,
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
    DateTime? ExpiresAt
);
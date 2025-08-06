using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class PollStatService : IPollStatService
{
    private readonly IPollStatsRepository _pollStatsRepository;
    private readonly IPollsRepository _pollsRepository;
    private readonly ILogger<PollStatService> _logger;

    public PollStatService(IPollStatsRepository pollStatsRepository, IPollsRepository pollsRepository, ILogger<PollStatService> logger)
    {
        _pollStatsRepository = pollStatsRepository;
        _pollsRepository = pollsRepository;
        _logger = logger;
    }

    public async Task<PollStatModel> CreatePollStatAsync(CreatePollStatRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating poll stat for poll ID: {PollId}, user ID: {UserId}", request.PollId, request.UserId);

        // Verify poll is active and not expired
        var poll = await _pollsRepository.GetByIdAsync(request.PollId);
        if (poll == null)
        {
            throw new InvalidOperationException($"Poll with ID '{request.PollId}' not found.");
        }

        if (!poll.IsActive)
        {
            throw new InvalidOperationException($"Poll '{request.PollId}' is not active.");
        }

        if (poll.ExpiresAt.HasValue && poll.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException($"Poll '{request.PollId}' has expired.");
        }

        // Check authentication requirement and user ID validation
        if (poll.RequiresAuthentication && (!request.UserId.HasValue || request.UserId <= 0))
        {
            throw new InvalidOperationException($"Poll '{request.PollId}' requires authentication.");
        }

        // Check if user has already responded to this poll (only for authenticated users)
        if (request.UserId.HasValue && request.UserId > 0)
        {
            var existingResponse = await _pollStatsRepository.GetUserResponseAsync(request.PollId, request.UserId.Value);
            if (existingResponse != null)
            {
                throw new InvalidOperationException($"User '{request.UserId}' has already responded to poll '{request.PollId}'.");
            }
        }

        var pollStat = new PollStatModel
        {
            PollId = request.PollId,
            PollAnswerId = request.PollAnswerId,
            UserId = request.UserId,
            SelectedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _pollStatsRepository.CreateAsync(pollStat);
    }

    public async Task<PollStatModel?> GetPollStatByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _pollStatsRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PollStatModel>> GetPollStatsByPollIdAsync(int pollId, CancellationToken cancellationToken = default)
    {
        return await _pollStatsRepository.GetByPollIdAsync(pollId);
    }

    public async Task<IEnumerable<PollStatModel>> GetPollStatsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _pollStatsRepository.GetByUserIdAsync(userId);
    }

    public async Task<PollStatModel?> GetUserPollResponseAsync(int pollId, int userId, CancellationToken cancellationToken = default)
    {
        return await _pollStatsRepository.GetUserResponseAsync(pollId, userId);
    }

    public async Task<IEnumerable<PollStatModel>> GetPollResultsAsync(int pollId, CancellationToken cancellationToken = default)
    {
        return await _pollStatsRepository.GetPollResultsAsync(pollId);
    }

    public async Task DeletePollStatAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting poll stat with ID: {PollStatId}", id);
        await _pollStatsRepository.DeleteAsync(id);
    }
}

public record CreatePollStatRequest(
    int PollId,
    int PollAnswerId,
    int? UserId
);
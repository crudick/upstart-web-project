using Microsoft.Extensions.Logging;
using Upstart.Application.Interfaces;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;

namespace Upstart.Application.Services;

public class PollAnswerService : IPollAnswerService
{
    private readonly IPollAnswersRepository _pollAnswersRepository;
    private readonly ILogger<PollAnswerService> _logger;

    public PollAnswerService(IPollAnswersRepository pollAnswersRepository, ILogger<PollAnswerService> logger)
    {
        _pollAnswersRepository = pollAnswersRepository;
        _logger = logger;
    }

    public async Task<PollAnswerModel> CreatePollAnswerAsync(CreatePollAnswerRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating poll answer for poll ID: {PollId}", request.PollId);

        var pollAnswer = new PollAnswerModel
        {
            PollId = request.PollId,
            AnswerText = request.AnswerText,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };

        return await _pollAnswersRepository.CreateAsync(pollAnswer);
    }

    public async Task<PollAnswerModel?> GetPollAnswerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _pollAnswersRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<PollAnswerModel>> GetPollAnswersByPollIdAsync(int pollId, CancellationToken cancellationToken = default)
    {
        return await _pollAnswersRepository.GetByPollIdAsync(pollId);
    }

    public async Task<PollAnswerModel> UpdatePollAnswerAsync(UpdatePollAnswerRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating poll answer with ID: {PollAnswerId}", request.Id);

        var existingPollAnswer = await _pollAnswersRepository.GetByIdAsync(request.Id);
        if (existingPollAnswer == null)
        {
            throw new InvalidOperationException($"Poll answer with ID '{request.Id}' not found.");
        }

        existingPollAnswer.AnswerText = request.AnswerText;
        existingPollAnswer.DisplayOrder = request.DisplayOrder;
        existingPollAnswer.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        return await _pollAnswersRepository.UpdateAsync(existingPollAnswer);
    }

    public async Task DeletePollAnswerAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting poll answer with ID: {PollAnswerId}", id);
        await _pollAnswersRepository.DeleteAsync(id);
    }
}

public record CreatePollAnswerRequest(
    int PollId,
    string AnswerText,
    int DisplayOrder
);

public record UpdatePollAnswerRequest(
    int Id,
    string AnswerText,
    int DisplayOrder
);
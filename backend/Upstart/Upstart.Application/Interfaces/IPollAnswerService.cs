using Upstart.Application.Services;
using Upstart.Domain.Models;

namespace Upstart.Application.Interfaces;

public interface IPollAnswerService
{
    Task<PollAnswerModel> CreatePollAnswerAsync(CreatePollAnswerRequest request, CancellationToken cancellationToken = default);
    Task<PollAnswerModel?> GetPollAnswerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollAnswerModel>> GetPollAnswersByPollIdAsync(int pollId, CancellationToken cancellationToken = default);
    Task<PollAnswerModel> UpdatePollAnswerAsync(UpdatePollAnswerRequest request, CancellationToken cancellationToken = default);
    Task DeletePollAnswerAsync(int id, CancellationToken cancellationToken = default);
}
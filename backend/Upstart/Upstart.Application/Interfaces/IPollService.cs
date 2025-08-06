using Upstart.Application.Services;
using Upstart.Domain.Models;

namespace Upstart.Application.Interfaces;

public interface IPollService
{
    Task<PollModel> CreatePollAsync(CreatePollRequest request, CancellationToken cancellationToken = default);
    Task<PollModel?> GetPollByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PollModel?> GetPollByGuidAsync(string guid, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollModel>> GetAllPollsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PollModel>> GetPollsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollModel>> GetActivePollsAsync(CancellationToken cancellationToken = default);
    Task<PollModel> UpdatePollAsync(UpdatePollRequest request, CancellationToken cancellationToken = default);
    Task DeletePollAsync(int id, CancellationToken cancellationToken = default);
}
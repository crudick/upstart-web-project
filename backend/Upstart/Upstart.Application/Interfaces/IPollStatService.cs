using Upstart.Application.Services;
using Upstart.Domain.Models;

namespace Upstart.Application.Interfaces;

public interface IPollStatService
{
    Task<PollStatModel> CreatePollStatAsync(CreatePollStatRequest request, CancellationToken cancellationToken = default);
    Task<PollStatModel?> GetPollStatByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollStatModel>> GetPollStatsByPollIdAsync(int pollId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollStatModel>> GetPollStatsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<PollStatModel?> GetUserPollResponseAsync(int pollId, int userId, CancellationToken cancellationToken = default);
    Task<PollStatModel?> GetSessionPollResponseAsync(int pollId, string sessionId, CancellationToken cancellationToken = default);
    Task<PollStatModel> UpdatePollStatAsync(UpdatePollStatRequest request, int? userId, string? sessionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PollStatModel>> GetPollResultsAsync(int pollId, CancellationToken cancellationToken = default);
    Task DeletePollStatAsync(int id, CancellationToken cancellationToken = default);
}
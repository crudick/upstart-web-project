using Upstart.Domain.Models;

namespace Upstart.Domain.Interfaces;

public interface IPollStatsRepository
{
    Task<PollStatModel> CreateAsync(PollStatModel pollStat);
    Task<PollStatModel?> GetByIdAsync(int id);
    Task<IEnumerable<PollStatModel>> GetByPollIdAsync(int pollId);
    Task<IEnumerable<PollStatModel>> GetByUserIdAsync(int userId);
    Task<PollStatModel?> GetUserResponseAsync(int pollId, int userId);
    Task<PollStatModel?> GetSessionResponseAsync(int pollId, string sessionId);
    Task<PollStatModel> UpdateAsync(PollStatModel pollStat);
    Task<IEnumerable<PollStatModel>> GetPollResultsAsync(int pollId);
    Task DeleteAsync(int id);
    Task DeleteByPollIdAsync(int pollId);
}
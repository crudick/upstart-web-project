using Upstart.Domain.Models;

namespace Upstart.Domain.Interfaces;

public interface IPollsRepository
{
    Task<PollModel> CreateAsync(PollModel poll);
    Task<PollModel?> GetByIdAsync(int id);
    Task<PollModel?> GetByGuidAsync(string guid);
    Task<IEnumerable<PollModel>> GetAllAsync();
    Task<IEnumerable<PollModel>> GetByUserIdAsync(int userId);
    Task<IEnumerable<PollModel>> GetActiveAsync();
    Task<PollModel> UpdateAsync(PollModel poll);
    Task DeleteAsync(int id);
    Task<IEnumerable<PollModel>> GetBySessionIdAsync(string sessionId);
    Task<int> MigratePollsFromSessionToUserAsync(string sessionId, int userId);
}
using Upstart.Domain.Models;

namespace Upstart.Domain.Interfaces;

public interface IPollAnswersRepository
{
    Task<PollAnswerModel> CreateAsync(PollAnswerModel pollAnswer);
    Task<PollAnswerModel?> GetByIdAsync(int id);
    Task<IEnumerable<PollAnswerModel>> GetByPollIdAsync(int pollId);
    Task<PollAnswerModel> UpdateAsync(PollAnswerModel pollAnswer);
    Task DeleteAsync(int id);
    Task DeleteByPollIdAsync(int pollId);
}
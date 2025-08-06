using Upstart.Domain.Models;

namespace Upstart.Domain.Interfaces;

public interface ILoansRepository
{
    Task<LoanModel> CreateAsync(LoanModel loan);
    Task<LoanModel?> GetByIdAsync(int id);
    Task<IEnumerable<LoanModel>> GetByUserIdAsync(int userId);
    Task<IEnumerable<LoanModel>> GetAllAsync();
    Task<LoanModel> UpdateAsync(LoanModel loan);
    Task DeleteAsync(int id);
}
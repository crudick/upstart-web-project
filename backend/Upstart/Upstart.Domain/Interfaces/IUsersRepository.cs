using Upstart.Domain.Models;

namespace Upstart.Domain.Interfaces;

public interface IUsersRepository
{
    Task<UserModel> CreateAsync(UserModel user);
    Task<UserModel?> GetByIdAsync(int id);
    Task<UserModel?> GetByEmailAsync(string email);
    Task<IEnumerable<UserModel>> GetAllAsync();
    Task<UserModel> UpdateAsync(UserModel user);
    Task DeleteAsync(int id);
}
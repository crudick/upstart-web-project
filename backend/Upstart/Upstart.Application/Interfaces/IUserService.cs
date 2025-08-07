using Upstart.Application.Services;
using Upstart.Domain.Models;

namespace Upstart.Application.Interfaces;

public interface IUserService
{
    Task<UserModel> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserModel> UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
using Microsoft.EntityFrameworkCore;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly UpstartDbContext _context;

    public UsersRepository(UpstartDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel> CreateAsync(UserModel user)
    {
        var entity = MapToEntity(user);
        _context.Set<UserEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<UserModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<UserEntity>().FindAsync(id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var entity = await _context.Set<UserEntity>()
            .FirstOrDefaultAsync(u => u.Email == email);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var entities = await _context.Set<UserEntity>().ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<UserModel> UpdateAsync(UserModel user)
    {
        var entity = MapToEntity(user);
        _context.Set<UserEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<UserEntity>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<UserEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private static UserEntity MapToEntity(UserModel model)
    {
        return new UserEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            PhoneNumber = model.PhoneNumber,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    private static UserModel MapToModel(UserEntity entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            PhoneNumber = entity.PhoneNumber,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
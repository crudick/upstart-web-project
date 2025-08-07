using Microsoft.EntityFrameworkCore;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Repositories;

public class PollsRepository : IPollsRepository
{
    private readonly UpstartDbContext _context;

    public PollsRepository(UpstartDbContext context)
    {
        _context = context;
    }

    public async Task<PollModel> CreateAsync(PollModel poll)
    {
        var entity = MapToEntity(poll);
        _context.Set<PollEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<PollModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .FirstOrDefaultAsync(p => p.Id == id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<PollModel?> GetByGuidAsync(string guid)
    {
        var entity = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .FirstOrDefaultAsync(p => p.PollGuid == guid);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<PollModel>> GetAllAsync()
    {
        var entities = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<IEnumerable<PollModel>> GetByUserIdAsync(int userId)
    {
        var entities = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .Where(p => p.UserId == userId)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<IEnumerable<PollModel>> GetActiveAsync()
    {
        var entities = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .Where(p => p.IsActive && (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow))
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<PollModel> UpdateAsync(PollModel poll)
    {
        // First check if the entity is already being tracked
        var trackedEntity = _context.ChangeTracker.Entries<PollEntity>()
            .FirstOrDefault(e => e.Entity.Id == poll.Id);

        PollEntity entityToUpdate;
        
        if (trackedEntity != null)
        {
            // Update the already tracked entity
            entityToUpdate = trackedEntity.Entity;
            entityToUpdate.PollGuid = poll.PollGuid;
            entityToUpdate.UserId = poll.UserId;
            entityToUpdate.SessionId = poll.SessionId;
            entityToUpdate.Question = poll.Question;
            entityToUpdate.IsActive = poll.IsActive;
            entityToUpdate.IsMultipleChoice = poll.IsMultipleChoice;
            entityToUpdate.RequiresAuthentication = poll.RequiresAuthentication;
            entityToUpdate.ExpiresAt = poll.ExpiresAt;
            entityToUpdate.CreatedAt = poll.CreatedAt;
            entityToUpdate.UpdatedAt = poll.UpdatedAt;
        }
        else
        {
            // No tracked entity, create new one and attach it
            entityToUpdate = MapToEntity(poll);
            _context.Set<PollEntity>().Update(entityToUpdate);
        }

        await _context.SaveChangesAsync();
        return MapToModel(entityToUpdate);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<PollEntity>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<PollEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PollModel>> GetBySessionIdAsync(string sessionId)
    {
        var entities = await _context.Set<PollEntity>()
            .Include(p => p.Answers)
            .Include(p => p.User)
            .Include(p => p.Stats)
            .Where(p => p.SessionId == sessionId)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<int> MigratePollsFromSessionToUserAsync(string sessionId, int userId)
    {
        var pollsToMigrate = await _context.Set<PollEntity>()
            .Where(p => p.SessionId == sessionId && p.UserId == null)
            .ToListAsync();

        foreach (var poll in pollsToMigrate)
        {
            poll.UserId = userId;
            poll.SessionId = null;
            poll.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return pollsToMigrate.Count;
    }

    private static PollEntity MapToEntity(PollModel model)
    {
        return new PollEntity
        {
            Id = model.Id,
            PollGuid = model.PollGuid,
            UserId = model.UserId,
            SessionId = model.SessionId,
            Question = model.Question,
            IsActive = model.IsActive,
            IsMultipleChoice = model.IsMultipleChoice,
            RequiresAuthentication = model.RequiresAuthentication,
            ExpiresAt = model.ExpiresAt,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    private static PollModel MapToModel(PollEntity entity)
    {
        return new PollModel
        {
            Id = entity.Id,
            PollGuid = entity.PollGuid,
            UserId = entity.UserId,
            SessionId = entity.SessionId,
            Question = entity.Question,
            IsActive = entity.IsActive,
            IsMultipleChoice = entity.IsMultipleChoice,
            RequiresAuthentication = entity.RequiresAuthentication,
            ExpiresAt = entity.ExpiresAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            User = entity.User != null ? MapUserToModel(entity.User) : null,
            Answers = entity.Answers.Select(MapAnswerToModel).ToList(),
            Stats = entity.Stats.Select(MapStatToModel).ToList()
        };
    }

    private static UserModel MapUserToModel(UserEntity entity)
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

    private static PollAnswerModel MapAnswerToModel(PollAnswerEntity entity)
    {
        return new PollAnswerModel
        {
            Id = entity.Id,
            PollId = entity.PollId,
            AnswerText = entity.AnswerText,
            DisplayOrder = entity.DisplayOrder,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static PollStatModel MapStatToModel(PollStatEntity entity)
    {
        return new PollStatModel
        {
            Id = entity.Id,
            PollId = entity.PollId,
            PollAnswerId = entity.PollAnswerId,
            UserId = entity.UserId,
            SelectedAt = entity.SelectedAt
        };
    }
}
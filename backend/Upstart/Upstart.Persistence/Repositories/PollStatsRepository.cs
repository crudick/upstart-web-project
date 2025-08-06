using Microsoft.EntityFrameworkCore;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Repositories;

public class PollStatsRepository : IPollStatsRepository
{
    private readonly UpstartDbContext _context;

    public PollStatsRepository(UpstartDbContext context)
    {
        _context = context;
    }

    public async Task<PollStatModel> CreateAsync(PollStatModel pollStat)
    {
        var entity = MapToEntity(pollStat);
        _context.Set<PollStatEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<PollStatModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<PollStatEntity>()
            .Include(ps => ps.Poll)
            .Include(ps => ps.PollAnswer)
            .Include(ps => ps.User)
            .FirstOrDefaultAsync(ps => ps.Id == id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<PollStatModel>> GetByPollIdAsync(int pollId)
    {
        var entities = await _context.Set<PollStatEntity>()
            .Include(ps => ps.Poll)
            .Include(ps => ps.PollAnswer)
            .Include(ps => ps.User)
            .Where(ps => ps.PollId == pollId)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<IEnumerable<PollStatModel>> GetByUserIdAsync(int userId)
    {
        var entities = await _context.Set<PollStatEntity>()
            .Include(ps => ps.Poll)
            .Include(ps => ps.PollAnswer)
            .Include(ps => ps.User)
            .Where(ps => ps.UserId == userId)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<PollStatModel?> GetUserResponseAsync(int pollId, int userId)
    {
        var entity = await _context.Set<PollStatEntity>()
            .Include(ps => ps.Poll)
            .Include(ps => ps.PollAnswer)
            .Include(ps => ps.User)
            .FirstOrDefaultAsync(ps => ps.PollId == pollId && ps.UserId == userId);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<PollStatModel>> GetPollResultsAsync(int pollId)
    {
        var entities = await _context.Set<PollStatEntity>()
            .Include(ps => ps.Poll)
            .Include(ps => ps.PollAnswer)
            .Include(ps => ps.User)
            .Where(ps => ps.PollId == pollId)
            .OrderBy(ps => ps.SelectedAt)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<PollStatEntity>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<PollStatEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByPollIdAsync(int pollId)
    {
        var entities = await _context.Set<PollStatEntity>()
            .Where(ps => ps.PollId == pollId)
            .ToListAsync();
        
        if (entities.Any())
        {
            _context.Set<PollStatEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }

    private static PollStatEntity MapToEntity(PollStatModel model)
    {
        return new PollStatEntity
        {
            Id = model.Id,
            PollId = model.PollId,
            PollAnswerId = model.PollAnswerId,
            UserId = model.UserId,
            SelectedAt = model.SelectedAt
        };
    }

    private static PollStatModel MapToModel(PollStatEntity entity)
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
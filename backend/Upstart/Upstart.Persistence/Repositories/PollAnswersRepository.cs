using Microsoft.EntityFrameworkCore;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Repositories;

public class PollAnswersRepository : IPollAnswersRepository
{
    private readonly UpstartDbContext _context;

    public PollAnswersRepository(UpstartDbContext context)
    {
        _context = context;
    }

    public async Task<PollAnswerModel> CreateAsync(PollAnswerModel pollAnswer)
    {
        var entity = MapToEntity(pollAnswer);
        _context.Set<PollAnswerEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<PollAnswerModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<PollAnswerEntity>()
            .Include(pa => pa.Poll)
            .FirstOrDefaultAsync(pa => pa.Id == id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<PollAnswerModel>> GetByPollIdAsync(int pollId)
    {
        var entities = await _context.Set<PollAnswerEntity>()
            .Include(pa => pa.Poll)
            .Where(pa => pa.PollId == pollId)
            .OrderBy(pa => pa.DisplayOrder)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<PollAnswerModel> UpdateAsync(PollAnswerModel pollAnswer)
    {
        var entity = MapToEntity(pollAnswer);
        _context.Set<PollAnswerEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<PollAnswerEntity>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<PollAnswerEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByPollIdAsync(int pollId)
    {
        var entities = await _context.Set<PollAnswerEntity>()
            .Where(pa => pa.PollId == pollId)
            .ToListAsync();
        
        if (entities.Any())
        {
            _context.Set<PollAnswerEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }

    private static PollAnswerEntity MapToEntity(PollAnswerModel model)
    {
        return new PollAnswerEntity
        {
            Id = model.Id,
            PollId = model.PollId,
            AnswerText = model.AnswerText,
            DisplayOrder = model.DisplayOrder,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    private static PollAnswerModel MapToModel(PollAnswerEntity entity)
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
}
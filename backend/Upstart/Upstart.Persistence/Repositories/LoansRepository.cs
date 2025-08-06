using Microsoft.EntityFrameworkCore;
using Upstart.Domain.Interfaces;
using Upstart.Domain.Models;
using Upstart.Persistence.Data;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Repositories;

public class LoansRepository : ILoansRepository
{
    private readonly UpstartDbContext _context;

    public LoansRepository(UpstartDbContext context)
    {
        _context = context;
    }

    public async Task<LoanModel> CreateAsync(LoanModel loan)
    {
        var entity = MapToEntity(loan);
        _context.Set<LoanEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task<LoanModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<LoanEntity>()
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);
        return entity != null ? MapToModel(entity) : null;
    }

    public async Task<IEnumerable<LoanModel>> GetByUserIdAsync(int userId)
    {
        var entities = await _context.Set<LoanEntity>()
            .Where(l => l.UserId == userId)
            .Include(l => l.User)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<IEnumerable<LoanModel>> GetAllAsync()
    {
        var entities = await _context.Set<LoanEntity>()
            .Include(l => l.User)
            .ToListAsync();
        return entities.Select(MapToModel);
    }

    public async Task<LoanModel> UpdateAsync(LoanModel loan)
    {
        var entity = MapToEntity(loan);
        _context.Set<LoanEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return MapToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<LoanEntity>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<LoanEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private static LoanEntity MapToEntity(LoanModel model)
    {
        return new LoanEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            LoanAmount = model.LoanAmount,
            InterestRate = model.InterestRate,
            TermMonths = model.TermMonths,
            MonthlyPayment = model.MonthlyPayment,
            LoanPurpose = model.LoanPurpose,
            LoanStatus = model.LoanStatus,
            ApplicationDate = model.ApplicationDate,
            ApprovalDate = model.ApprovalDate,
            DisbursementDate = model.DisbursementDate,
            MaturityDate = model.MaturityDate,
            OutstandingBalance = model.OutstandingBalance,
            TotalPaymentsMade = model.TotalPaymentsMade,
            NextPaymentDueDate = model.NextPaymentDueDate,
            PaymentFrequency = model.PaymentFrequency,
            LateFees = model.LateFees,
            OriginationFee = model.OriginationFee,
            APR = model.APR,
            LoanOfficerNotes = model.LoanOfficerNotes,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    private static LoanModel MapToModel(LoanEntity entity)
    {
        return new LoanModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            LoanAmount = entity.LoanAmount,
            InterestRate = entity.InterestRate,
            TermMonths = entity.TermMonths,
            MonthlyPayment = entity.MonthlyPayment,
            LoanPurpose = entity.LoanPurpose,
            LoanStatus = entity.LoanStatus,
            ApplicationDate = entity.ApplicationDate,
            ApprovalDate = entity.ApprovalDate,
            DisbursementDate = entity.DisbursementDate,
            MaturityDate = entity.MaturityDate,
            OutstandingBalance = entity.OutstandingBalance,
            TotalPaymentsMade = entity.TotalPaymentsMade,
            NextPaymentDueDate = entity.NextPaymentDueDate,
            PaymentFrequency = entity.PaymentFrequency,
            LateFees = entity.LateFees,
            OriginationFee = entity.OriginationFee,
            APR = entity.APR,
            LoanOfficerNotes = entity.LoanOfficerNotes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
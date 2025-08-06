using Upstart.Application.Services;
using Upstart.Domain.Models;

namespace Upstart.Application.Interfaces;

public interface ILoanService
{
    Task<LoanModel> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
}
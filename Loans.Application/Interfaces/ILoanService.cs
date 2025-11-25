using Loans.Domain;
using Loans.Domain.Dtos;

namespace Loans.Application.Interfaces
{
    public interface ILoanService
    {
        Task<LoanDto> CreateLoanAsync(Guid userId, LoanCreateDto dto);
        Task<IEnumerable<LoanDto>> GetLoansForUserAsync(Guid userId);
        Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<LoanDto> GetLoanByIdAsync(Guid loanId, Guid callerUserId, bool isAccountant);
        Task UpdateLoanAsync(Guid loanId, Guid callerUserId, LoanUpdateDto dto, bool isAccountant);
        Task DeleteLoanAsync(Guid loanId, Guid callerUserId, bool isAccountant);
        Task OverrideLoanStatusAsync(Guid loanId, LoanStatus status);
    }
}

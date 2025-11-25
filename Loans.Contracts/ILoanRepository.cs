using Loans.Domain.Entities;

namespace Loans.Contracts
{
    public interface ILoanRepository : IRepositoryBase<Loan>
    {
        Task<IEnumerable<Loan>> GetLoansForUserAsync(Guid userId, bool trackChanges);
        Task<Loan?> GetLoanByIdAsync(Guid loanId, bool trackChanges);
    }
}

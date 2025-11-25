using Loans.Contracts;
using Loans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loans.Infrastructure.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(RepositoryContext context) : base(context) { }

        public async Task<Loan?> GetLoanByIdAsync(Guid loanId, bool trackChanges)
        {
            return await FindByCondition(l => l.Id == loanId, trackChanges).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansForUserAsync(Guid userId, bool trackChanges)
        {
            return await FindByCondition(l => l.UserId == userId, trackChanges).ToListAsync();
        }
    }
}

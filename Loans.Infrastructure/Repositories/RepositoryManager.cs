using Loans.Contracts;

namespace Loans.Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<IUserRepository> _userRepo;
        private readonly Lazy<ILoanRepository> _loanRepo;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _userRepo = new Lazy<IUserRepository>(() => new UserRepository(_context));
            _loanRepo = new Lazy<ILoanRepository>(() => new LoanRepository(_context));
        }

        public IUserRepository User => _userRepo.Value;
        public ILoanRepository Loan => _loanRepo.Value;
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}

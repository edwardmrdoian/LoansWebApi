namespace Loans.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }
        ILoanRepository Loan { get; }
        Task SaveAsync();
    }
}

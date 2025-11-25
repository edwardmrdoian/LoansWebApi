using Loans.Domain.Entities;

namespace Loans.Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByUsernameAsync(string username, bool trackChanges);
        Task<User?> GetByIdAsync(Guid id, bool trackChanges);
    }
}

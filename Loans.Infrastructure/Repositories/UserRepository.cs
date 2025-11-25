using Loans.Contracts;
using Loans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loans.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext context) : base(context) { }

        public async Task<User?> GetByIdAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(u => u.Id == id, trackChanges).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username, bool trackChanges)
        {
            return await FindByCondition(u => u.Username == username, trackChanges).FirstOrDefaultAsync();
        }
    }
}

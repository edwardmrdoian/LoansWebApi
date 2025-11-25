using Loans.Domain.Dtos;

namespace Loans.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task BlockUserAsync(Guid id, int days);

        Task<bool> IsUserBlockedAsync(Guid id);
    }
}

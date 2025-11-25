using AutoMapper;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Contracts;
using Loans.Domain.Dtos;
using Loans.Domain.Errors;

namespace Loans.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public UserService(IRepositoryManager repo, IMapper mapper, ILoggerManager logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _repo.User.GetByIdAsync(id, false)
                ?? throw new NotFoundException("User not found", ErrorCodes.USER_NOT_FOUND);
            return new UserDto(user.Id, user.FirstName, user.LastName, user.Username, user.Age, user.MonthlyIncome, user.Email, user.IsBlocked, user.Role.ToString());
        }

        public async Task BlockUserAsync(Guid id, int days)
        {
            var user = await _repo.User.GetByIdAsync(id, true)
                    ?? throw new NotFoundException("User not found", ErrorCodes.USER_NOT_FOUND);

            if (user.IsBlocked)
                throw new BadRequestException("User is already blocked.", ErrorCodes.BAD_REQUEST);

            user.IsBlocked = true;
            _repo.User.Update(user);
            await _repo.SaveAsync();

            _logger.LogInfo($"User blocked: {user.Username} ({user.Id}) for {days} days");
        }

        public async Task<bool> IsUserBlockedAsync(Guid id)
        {
            var user = await _repo.User.GetByIdAsync(id, false)
                ?? throw new NotFoundException("User not found.", ErrorCodes.USER_NOT_FOUND);
            return user.IsBlocked;
        }
    }
}

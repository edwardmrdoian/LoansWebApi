using AutoMapper;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Contracts;
using Loans.Domain;
using Loans.Domain.Dtos;
using Loans.Domain.Entities;
using Loans.Domain.Errors;
using Microsoft.EntityFrameworkCore;


namespace Loans.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public LoanService(IRepositoryManager repo, IMapper mapper, ILoggerManager logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LoanDto> CreateLoanAsync(Guid userId, LoanCreateDto dto)
        {
            var user = await _repo.User.GetByIdAsync(userId, true)
                    ?? throw new NotFoundException("User not found", ErrorCodes.USER_NOT_FOUND);
            if (user.IsBlocked) throw new ForbiddenException("User is blocked from creating loans", ErrorCodes.USER_BLOCKED);

            var loan = new Loan
            {
                UserId = userId,
                LoanType = dto.LoanType,
                Amount = dto.Amount,
                Currency = dto.Currency,
                PeriodMonths = dto.PeriodMonths,
                Status = LoanStatus.InProcess
            };

            _repo.Loan.Create(loan);
            await _repo.SaveAsync();

            _logger.LogInfo($"Loan created: {loan.Id} by user {userId}");

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetLoansForUserAsync(Guid userId)
        {
            var loans = await _repo.Loan.FindByCondition(l => l.UserId == userId, false).ToListAsync();
            return loans.Select(l => _mapper.Map<LoanDto>(l)).ToList();
        }
        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _repo.Loan.FindAll(false).ToListAsync();
            return loans.Select(l => _mapper.Map<LoanDto>(l)).ToList();
        }
        public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId)
        {
            var user = await _repo.User.GetByIdAsync(userId, false)
                ?? throw new NotFoundException("User not found.", ErrorCodes.USER_NOT_FOUND);
            var loans = await _repo.Loan.FindByCondition(l => l.UserId == userId, false).ToListAsync();
            return loans.Select(l => _mapper.Map<LoanDto>(l)).ToList();
        }

        public async Task<LoanDto> GetLoanByIdAsync(Guid loanId, Guid callerUserId, bool isAccountant)
        {
            var loan = await _repo.Loan.GetLoanByIdAsync(loanId, false)
                ?? throw new NotFoundException("Loan not found.", ErrorCodes.LOAN_NOT_FOUND);
            if (!isAccountant && loan.UserId != callerUserId)
                throw new ForbiddenException("You are not allowed to access this loan.", ErrorCodes.LOAN_FORBIDDEN);

            return _mapper.Map<LoanDto>(loan);
        }
        public async Task UpdateLoanAsync(Guid loanId, Guid callerUserId, LoanUpdateDto dto, bool isAccountant)
        {
            var loan = await _repo.Loan.GetLoanByIdAsync(loanId, true)
                    ?? throw new NotFoundException("Loan not found", ErrorCodes.LOAN_NOT_FOUND);

            if (!isAccountant)
            {
                if (loan.UserId != callerUserId) throw new ForbiddenException("You are not allowed to update this loan", ErrorCodes.LOAN_UPDATE_FORBIDDEN);
                if (loan.Status != LoanStatus.InProcess) throw new BadRequestException("Only loans in process can be updated", ErrorCodes.BAD_REQUEST);
            }

            _mapper.Map(dto, loan);
            _repo.Loan.Update(loan);
            await _repo.SaveAsync();

            _logger.LogInfo($"Loan updated: {loan.Id} by {(isAccountant ? "accountant" : callerUserId.ToString())}");
        }

        public async Task DeleteLoanAsync(Guid loanId, Guid callerUserId, bool isAccountant)
        {
            var loan = await _repo.Loan.GetLoanByIdAsync(loanId, true)
                ?? throw new NotFoundException("Loan not found", ErrorCodes.LOAN_NOT_FOUND);
            if (!isAccountant)
            {
                if (loan.UserId != callerUserId) throw new ForbiddenException("Access denied", ErrorCodes.LOAN_DELETE_FORBIDDEN);
                if (loan.Status != LoanStatus.InProcess) throw new BadRequestException("Only loans in process can be deleted", ErrorCodes.BAD_REQUEST);
            }

            _repo.Loan.Delete(loan);
            await _repo.SaveAsync();
            _logger.LogInfo($"Loan deleted: {loan.Id} by {(isAccountant ? "accountant" : callerUserId.ToString())}");
        }

        public async Task OverrideLoanStatusAsync(Guid loanId, LoanStatus status)
        {
            var loan = await _repo.Loan.GetLoanByIdAsync(loanId, true)
                ?? throw new NotFoundException("Loan not found", ErrorCodes.LOAN_NOT_FOUND);
            loan.Status = status;
            _repo.Loan.Update(loan);
            await _repo.SaveAsync();

            _logger.LogInfo($"Loan {loan.Id} status changed to {status}");
        }
    }
}

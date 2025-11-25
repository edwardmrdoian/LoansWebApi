namespace Loans.Domain.Dtos
{
    public record LoanCreateDto(LoanType LoanType, decimal Amount, string Currency, int PeriodMonths);
    public record LoanUpdateDto(decimal? Amount, int? PeriodMonths);
    public record LoanStatusUpdateDto(LoanStatus Status);
    public record LoanDto(Guid Id, LoanType LoanType, decimal Amount, string Currency, int PeriodMonths, LoanStatus Status, Guid UserId);
}

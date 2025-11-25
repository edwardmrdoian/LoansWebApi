using FluentValidation;
using Loans.Domain.Dtos;

namespace Loans.Application.Validators
{
    public class LoanUpdateDtoValidator : AbstractValidator<LoanUpdateDto>
    {
        public LoanUpdateDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).When(x => x.Amount.HasValue);
            RuleFor(x => x.PeriodMonths).GreaterThan(0).When(x => x.PeriodMonths.HasValue);
        }
    }
}

using FluentValidation;
using Loans.Domain.Dtos;

namespace Loans.Application.Validators
{
    public class LoanCreateDtoValidator : AbstractValidator<LoanCreateDto>
    {
        public LoanCreateDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0");
            RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter code");
            RuleFor(x => x.PeriodMonths).GreaterThan(0).LessThanOrEqualTo(360);
            RuleFor(x => x.LoanType).IsInEnum();
        }
    }
}

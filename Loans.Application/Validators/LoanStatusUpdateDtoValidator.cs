using FluentValidation;
using Loans.Domain.Dtos;

namespace Loans.Application.Validators
{
    public class LoanStatusUpdateDtoValidator : AbstractValidator<LoanStatusUpdateDto>
    {
        public LoanStatusUpdateDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}

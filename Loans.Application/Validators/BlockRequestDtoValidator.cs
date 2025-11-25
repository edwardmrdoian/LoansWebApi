using FluentValidation;
using Loans.Domain.Dtos;

namespace Loans.Application.Validators
{
    public class BlockRequestDtoValidator : AbstractValidator<BlockRequestDto>
    {
        public BlockRequestDtoValidator()
        {
            RuleFor(x => x.Days).GreaterThan(0).WithMessage("Block days must be greater than 0");
        }
    }
}

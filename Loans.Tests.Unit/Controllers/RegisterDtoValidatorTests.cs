using FluentAssertions;
using FluentValidation.TestHelper;
using Loans.Application.Validators;
using Loans.Domain.Dtos;
using Xunit;

namespace Loans.Tests.Unit.Controllers
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        [Fact]
        public void Validate_EmptyModel_HasErrors()
        {
            var dto = new RegisterDto(
                FirstName: "",
                LastName: "",
                Username: "",
                Email: "",
                Password: "",
                Age: 0,
                MonthlyIncome: 0
            );
            var res = _validator.TestValidate(dto);
            res.ShouldHaveValidationErrorFor(x => x.FirstName);
            res.ShouldHaveValidationErrorFor(x => x.Password);
            res.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_ValidModel_NoErrors()
        {
            var dto = new RegisterDto(
                FirstName: "F",
                LastName: "L",
                Username: "user1",
                Email: "e@e.com",
                Password: "Password1!",
                Age: 25,
                MonthlyIncome: 1000
            );

            var res = _validator.TestValidate(dto);
            res.IsValid.Should().BeTrue();
        }
    }
}

using FluentAssertions;
using Loans.Api.Controllers;
using Loans.Application.Interfaces;
using Loans.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Loans.Tests.Unit.Controllers
{
    public class LoanControllerTests
    {
        private static ControllerContext CreateControllerContext(Guid userId, string role = "User")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var loanMock = new Mock<ILoanService>();
            var loan = new LoanDto
            (
                Id: Guid.NewGuid(),
                LoanType: Domain.LoanType.FastLoan,
                Amount: 100,
                Currency: "USD",
                PeriodMonths: 12,
                Status: Domain.LoanStatus.InProcess,
                UserId: Guid.NewGuid()
            );
            loanMock.Setup(l => l.CreateLoanAsync(It.IsAny<Guid>(), It.IsAny<LoanCreateDto>())).ReturnsAsync(loan);

            var ctrl = new LoansController(loanMock.Object);
            ctrl.ControllerContext = CreateControllerContext(Guid.NewGuid());

            var res = await ctrl.CreateLoan(new LoanCreateDto(Domain.LoanType.FastLoan, 100, "USD", 12));

            res.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task GetMyLoans_ReturnsOk()
        {
            var loanMock = new Mock<ILoanService>();
            loanMock.Setup(l => l.GetLoansForUserAsync(It.IsAny<Guid>())).ReturnsAsync(new List<LoanDto>());

            var ctrl = new LoansController(loanMock.Object);
            ctrl.ControllerContext = CreateControllerContext(Guid.NewGuid());

            var res = await ctrl.GetMyLoans();

            res.Should().BeOfType<OkObjectResult>();
        }
    }
}

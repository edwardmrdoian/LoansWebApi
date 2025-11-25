using FluentAssertions;
using Loans.Application.Services;
using Loans.Domain.Dtos;
using Loans.Domain.Entities;
using Loans.Tests.Unit.Helpers;
using Moq;
using Xunit;

namespace Loans.Tests.Unit.Services
{
    public class LoanServiceTests
    {
        [Fact]
        public async Task CreateLoanAsync_UserBlocked_ThrowsForbidden()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User { Id = Guid.NewGuid(), IsBlocked = true };
            repoMock.Setup(r => r.User.GetByIdAsync(user.Id, false)).ReturnsAsync(user);

            var svc = new LoanService(repoMock.Object, mapperMock.Object, loggerMock.Object);

            var dto = new LoanCreateDto(Domain.LoanType.FastLoan, 100, "USD", 12);

            await svc.Invoking(s => s.CreateLoanAsync(user.Id, dto))
                .Should().ThrowAsync<Loans.Application.Exceptions.ForbiddenException>();
        }

        [Fact]
        public async Task CreateLoanAsync_Valid_CreatesLoan()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User { Id = Guid.NewGuid(), IsBlocked = false };
            repoMock.Setup(r => r.User.GetByIdAsync(user.Id, false)).ReturnsAsync(user);

            Loan createdLoan = null!;
            repoMock.Setup(r => r.Loan.Create(It.IsAny<Loan>()))
                .Callback<Loan>(l => createdLoan = l);

            var svc = new LoanService(repoMock.Object, mapperMock.Object, loggerMock.Object);

            var dto = new LoanCreateDto(Domain.LoanType.FastLoan, 500, "USD", 12);

            var result = await svc.CreateLoanAsync(user.Id, dto);

            repoMock.Verify(r => r.Loan.Create(It.IsAny<Loan>()), Times.Once);
            repoMock.Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}

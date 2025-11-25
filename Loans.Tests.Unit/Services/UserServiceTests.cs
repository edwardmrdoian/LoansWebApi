using FluentAssertions;
using Loans.Application.Services;
using Loans.Domain.Entities;
using Loans.Tests.Unit.Helpers;
using Moq;
using Xunit;

namespace Loans.Tests.Unit.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task BlockUserAsync_UserNotFound_ThrowsNotFound()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            repoMock.Setup(r => r.User.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync((User)null);

            var svc = new UserService(repoMock.Object, mapperMock.Object, loggerMock.Object);

            await svc.Invoking(s => s.BlockUserAsync(Guid.NewGuid(), 10))
                .Should().ThrowAsync<Loans.Application.Exceptions.NotFoundException>();
        }

        [Fact]
        public async Task BlockUserAsync_UserAlreadyBlocked_ThrowsBadRequest()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User { Id = Guid.NewGuid(), Username = "u", IsBlocked = true };

            repoMock.Setup(r => r.User.GetByIdAsync(user.Id, true)).ReturnsAsync(user);

            var svc = new UserService(repoMock.Object, mapperMock.Object, loggerMock.Object);

            await svc.Invoking(s => s.BlockUserAsync(user.Id, 5))
                .Should().ThrowAsync<Loans.Application.Exceptions.BadRequestException>();
        }

        [Fact]
        public async Task BlockUserAsync_SetsBlocked()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User { Id = Guid.NewGuid(), Username = "u", IsBlocked = false };

            repoMock.Setup(r => r.User.GetByIdAsync(user.Id, true)).ReturnsAsync(user);

            var svc = new UserService(repoMock.Object, mapperMock.Object, loggerMock.Object);

            await svc.BlockUserAsync(user.Id, 7);

            repoMock.Verify(r => r.User.Update(It.Is<User>(x => x.IsBlocked)), Times.Once);
            repoMock.Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}

using AutoFixture;
using FluentAssertions;
using Loans.Application.Services;
using Loans.Application.Settings;
using Loans.Domain.Dtos;
using Loans.Domain.Entities;
using Loans.Tests.Unit.Helpers;
using Moq;

using Xunit;

namespace Loans.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private readonly IFixture _fixture = AutoFixtureExtension.CreateFixture();

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsNotFoundException()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            repoMock.Setup(r => r.User.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((User)null);

            var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtSettings { Secret = "abc", Issuer = "i", Audience = "a", ExpiryMinutes = 60 });

            var svc = new AuthService(repoMock.Object, mapperMock.Object, (Microsoft.Extensions.Options.IOptions<JwtSettings>)loggerMock.Object, (Contracts.ILoggerManager)jwtOptions);

            var dto = new LoginDto("noexist", "pass");

            await svc.Invoking(s => s.LoginAsync(dto))
                .Should().ThrowAsync<Loans.Application.Exceptions.NotFoundException>()
                .WithMessage("*not found*");
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsBadRequest()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User { Id = Guid.NewGuid(), Username = "u", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") };

            repoMock.Setup(r => r.User.GetByUsernameAsync("u", false)).ReturnsAsync(user);

            var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtSettings { Secret = "abc", Issuer = "i", Audience = "a", ExpiryMinutes = 60 });

            var svc = new AuthService(repoMock.Object, mapperMock.Object, (Microsoft.Extensions.Options.IOptions<JwtSettings>)loggerMock.Object, (Contracts.ILoggerManager)jwtOptions);

            var dto = new LoginDto("u", "wrong");

            await svc.Invoking(s => s.LoginAsync(dto))
                .Should().ThrowAsync<Loans.Application.Exceptions.BadRequestException>()
                .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenAndUser()
        {
            var (repoMock, mapperMock, loggerMock) = MocksFactory.CreateCoreMocks();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "u",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret"),
                FirstName = "F",
                LastName = "L",
                Age = 20,
                MonthlyIncome = 1000,
                Email = "e@e.com",
                IsBlocked = false,
                Role = Loans.Domain.Role.User
            };

            repoMock.Setup(r => r.User.GetByUsernameAsync("u", false)).ReturnsAsync(user);

            var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtSettings { Secret = "ThisIsASecretForTests1234567890", Issuer = "test", Audience = "test", ExpiryMinutes = 60 });

            var svc = new AuthService(repoMock.Object, mapperMock.Object, (Microsoft.Extensions.Options.IOptions<JwtSettings>)loggerMock.Object, (Contracts.ILoggerManager)jwtOptions);

            var dto = new LoginDto("u", "secret");

            var result = await svc.LoginAsync(dto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrWhiteSpace();
            result.User.Username.Should().Be("u");
        }
    }
}

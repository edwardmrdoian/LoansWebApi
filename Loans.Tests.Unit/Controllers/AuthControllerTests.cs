using FluentAssertions;
using Loans.Api.Controllers;
using Loans.Application.Interfaces;
using Loans.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Loans.Tests.Unit.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsCreated()
        {
            var authMock = new Mock<IAuthService>();
            var created = new UserDto(Guid.NewGuid(), "F", "L", "u", 20, 1000, "e", false, "User");
            authMock.Setup(a => a.RegisterAsync(It.IsAny<RegisterDto>())).ReturnsAsync(created);

            var ctrl = new AuthController(authMock.Object);

            var res = await ctrl.Register(new RegisterDto("F", "L", "u", "e@e.com", "P@ssw0rd", 20, 1000));

            res.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Login_ReturnsOk()
        {
            var authMock = new Mock<IAuthService>();
            var dto = new AuthResponseDto("token", new UserDto(Guid.NewGuid(), "F", "L", "u", 20, 1000, "e", false, "User"));
            authMock.Setup(a => a.LoginAsync(It.IsAny<LoginDto>())).ReturnsAsync(dto);

            var ctrl = new AuthController(authMock.Object);

            var res = await ctrl.Login(new LoginDto("u", "p"));

            res.Should().BeOfType<OkObjectResult>();
        }
    }
}

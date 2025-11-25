using FluentAssertions;
using Loans.Api.Middlewares;
using Loans.Application.Exceptions;
using Loans.Contracts;
using Loans.Domain.Errors;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text.Json;
using Xunit;

namespace Loans.Tests.Unit.Middleware
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ApiException_ReturnsStructuredError()
        {
            var loggerMock = new Mock<ILoggerManager>();

            RequestDelegate throwingDelegate = (HttpContext ctx) =>
            {
                throw new NotFoundException("User not found", ErrorCodes.USER_NOT_FOUND);
            };

            var middleware = new ExceptionMiddleware(throwingDelegate, loggerMock.Object);

            var context = new DefaultHttpContext();
            var stream = new MemoryStream();
            context.Response.Body = stream;

            await middleware.InvokeAsync(context);

            stream.Position = 0;
            var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();

            var error = JsonSerializer.Deserialize<ErrorResponse>(text);
            error.Should().NotBeNull();
            error!.ErrorCode.Should().Be(ErrorCodes.USER_NOT_FOUND);
            context.Response.StatusCode.Should().Be(404);
        }
    }
}

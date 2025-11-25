using FluentValidation;
using Loans.Application.Exceptions;
using Loans.Contracts;
using Loans.Domain.Errors;
using System.Net;
using System.Text.Json;

namespace Loans.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unhandled exception: {ex.Message}", ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ErrorResponse response = new();
            int status = (int)HttpStatusCode.InternalServerError;

            switch (ex)
            {
                case ApiException e:
                    response.ErrorCode = e.ErrorCode;
                    response.Message = e.Message;
                    response.StatusCode = status = ex switch
                    {
                        NotFoundException => (int)HttpStatusCode.NotFound,
                        BadRequestException => (int)HttpStatusCode.BadRequest,
                        ForbiddenException => (int)HttpStatusCode.Forbidden,
                        UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                        _ => (int)HttpStatusCode.BadRequest
                    };
                    break;

                case ValidationException fv:
                    response.ErrorCode = ErrorCodes.VALIDATION_ERROR;
                    response.Message = "Validation failed";
                    response.Details = fv.Errors.Select(e => e.ErrorMessage).ToList();
                    response.StatusCode = status = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentException argEx:
                    response.ErrorCode = ErrorCodes.BAD_REQUEST;
                    response.Message = argEx.Message;
                    response.StatusCode = status = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.ErrorCode = ErrorCodes.INTERNAL_ERROR;
                    response.Message = "An unexpected error occurred.";
                    response.StatusCode = status;
                    break;
            }

            var logMsg = $"[ERROR] {context.Request.Method} {context.Request.Path} - {ex.Message}";
            _logger.LogError($"{logMsg}{Environment.NewLine}{ex}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}

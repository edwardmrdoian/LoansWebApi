using Loans.Domain.Errors;

namespace Loans.Application.Exceptions
{
    public sealed class BadRequestException : ApiException
    {
        public BadRequestException(string message, string code = ErrorCodes.BAD_REQUEST)
            : base(message, code) { }
    }
}

using Loans.Domain.Errors;

namespace Loans.Application.Exceptions
{
    public sealed class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message, string code = ErrorCodes.UNAUTHORIZED)
            : base(message, code) { }
    }
}

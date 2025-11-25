using Loans.Domain.Errors;

namespace Loans.Application.Exceptions
{
    public sealed class ForbiddenException : ApiException
    {
        public ForbiddenException(string message, string code = ErrorCodes.LOAN_FORBIDDEN)
        : base(message, code) { }
    }
}

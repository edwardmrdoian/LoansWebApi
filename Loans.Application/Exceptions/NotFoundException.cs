using Loans.Domain.Errors;

namespace Loans.Application.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message, string code = ErrorCodes.LOAN_NOT_FOUND)
       : base(message, code) { }
    }
}

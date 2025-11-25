namespace Loans.Application.Exceptions
{
    public abstract class ApiException : Exception
    {
        public string ErrorCode { get; }
        public ApiException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

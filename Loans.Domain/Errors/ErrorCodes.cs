namespace Loans.Domain.Errors
{
    public static class ErrorCodes
    {
        public const string INTERNAL_ERROR = "INTERNAL_ERROR";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string USER_BLOCKED = "USER_BLOCKED";
        public const string LOAN_NOT_FOUND = "LOAN_NOT_FOUND";
        public const string LOAN_FORBIDDEN = "LOAN_FORBIDDEN";
        public const string LOAN_UPDATE_FORBIDDEN = "LOAN_UPDATE_FORBIDDEN";
        public const string LOAN_DELETE_FORBIDDEN = "LOAN_DELETE_FORBIDDEN";
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string BAD_REQUEST = "BAD_REQUEST";
    }
}

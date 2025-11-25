namespace Loans.Domain.Errors
{
    public sealed class ErrorResponse
    {
        public string ErrorCode { get; set; } = "INTERNAL_ERROR";
        public string Message { get; set; } = "An unexpected error occurred.";
        public int StatusCode { get; set; }
        public IEnumerable<string>? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

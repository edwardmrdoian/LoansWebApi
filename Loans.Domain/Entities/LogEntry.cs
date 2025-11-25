namespace Loans.Domain.Entities
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Logger { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
    }
}

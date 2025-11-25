using System.ComponentModel.DataAnnotations.Schema;

namespace Loans.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public LoanType LoanType { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GEL";
        public int PeriodMonths { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.InProcess;

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}

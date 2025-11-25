using System.ComponentModel.DataAnnotations.Schema;

namespace Loans.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public int Age { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal MonthlyIncome { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsBlocked { get; set; } = false;
        public Role Role { get; set; } = Role.User;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}

using Loans.Domain;
using Loans.Domain.Entities;
using Loans.Infrastructure;

namespace Loans.Tests.Integration.TestHelpers
{
    public static class DbSeeder
    {
        public static void Seed(RepositoryContext context)
        {
            context.Users.RemoveRange(context.Users);
            context.Loans.RemoveRange(context.Loans);
            context.SaveChanges();

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Username = "john",
                Email = "john@example.com",
                Age = 30,
                MonthlyIncome = 2000,
                IsBlocked = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = Role.User
            };

            var acc = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Acc",
                LastName = "Ount",
                Username = "accountant",
                Email = "acc@example.com",
                Age = 35,
                MonthlyIncome = 4000,
                IsBlocked = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = Role.Accountant
            };

            context.Users.AddRange(user1, acc);

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                LoanType = LoanType.FastLoan,
                Amount = 1000,
                Currency = "USD",
                PeriodMonths = 12,
                Status = LoanStatus.InProcess,
            };

            context.Loans.Add(loan);
            context.SaveChanges();
        }
    }
}

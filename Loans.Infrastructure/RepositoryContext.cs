using Loans.Domain;
using Loans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loans.Infrastructure
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<LogEntry> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Loans)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var acctId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var acct = new User
            {
                Id = acctId,
                FirstName = "System",
                LastName = "Accountant",
                Username = "accountant1",
                Email = "accountant@example.com",
                Age = 30,
                MonthlyIncome = 0m,
                PasswordHash = BC.EnhancedHashPassword("Accountant123", 13),
                IsBlocked = false,
                Role = Role.Accountant
            };

            modelBuilder.Entity<User>().HasData(acct);

            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("Logs");

                entity.HasKey(l => l.Id);

                entity.Property(l => l.Level)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(l => l.Logger)
                      .HasMaxLength(250)
                      .IsRequired();

                entity.Property(l => l.Message)
                      .IsRequired();

                entity.Property(l => l.Exception)
                      .HasColumnType("nvarchar(max)")
                      .IsRequired(false);
            });
        }
    }
}

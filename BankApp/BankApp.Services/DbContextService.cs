using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class DbContextService:DbContext
    {
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Clerk> Clerks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-AALRII2;Initial Catalog=BankDB;Integrated Security=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasKey(id => id.BankId);
                entity.HasMany(bank => bank.Accounts);
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(id => id.AccountId);
                entity.HasMany(accnt => accnt.Transactions);
            });
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(id => id.TransactionId);

            });
            modelBuilder.Entity<Clerk>(entity =>
            {
                entity.HasKey(id => id.ClerkId);
                entity.HasOne(clerk => clerk.Bank).WithMany(bank => bank.Clerks);
            });
        }
    }
}

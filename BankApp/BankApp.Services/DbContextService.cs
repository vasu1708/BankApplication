using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class DbContextService:DbContext
    {
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;Database=BankDatabase;UID=srinivas,Password=Mysql@1234");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasKey(id => id.BankId);
                entity.HasMany(bank => bank.Accounts).WithOne();
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(id => id.AccountId);
                entity.HasMany(accnt => accnt.Transactions).WithOne();
            });
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(id => id.TransactionId);

            });
        }
    }
}

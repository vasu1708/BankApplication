using Microsoft.EntityFrameworkCore;

namespace BankApp.Models
{
    internal class BankDBContext:DbContext
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
                entity.Property(name => name.BankName).IsRequired();
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(id => id.AccountId);
                entity.Property(name => name.AccountHolderName).IsRequired();
            });
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(id => id.TransactionId);
                entity.Property(senderId => senderId.SenderAccountId).IsRequired();
            });
        }
    }
}

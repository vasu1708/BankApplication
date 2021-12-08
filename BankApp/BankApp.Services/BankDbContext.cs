using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class BankDbContext:DbContext
    {
        public DbSet<Models.Bank> Banks;
        public DbSet<Models.Account> Accounts;
        public DbSet<Models.Transaction> Transactions;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=DESKTOP-AALRII2;Initial Catalog=BankDB;Integrated Security=True");
        }
    }
}

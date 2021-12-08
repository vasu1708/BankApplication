using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Models;

namespace BankApp.Services
{
    internal class ClerkService
    {
        public static string GetAccountNumber()
        {
            Random random = new Random();
            uint AccountNo = (UInt32)random.Next(111111111, 999999999);
            return AccountNo.ToString();
        }
        public string GetAccountId(string accountNumber)
        {
            
            BankDbContext context = new BankDbContext();
            var accountId = context.Accounts.Single(x=>x.AccountNumber==accountNumber).AccountId;
            return accountId;
        }
        public bool IsAccountExist(string accountNumber)
        {
            BankDbContext context = new BankDbContext();
            if(new BankDbContext().Accounts.Any(x => x.AccountNumber == accountNumber))
                return true;
            return false;
        }
        public void CreateAccount(string name,string password,String address,Enums.Gender gender,DateOnly dob,string mobileNumber)
        {
            
            BankDbContext context = new BankDbContext();
            if (context.Accounts.Any(x => x.AccountHolderName == name))
                throw new Exception("Account already exist on this name");
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string accountId = $"{name.Substring(0, 3)}{date}";
            Account account = new Account()
            {
                AccountId = accountId,
                BankId = "ANDB",
                AccountHolderName = name,
                AccountNumber = GetAccountNumber(),
                AccountPassword = password,
                AccountBalance = 0,
                Address = address,
                Gender = gender,
                DateOfBirth = dob,
                MobileNumber = mobileNumber,
                Currency = Enums.CurrencyType.INR,
                AccountCreationDate = DateOnly.FromDateTime(DateTime.Now)
            };
            context.Accounts.Add(account);
            context.SaveChanges();
        }
        public void DeleteAccount(string accountNumber)
        {
            BankDbContext context = new BankDbContext();
            string accountId = GetAccountId(accountNumber);
            var account = context.Accounts.Single(x => x.AccountId == accountId);
            context.Accounts.Remove(account);
            context.SaveChanges();
        }
        public List<Transaction> TransactionHistory(string accountNumber)
        {
            BankDbContext context = new BankDbContext();
            string accountId = GetAccountId(accountNumber);
            return context.Transactions.Where(x => x.Accountid == accountId).ToList();
        }
        public void RevertTransaction(string transactionId)
        {
            BankDbContext context = new BankDbContext();
            var transaction = context.Transactions.Single(x => x.TransactionId == transactionId);
            if (transaction == null)
                throw new Exception("Invalid Transaction");
            context.Transactions.Remove(transaction);
        }
    }
}

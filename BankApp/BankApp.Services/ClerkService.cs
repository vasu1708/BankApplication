using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Models;

namespace BankApp.Services
{
    public class ClerkService
    {
        public static string GetAccountNumber()
        {
            Random random = new Random();
            uint AccountNo = (UInt32)random.Next(111111111, 999999999);
            return AccountNo.ToString();
        }
        public string GetAccountId(string accountNumber)
        {
            DbContextService context = new DbContextService();
            var accountId = context.Accounts.Single(x => x.AccountNumber == accountNumber).AccountId;
            return accountId;
        }
        public string IsAccountExist(string accountNumber)
        {
            var accountId = GetAccountId(accountNumber);
            if(accountId == null)
                throw new Exception("Account not Exist");
            return accountId;
        }
        public string CreateAccount(string bankName,string name,string password,string address,Enums.Gender gender,string dob,string mobileNumber)
        {
            
            DbContextService context = new DbContextService();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string accountId = $"{name.Substring(0, 3)}{date}";
            string accountNumber = GetAccountNumber();
            Bank bank = context.Banks.Single(bank => bank.BankName == bankName);
            context.Accounts.Add(new Account()
            {
                AccountId = accountId,
                AccountHolderName = name,
                AccountNumber = accountNumber,
                AccountPassword = password,
                AccountBalance = 0,
                Address = address,
                Gender = gender,
                DateOfBirth = dob,
                MobileNumber = mobileNumber,
                Currency = Enums.CurrencyType.INR,
                AccountCreationDate = DateTime.Now.Date,
                Bank = bank,
                Transactions = new List<Transaction>()
            });
            context.SaveChanges();
            return accountNumber;
        }
        public void DeleteAccount(string accountNumber)
        {
            DbContextService context = new DbContextService();
            string accountId = IsAccountExist(accountNumber);
            var account = context.Accounts.Single(a => a.AccountId == accountId);
            context.Accounts.Remove(account);
            context.SaveChanges();
        }
        public void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            DbContextService context = new DbContextService();
            string accountId = IsAccountExist(accountNumber);
            context.Accounts.Single(a => a.AccountId == accountId).MobileNumber = mobileNumber;
            context.SaveChanges();
        }
        public void UpdateAddress(string accountNumber,string address)
        {
            DbContextService context = new DbContextService();
            string accountId = IsAccountExist(accountNumber);
            context.Accounts.Single(a => a.AccountId == accountId).Address = address;
            context.SaveChanges();
        }
        public void UpdateCurrency(string accountNumber,Enums.CurrencyType currency)
        {
            DbContextService context = new DbContextService();
            string accountId = IsAccountExist(accountNumber);
            Account account = context.Accounts.Single(a => a.AccountId == accountId);
           if( account.Currency == currency)
                throw new Exception("Oops! it is same as earlier");
            switch (currency)
            {
                case Enums.CurrencyType.INR:
                    account.AccountBalance *= 70;
                    break;
                case Enums.CurrencyType.INUSD:
                    account.AccountBalance /= 70;
                    break;
            }
            context.SaveChanges();
        }
        public List<string> TransactionHistory(string accountNumber)
        {
            List<string> transactionList = new List<string>();
            DbContextService context = new DbContextService();
            string accountId = IsAccountExist(accountNumber);
            var txns = context.Transactions.Where(a => a.AccountId == accountId).ToList();
            string transaction;
            foreach(var txn in txns)
            {
                transaction = $"{txn.TransactionId} {txn.SenderAccountId} {txn.ReceiverAccountId} {txn.TransactionType} {txn.Amount}({txn.Currency}) {txn.TimeOfTransaction}";
                transactionList.Add(transaction);
            }
            return transactionList;
        }
        public decimal GetAccountBalance(string accountNumber)
        {
            return new DbContextService().Accounts.Single(a => a.AccountId == IsAccountExist(accountNumber)).AccountBalance;
        }
        public Enums.CurrencyType GetCurrencyType(string accountNumber)
        {
            return new DbContextService().Accounts.Single(a => a.AccountId == IsAccountExist(accountNumber)).Currency;
        }
        public void PerformTransaction(string accountId,string senderId, string receiverId, Enums.TransactionType transactionType, decimal amount)
        {
            DbContextService context = new DbContextService();
            Account account = context.Accounts.Single(account => account.AccountId == accountId);
            if (transactionType == Enums.TransactionType.CREDIT)
                account.AccountBalance += amount;
            else
            {
                if (account.AccountBalance < amount)
                    throw new Exception("Insufficient Balance!");
                account.AccountBalance -= amount;
            }
            string date = DateTime.Now.ToString("dd-mm-yyyy");
            context.Transactions.Add(new Transaction()
            {
                TransactionId = $"TXN{account.BankId}{senderId}{date}",
                SenderAccountId = senderId,
                ReceiverAccountId = receiverId,
                Amount = amount,
                TransactionType = transactionType,
                TimeOfTransaction = DateTime.Now,
                Currency = account.Currency,
                Account = account
            });
            
            context.SaveChanges();
        }
        public void RevertTransaction(string accountNumber,string transactionId)
        {
            var accountId = IsAccountExist(accountNumber);
            DbContextService context = new DbContextService();
            var transaction = context.Transactions.Single(x => x.TransactionId == transactionId);
            if (transaction == null)
                throw new Exception("Invalid Transaction");
            context.Transactions.Remove(transaction);
            context.SaveChanges();
        }
        public bool VerifyPassword(string id, string password)
        {
            return new DbContextService().Clerks.Single(Clerk => Clerk.ClerkId == id).Password == password;
        }
        public void UpdateCharges(string bankName,Enums.ChargeType type,decimal charge)
        {
            DbContextService context = new DbContextService();
            string bankId = new CustomerService().GetBankId(bankName);
            Bank bank = context.Banks.Single(b => b.BankId == bankId);
            switch (type)
            {
                case Enums.ChargeType.SameBankIMPS:
                    bank.SameBankIMPS = charge;
                    break;
                case Enums.ChargeType.OtherBankIMPS:
                    bank.OtherBankIMPS = charge;
                    break;
                case Enums.ChargeType.SameBankRTGS:
                    bank.SameBankRTGS = charge;
                    break;
                default:
                    bank.OtherBankRTGS = charge;
                    break;
            }
            context.SaveChanges();
        }
    }
}

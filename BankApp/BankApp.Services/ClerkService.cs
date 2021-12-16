﻿using System;
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
        public Account GetAccount(string accountNumber)
        {
            DbContextService context = new DbContextService();
            var account = context.Accounts.Single(x => x.AccountNumber == accountNumber);
            return account;
        }
        public Account IsAccountExist(string accountNumber)
        {
            var account = GetAccount(accountNumber);
            if(account == null)
                throw new Exception("Account not Exist");
            return account;
        }
        public string CreateAccount(string bankName,string name,string password,string address,Enums.Gender gender,DateOnly dob,string mobileNumber)
        {
            
            DbContextService context = new DbContextService();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string accountId = $"{name.Substring(0, 3)}{date}";
            string accountNumber = GetAccountNumber();
            context.Accounts.Add( new Account()
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
                AccountCreationDate = DateOnly.FromDateTime(DateTime.Now),
                Bank = context.Banks.Single(bank => bank.BankName == bankName),
                Transactions = new List<Transaction>()
            });
            context.SaveChanges();
            return accountNumber;
        }
        public void DeleteAccount(string accountNumber)
        {
            DbContextService context = new DbContextService();
            var account = IsAccountExist(accountNumber);
            context.Accounts.Remove(account);
            context.SaveChanges();
        }
        public void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            var account = IsAccountExist(accountNumber);
            account.MobileNumber = mobileNumber;
            new DbContextService().SaveChanges();
        }
        public void UpdateAddress(string accountNumber,string address)
        {
            var account = IsAccountExist(accountNumber);
            account.Address = address;
            new DbContextService().SaveChanges();
        }
        public void UpdateCurrency(string accountNumber,Enums.CurrencyType currency)
        {
            var account = IsAccountExist(accountNumber);
            account.Currency = currency;
            new DbContextService().SaveChanges();
        }
        public List<string> TransactionHistory(string accountNumber)
        {
            List<string> transactionList = new List<string>();
            DbContextService context = new DbContextService();
            var account = context.Accounts.Single(accnt => accnt.AccountNumber == accountNumber);
            string transaction;
            foreach(var txn in account.Transactions)
            {
                transaction = @"{txn.TransactionId} {txn.SenderAccountId} {txn.ReceiverAccountId} {txn.TransactionType} {txn.Amount}";
                transactionList.Add(transaction);
            }
            return new List<string>();
        }
        public void PerformTransaction(string accountNumber,string senderId, string receiverId, Enums.TransactionType transactionType, decimal amount)
        {
            DbContextService context = new DbContextService();
            context.Transactions.Add(new Transaction
            {
                SenderAccountId = senderId,
                ReceiverAccountId = receiverId,
                Amount = amount,
                TransactionType = transactionType,
                TimeOfTransaction = DateTime.Now,
                Account = context.Accounts.Single(account => account.AccountNumber == accountNumber)
            }) ;
            context.SaveChanges();
        }
        public void RevertTransaction(string accountNumber,string transactionId)
        {
            var account = IsAccountExist(accountNumber);
            var transaction = account.Transactions.Single(x => x.TransactionId == transactionId);
            if (transaction == null)
                throw new Exception("Invalid Transaction");
            account.Transactions.Remove(transaction);
            new DbContextService().SaveChanges();

        }
        public bool VerifyPassword(string id, string password)
        {
            return new DbContextService().Clerks.Single(Clerk => Clerk.ClerkId == id).Password == password;
        }
        public void UpdateCharges(string id,Enums.ChargeType type,decimal charge)
        {
            DbContextService context = new DbContextService();
            Bank bank = context.Clerks.Single(clerk => clerk.ClerkId == id).Bank;
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

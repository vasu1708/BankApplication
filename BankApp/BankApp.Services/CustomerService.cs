using BankApp.Models;
using System;
using System.Collections.Generic;
namespace BankApp.Services
{
    public class CustomerService
    {
        //public static Dictionary<string, Bank> Banks = new Dictionary<string, Bank>();
        public string AddBank(string bankName,string clerkName,string password)
        {
            if (DatabaseService.IsBankExist(bankName))
                throw new Exception("Bank Name already exist");
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = $"{bankName.Substring(0, 3)}{Date}";
            decimal RTGSChargesSame = 5;
            decimal RTGSChargesOther = 0;
            decimal IMPSChargesSame = 2;
            decimal IMPSChargesOther = 6;
            DatabaseService.InsertIntoBank(bankName,BankId,RTGSChargesSame,RTGSChargesOther,IMPSChargesSame,IMPSChargesOther);
            //DatabaseConnectionService.AddClerk(BankId,clerkName,password);
            ClerkService clerk = new ClerkService();
            clerk.BankName = bankName;
            string Id = $"{clerkName}@{bankName}";
            clerk.ClerkId = Id;
            clerk.Password = password;
            ClerkService.Clerks[Id] = clerk;
            return Id;
        }   
        public void Deposit(string bankName,string accountNumber,decimal amount)
        {
            if (!DatabaseService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string TypeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            DatabaseService.UpdateAccountBalance(accountNumber,TypeOfTransaction,amount);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = DatabaseService.GetBankId(bankName);
            string AccountId = DatabaseService.GetAccountId(bankName,accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            DatabaseService.InsertTransaction(bankName,accountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TypeOfTransaction,amount,PointOfTime);
        }
        public void Withdraw(string bankName,string accountNumber,decimal amount,string password)
        {
            if (!DatabaseService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseService.VerifyAccountPassword(accountNumber,password))
                throw new Exception("Incorrect Password");
            if (DatabaseService.FetchAccountBalance(accountNumber) < amount)
                throw new Exception("Insufficient Balance");
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DatabaseService.UpdateAccountBalance(accountNumber,TypeOfTransaction,amount);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string BankId = DatabaseService.GetBankId(bankName);
            string AccountId = DatabaseService.GetAccountId(bankName, accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            DatabaseService.InsertTransaction(bankName,accountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount,PointOfTime);
        }
        public void Transfer(string senderBankName,string senderAccountNumber,string receiverBankName,string receiverAccountNumber,decimal amount,string password,Enums.TypeOfTransfer typeOfTransfer)
        {
            if (!DatabaseService.IsAccountExist(senderBankName, senderAccountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseService.IsAccountExist(receiverBankName, receiverAccountNumber))
                throw new Exception("The Account of receiver does not exist");
            if (!DatabaseService.VerifyAccountPassword(senderAccountNumber, password))
                throw new Exception("Incorrect Pin");
            
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string SenderBankId = DatabaseService.GetBankId(senderBankName);
            string SenderAccountId = DatabaseService.GetAccountId(senderBankName,senderAccountNumber);
            string TransactionId = $"TXN{SenderBankId}{SenderAccountId}{Date}";
            string ReceiverBankId = DatabaseService.GetBankId(senderBankName);
            string ReceiverAccountId = DatabaseService.GetAccountId(receiverBankName,receiverAccountNumber);
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DateTime PointOfTime = DateTime.Now;
            DatabaseService.UpdateAccountBalance(senderAccountNumber, "DEBIT", amount);
            DatabaseService.InsertTransaction(senderBankName, senderAccountNumber, TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            decimal Charges;
            if (senderBankName == receiverBankName)
            {
                if(typeOfTransfer == Enums.TypeOfTransfer.IMPS)
                    Charges = DatabaseService.GetImpsChargesForSameBank(SenderBankId) * amount / 100;
                else
                    Charges = DatabaseService.GetRtgsChargesForSameBank(SenderBankId) * amount / 100;
            }
            else
            {
                if(typeOfTransfer== Enums.TypeOfTransfer.IMPS)
                    Charges = DatabaseService.GetImpsChargesForOtherBank(SenderBankId) * amount / 100;
                else
                    Charges = DatabaseService.GetRtgsChargesForOtherBank(SenderBankId) * amount / 100;
                
            }
            amount -= Charges;
            DatabaseService.UpdateAccountBalance(receiverAccountNumber, "CREDIT", amount);
            DatabaseService.InsertTransaction(receiverBankName,receiverAccountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            DatabaseService.UpdateBankBalance(senderBankName,Charges);
        }
        public List<string> TransactionHistory(string bankName,string accountNumber,string password)
        {
            List<string> History = new List<string>();
            if (!DatabaseService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseService.VerifyAccountPassword(accountNumber,password))
                throw new Exception("Incorrect Password");
            History = DatabaseService.FetchAccountTransactions(accountNumber);
            return History;
        }


    }
}

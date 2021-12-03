using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Models;
namespace BankApp.Services
{
    public class CustomerService
    {
        //public static Dictionary<string, Bank> Banks = new Dictionary<string, Bank>();
        public string AddBank(string bankName,string clerkName,string password)
        {
            if (DatabaseConnectionService.IsBankExist(bankName))
                throw new Exception("Bank Name already exist");
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = $"{bankName.Substring(0, 3)}{Date}";
            decimal RTGSChargesSame = 5;
            decimal RTGSChargesOther = 0;
            decimal IMPSChargesSame = 2;
            decimal IMPSChargesOther = 6;
            DatabaseConnectionService.InsertIntoBank(bankName,BankId,RTGSChargesSame,RTGSChargesOther,IMPSChargesSame,IMPSChargesOther);
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
            if (!DatabaseConnectionService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string TypeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            DatabaseConnectionService.UpdateAccountBalance(accountNumber,TypeOfTransaction,amount);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = DatabaseConnectionService.GetBankId(bankName);
            string AccountId = DatabaseConnectionService.GetAccountId(bankName,accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            DatabaseConnectionService.InsertTransaction(bankName,accountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TypeOfTransaction,amount,PointOfTime);
        }
        public void Withdraw(string bankName,string accountNumber,decimal amount,string password)
        {
            if (!DatabaseConnectionService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseConnectionService.VerifyAccountPassword(accountNumber,password))
                throw new Exception("Incorrect Password");
            if (DatabaseConnectionService.FetchAccountBalance(accountNumber) < amount)
                throw new Exception("Insufficient Balance");
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DatabaseConnectionService.UpdateAccountBalance(accountNumber,TypeOfTransaction,amount);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string BankId = DatabaseConnectionService.GetBankId(bankName);
            string AccountId = DatabaseConnectionService.GetAccountId(bankName, accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            DatabaseConnectionService.InsertTransaction(bankName,accountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount,PointOfTime);
        }
        public void Transfer(string senderBankName,string senderAccountNumber,string receiverBankName,string receiverAccountNumber,decimal amount,string password,Enums.TypeOfTransfer typeOfTransfer)
        {
            if (!DatabaseConnectionService.IsAccountExist(senderBankName, senderAccountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseConnectionService.IsAccountExist(receiverBankName, receiverAccountNumber))
                throw new Exception("The Account of receiver does not exist");
            if (!DatabaseConnectionService.VerifyAccountPassword(senderAccountNumber, password))
                throw new Exception("Incorrect Pin");
            
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string SenderBankId = DatabaseConnectionService.GetBankId(senderBankName);
            string SenderAccountId = DatabaseConnectionService.GetAccountId(senderBankName,senderAccountNumber);
            string TransactionId = $"TXN{SenderBankId}{SenderAccountId}{Date}";
            string ReceiverBankId = DatabaseConnectionService.GetBankId(senderBankName);
            string ReceiverAccountId = DatabaseConnectionService.GetAccountId(receiverBankName,receiverAccountNumber);
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DateTime PointOfTime = DateTime.Now;
            DatabaseConnectionService.UpdateAccountBalance(senderAccountNumber, "DEBIT", amount);
            DatabaseConnectionService.InsertTransaction(senderBankName, senderAccountNumber, TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            decimal Charges;
            if (senderBankName == receiverBankName)
            {
                if(typeOfTransfer == Enums.TypeOfTransfer.IMPS)
                    Charges = DatabaseConnectionService.GetImpsChargesForSameBank(SenderBankId) * amount / 100;
                else
                    Charges = DatabaseConnectionService.GetRtgsChargesForSameBank(SenderBankId) * amount / 100;
            }
            else
            {
                if(typeOfTransfer== Enums.TypeOfTransfer.IMPS)
                    Charges = DatabaseConnectionService.GetImpsChargesForOtherBank(SenderBankId) * amount / 100;
                else
                    Charges = DatabaseConnectionService.GetRtgsChargesForOtherBank(SenderBankId) * amount / 100;
                
            }
            amount -= Charges;
            DatabaseConnectionService.UpdateAccountBalance(receiverAccountNumber, "CREDIT", amount);
            DatabaseConnectionService.InsertTransaction(receiverBankName,receiverAccountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            DatabaseConnectionService.UpdateBankBalance(senderBankName,Charges);
        }
        public List<string> TransactionHistory(string bankName,string accountNumber,string password)
        {
            List<string> History = new List<string>();
            if (!DatabaseConnectionService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            if (!DatabaseConnectionService.VerifyAccountPassword(accountNumber,password))
                throw new Exception("Incorrect Password");
            History = DatabaseConnectionService.FetchAccountTransactions(accountNumber);
            return History;
        }


    }
}

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
        public static Dictionary<string, Bank> Banks = new Dictionary<string, Bank>();
        public string AddBank(string bankName,string clerkName,string password)
        {
            if (Banks.ContainsKey(bankName))
                throw new Exception("Bank Name already exist");
            Bank bank = new Bank();
            bank.BankName = bankName;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            bank.BankId = $"{bankName.Substring(0, 3)}{Date}";
            bank.IMPSForSameBank = 5;
            bank.RTGSForSameBank = 0;
            bank.IMPSForOtherBank = 2;
            bank.RTGSForOtherBank = 6;
            bank.Currency = Enums.CurrencyType.INR; 
            Random random = new Random();
            bank.AccountNumberGenerator = (UInt32) random.Next(111111111, 999999999);
            bank.Accounts = new Dictionary<string,Account>();
            bank.Transactions = new Dictionary<string,Transaction>();
            ClerkService clerk = new ClerkService();
            clerk.BankName = bankName;
            clerk.ClerkId = $"{clerkName}@{bankName}".ToUpper();
            clerk.Password = password;
            ClerkService.Clerks[clerk.ClerkId] = clerk;
            Banks[bankName] = bank;
            return clerk.ClerkId;
        }   
        public void Deposit(string bankName,string accountNumber,decimal amount)
        {
            if (!Banks[bankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Account does not exist");
            Banks[bankName].Accounts[accountNumber].Balance += amount;

            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            Transaction transaction = new Transaction();
            transaction.TransactionId = $"TXN{Banks[bankName].BankId}{Banks[bankName].Accounts[accountNumber].AccountId}{Date}";
            transaction.SenderAccountId = "-";
            transaction.ReceiverAccountId = Banks[bankName].Accounts[accountNumber].AccountId;
            transaction.TypeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            transaction.TimeOfTransaction = PointOfTime;
            transaction.Amount = amount;
            Banks[bankName].Transactions.Add(transaction.TransactionId,transaction);
        }
        public void Withdraw(string bankName,string accountNumber,decimal amount,string password)
        {
            if (!Banks[bankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Acount does not exist");
            if (password != Banks[bankName].Accounts[accountNumber].Password)
                throw new Exception("Incorrect Password");
            if (Banks[bankName].Accounts[accountNumber].Balance < amount)
                throw new Exception("Insufficient Balance");
            Banks[bankName].Accounts[accountNumber].Balance -= amount;

            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            Transaction transaction = new Transaction();
            transaction.TransactionId = $"TXN{Banks[bankName].BankId}{Banks[bankName].Accounts[accountNumber].AccountId}{Date}";
            transaction.SenderAccountId = Banks[bankName].Accounts[accountNumber].AccountId;
            transaction.ReceiverAccountId = "-";
            transaction.TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            transaction.TimeOfTransaction = PointOfTime;
            transaction.Amount = amount;
            Banks[bankName].Transactions.Add(transaction.TransactionId,transaction);
        }
        public void Transfer(string senderBankName,string senderAccountNumber,string receiverBankName,string receiverAccountNumber,decimal amount,string password,Enums.TypeOfTransfer typeOfTransfer)
        {
            if (!Banks[senderBankName].Accounts.ContainsKey(senderAccountNumber))
                throw new Exception("Acount does not exist");
            if (!Banks[receiverBankName].Accounts.ContainsKey(receiverAccountNumber))
                throw new Exception("The Account of receiver does not exist");
            if (password != Banks[senderBankName].Accounts[senderAccountNumber].Password)
                throw new Exception("Incorrect Pin");
            Banks[senderBankName].Accounts[senderAccountNumber].Balance -= amount;           
            decimal Charges;
            if (senderBankName == receiverBankName)
            {
                if(typeOfTransfer == Enums.TypeOfTransfer.IMPS)
                    Charges = Banks[senderBankName].IMPSForSameBank * amount / 100;
                else
                    Charges = Banks[senderBankName].RTGSForSameBank * amount / 100;
            }
            else
            {
                if(typeOfTransfer== Enums.TypeOfTransfer.IMPS)
                    Charges = Banks[senderBankName].IMPSForOtherBank * amount / 100;
                else
                    Charges = Banks[senderBankName].RTGSForOtherBank * amount / 100;
                
            }
            Banks[senderBankName].Balance += Charges;
            amount -= Charges;
            Banks[receiverBankName].Accounts[receiverAccountNumber].Balance += amount;
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            Transaction senderTransaction = new Transaction();
            senderTransaction.TransactionId = $"TXN{Banks[senderBankName].BankId}{Banks[senderBankName].Accounts[senderAccountNumber].AccountId}{Date}";
            senderTransaction.SenderAccountId = Banks[senderBankName].Accounts[senderAccountNumber].AccountId;
            senderTransaction.ReceiverAccountId = Banks[receiverBankName].Accounts[receiverAccountNumber].AccountId;
            senderTransaction.TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            senderTransaction.TimeOfTransaction = PointOfTime;
            senderTransaction.Amount = amount;
            Banks[senderBankName].Transactions.Add(senderTransaction.TransactionId,senderTransaction);

            Transaction receiverTransaction = new Transaction();
            receiverTransaction.TransactionId = $"TXN{Banks[receiverBankName].BankId}{Banks[receiverBankName].Accounts[receiverAccountNumber].AccountId}{Date}";
            receiverTransaction.SenderAccountId = Banks[senderBankName].Accounts[senderAccountNumber].AccountId;
            receiverTransaction.ReceiverAccountId = Banks[receiverBankName].Accounts[receiverAccountNumber].AccountId;
            receiverTransaction.TypeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            receiverTransaction.TimeOfTransaction = PointOfTime;
            receiverTransaction.Amount = amount;
            Banks[receiverBankName].Transactions.Add(receiverTransaction.TransactionId,receiverTransaction);
        }
        public List<string> TransactionHistory(string bankName,string accountNumber,string password)
        {
            List<string> History = new List<string>();
            if (!Banks[bankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Acount does not exist");
            if (password != Banks[bankName].Accounts[accountNumber].Password)
                throw new Exception("Incorrect Password");
            foreach(var TxnKey in Banks[bankName].Transactions.Keys)
            {
                if(Banks[bankName].Transactions[TxnKey].SenderAccountId==Banks[bankName].Accounts[accountNumber].AccountId || Banks[bankName].Transactions[TxnKey].ReceiverAccountId== Banks[bankName].Accounts[accountNumber].AccountId)
                {
                    History.Add(TxnKey);
                }
            }
            return History;
        }

    }
}

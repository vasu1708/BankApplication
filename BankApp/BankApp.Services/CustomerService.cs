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
        public string AddBank(string BankName,string ClerkName,string Password)
        {
            if (Banks.ContainsKey(BankName))
                throw new Exception("Bank Name already exist");
            Bank bank = new Bank();
            bank.BankName = BankName;
            string Date = DateTime.Now.ToString("dd_MM_yyyy");
            bank.BankId = BankName.Substring(0, 3) + Date;
            bank.IMPSForSameBank = 5;
            bank.RTGSForSameBank = 0;
            bank.IMPSForOtherBank = 2;
            bank.RTGSForOtherBank = 6;
            Random random = new Random();
            bank.AccountNumberGenerator = (UInt32) random.Next(111111111, 999999999);
            bank.Accounts = new Dictionary<string,Account>();
            bank.Transactions = new List<Transaction>();
            ClerkService clerk = new ClerkService();
            clerk.BankName = BankName;
            clerk.ClerkId = $"{ClerkName}@{BankName}".ToUpper();
            clerk.Password = Password;
            ClerkService.Clerks[clerk.ClerkId] = clerk;
            Banks[BankName] = bank;
            return clerk.ClerkId;
        }   
        public void Deposit(string BankName,string AccountNumber,decimal Amount)
        {
            if (!Banks[BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Account does not exist");
            Banks[BankName].Accounts[AccountNumber].Balance += Amount;

            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd_MM_yyyy");
            Transaction transaction = new Transaction();
            transaction.TransactionId = $"TXN{Banks[BankName].BankId}{Banks[BankName].Accounts[AccountNumber].AccountId}{Date}";
            transaction.SenderAccountId = "-";
            transaction.ReceiverAccountId = Banks[BankName].Accounts[AccountNumber].AccountId;
            transaction.TypeOfTransaction = Enums.TransactionType.Credit.ToString();
            transaction.TimeOfTransaction = PointOfTime;
            transaction.Amount = Amount;
            Banks[BankName].Transactions.Add(transaction);
        }
        public void Withdraw(string BankName,string AccountNumber,decimal Amount,string Password)
        {
            if (!Banks[BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Acount does not exist");
            if (Password != Banks[BankName].Accounts[AccountNumber].Password)
                throw new Exception("Incorrect Password");
            if (Banks[BankName].Accounts[AccountNumber].Balance < Amount)
                throw new Exception("Insufficient Balance");
            Banks[BankName].Accounts[AccountNumber].Balance -= Amount;

            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd_MM_yyyy");
            Transaction transaction = new Transaction();
            transaction.TransactionId = $"TXN{Banks[BankName].BankId}{Banks[BankName].Accounts[AccountNumber].AccountId}{Date}";
            transaction.SenderAccountId = Banks[BankName].Accounts[AccountNumber].AccountId;
            transaction.ReceiverAccountId = "-";
            transaction.TypeOfTransaction = Enums.TransactionType.Debit.ToString();
            transaction.TimeOfTransaction = PointOfTime;
            transaction.Amount = Amount;
            Banks[BankName].Transactions.Add(transaction);
        }
        public void Transfer(string SenderBankName,string SenderAccountNumber,string ReceiverBankName,string ReceiverAccountNumber,decimal Amount,string Password,string TypeOfTransfer)
        {
            if (!Banks[SenderBankName].Accounts.ContainsKey(SenderAccountNumber))
                throw new Exception("Acount does not exist");
            if (Banks[ReceiverBankName].Accounts.ContainsKey(ReceiverAccountNumber))
                throw new Exception("Acount does not exist");
            if (Password != Banks[SenderBankName].Accounts[SenderAccountNumber].Password)
                throw new Exception("Incorrect Pin");
            Banks[SenderBankName].Accounts[SenderAccountNumber].Balance -= Amount;
            
            decimal Charges;
            if (SenderBankName == ReceiverBankName)
            {
                if(TypeOfTransfer == "IMPS")
                    Charges = Banks[SenderBankName].IMPSForSameBank * Amount / 100;
                else
                    Charges = Banks[SenderBankName].RTGSForSameBank * Amount / 100;
            }
            else
            {
                if(TypeOfTransfer=="IMPS")
                    Charges = Banks[SenderBankName].IMPSForOtherBank * Amount / 100;
                else
                    Charges = Banks[SenderBankName].RTGSForOtherBank * Amount / 100;
                
            }
            Banks[SenderBankName].Balance += Charges;
            Amount -= Charges;
            Banks[ReceiverBankName].Accounts[ReceiverAccountNumber].Balance += Amount;
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd_MM_yyyy");
            Transaction senderTransaction = new Transaction();
            senderTransaction.TransactionId = $"TXN{Banks[SenderBankName].BankId}{Banks[SenderBankName].Accounts[SenderAccountNumber].AccountId}{Date}";
            senderTransaction.SenderAccountId = "-";
            senderTransaction.ReceiverAccountId = Banks[ReceiverBankName].Accounts[ReceiverAccountNumber].AccountId;
            senderTransaction.TypeOfTransaction = Enums.TransactionType.Debit.ToString();
            senderTransaction.TimeOfTransaction = PointOfTime;
            senderTransaction.Amount = Amount;
            Banks[SenderBankName].Transactions.Add(senderTransaction);

            Transaction receiverTransaction = new Transaction();
            receiverTransaction.TransactionId = $"TXN{Banks[ReceiverBankName].BankId}{Banks[ReceiverBankName].Accounts[ReceiverAccountNumber].AccountId}{Date}";
            receiverTransaction.SenderAccountId = Banks[SenderBankName].Accounts[SenderAccountNumber].AccountId;
            receiverTransaction.ReceiverAccountId = "-";
            receiverTransaction.TypeOfTransaction = Enums.TransactionType.Credit.ToString();
            receiverTransaction.TimeOfTransaction = PointOfTime;
            receiverTransaction.Amount = Amount;
            Banks[ReceiverBankName].Transactions.Add(receiverTransaction);
        }
        public void TransactionHistory(string BankName,string AccountNumber,string Password)
        {
            if (!Banks[BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Acount does not exist");
            if (Password != Banks[BankName].Accounts[AccountNumber].Password)
                throw new Exception("Incorrect Password");
            foreach(var tr in Banks[BankName].Transactions)
            {
                if(tr.SenderAccountId==Banks[BankName].Accounts[AccountNumber].AccountId || tr.ReceiverAccountId== Banks[BankName].Accounts[AccountNumber].AccountId)
                {
                    Console.WriteLine("Id : ", tr.TransactionId);
                    Console.WriteLine("Sender Id : ", tr.SenderAccountId);
                    Console.WriteLine("Recciver Id : ", tr.ReceiverAccountId);
                    Console.WriteLine("Amount : ", tr.Amount);
                    Console.WriteLine("On : ",tr.TimeOfTransaction);
                }
            }
        }

    }
}

using BankApp.Models;
using System;
using System.Collections.Generic;
namespace BankApp.Services
{
    public class CustomerService
    {
        public static string AddBank(string bankName,string clerkName,string dob,string address,string password,string mobileNumber,decimal salary)
        {
            if (ClerkService.IsBankExist(bankName))
                throw new Exception("Bank Name already exist");
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = $"{bankName.Substring(0, 3)}{Date}";
            decimal RTGSChargesSame = 5;
            decimal RTGSChargesOther = 0;
            decimal IMPSChargesSame = 2;
            decimal IMPSChargesOther = 6;
            string Query = $"INSERT INTO Bank VALUES(@bankName,@bankId,@estdDate,@balance,@impsSame,@impsOther,@rtgsSame,@rtgsOther)";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankName", bankName);
            cmd.Parameters.AddWithValue("@bankId", BankId);
            cmd.Parameters.AddWithValue("@estdDate", Date);
            cmd.Parameters.AddWithValue("@balance", 0);
            cmd.Parameters.AddWithValue("@impsSame", IMPSChargesSame);
            cmd.Parameters.AddWithValue("@impsOther", IMPSChargesOther);
            cmd.Parameters.AddWithValue("@rtgsSame", RTGSChargesSame);
            cmd.Parameters.AddWithValue("@rtgsOther", RTGSChargesOther);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
            string clerkId = $"{clerkName}@{bankName.Replace(" ","").ToLower()}";
            string doj = Date;
            AddClerk(BankId, clerkName, clerkId,password,dob,address,mobileNumber,salary,doj);
            return clerkId;
        }
        public static void AddClerk(string bankId, string clerkName, string clerkId,string password,string dob,string address,string mobileNumber,decimal salary,string doj)
        {
            string Query = @"INSERT INTO Clerk(clerkName,ClerkId,BankId,Password,DOB,DOJ,Address,MobileNumber,Salary) 
                             VALUES(@clerkName,@clerkId,@BankId,@password,@dob,@doj,@address,@mobileNo,@salary)";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@clerkName", clerkName);
            cmd.Parameters.AddWithValue("@clerkId", clerkId);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@doj", doj);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("mobileNo", mobileNumber);
            cmd.Parameters.AddWithValue("@salary", salary);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void Deposit(string bankName,string accountNumber,decimal amount)
        {
            if (!ClerkService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string TypeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            decimal Balance = FetchAccountBalance(accountNumber);
            Balance += amount;
            UpdateAccountBalance(accountNumber,TypeOfTransaction,Balance);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            string BankId = ClerkService.GetBankId(bankName);
            string AccountId = ClerkService.GetAccountId(bankName,accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            InsertTransaction(BankId,accountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TypeOfTransaction,amount,PointOfTime);
        }
        public static decimal FetchAccountBalance(string accountNumber)
        {
            string Query = $"SELECT balance FROM Account WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            decimal Amount = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static void Withdraw(string bankName,string accountNumber,decimal amount,string password)
        {
            if (!ClerkService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Acount does not exist!");
            if (FetchAccountPassword(accountNumber) != password)
                throw new Exception("Incorrect Password!");
            decimal Balance = FetchAccountBalance(accountNumber);
            if ( Balance < amount)
                throw new Exception("Insufficient Balance!");
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            Balance -= amount;
            UpdateAccountBalance(accountNumber,TypeOfTransaction,Balance);
            DateTime PointOfTime = DateTime.Now;
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string BankId = ClerkService.GetBankId(bankName);
            string AccountId = ClerkService.GetAccountId(bankName, accountNumber);
            string TransactionId = $"TXN{BankId}{AccountId}{Date}";
            string SenderAccountId = "-";
            string ReceiverAccountId = "-";
            InsertTransaction(BankId,accountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount,PointOfTime);
        }
        public static void Transfer(string senderBankName,string senderAccountNumber,string receiverBankName,string receiverAccountNumber,decimal amount,string password,Enums.TypeOfTransfer typeOfTransfer)
        {
            if (!ClerkService.IsAccountExist(senderBankName, senderAccountNumber))
                throw new Exception("Acount does not exist!");
            if (!ClerkService.IsAccountExist(receiverBankName, receiverAccountNumber))
                throw new Exception("The Account of receiver does not exist!");
            if (FetchAccountPassword(senderAccountNumber) != password)
                throw new Exception("Incorrect Password!");
            decimal Balance = FetchAccountBalance(senderAccountNumber);
            if (Balance < amount)
                throw new Exception("Insufficient Balance!");
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            string SenderBankId = ClerkService.GetBankId(senderBankName);
            string SenderAccountId = ClerkService.GetAccountId(senderBankName,senderAccountNumber);
            string TransactionId = $"TXN{SenderBankId}{SenderAccountId}{Date}";
            string ReceiverBankId = ClerkService.GetBankId(senderBankName);
            string ReceiverAccountId = ClerkService.GetAccountId(receiverBankName,receiverAccountNumber);
            string TypeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DateTime PointOfTime = DateTime.Now;
            Balance -= amount;
            UpdateAccountBalance(senderAccountNumber, "DEBIT", amount);
            InsertTransaction(senderBankName, senderAccountNumber, TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            decimal Charges;
            if (senderBankName == receiverBankName)
            {
                if(typeOfTransfer == Enums.TypeOfTransfer.IMPS)
                    Charges = ClerkService.GetImpsChargesForSameBank(SenderBankId) * amount / 100;
                else
                    Charges = ClerkService.GetRtgsChargesForSameBank(SenderBankId) * amount / 100;
            }
            else
            {
                if(typeOfTransfer== Enums.TypeOfTransfer.IMPS)
                    Charges = ClerkService.GetImpsChargesForOtherBank(SenderBankId) * amount / 100;
                else
                    Charges = ClerkService.GetRtgsChargesForOtherBank(SenderBankId) * amount / 100;
                
            }
            amount -= Charges;
            Balance = FetchAccountBalance(receiverAccountNumber);
            UpdateAccountBalance(receiverAccountNumber, "CREDIT", amount);
            InsertTransaction(receiverBankName,receiverAccountNumber,TransactionId, SenderAccountId, ReceiverAccountId, TypeOfTransaction, amount, PointOfTime);
            Balance = ClerkService.FetchBankBalance(SenderBankId);
            Balance += Charges;
            ClerkService.UpdateBankBalance(senderBankName,Balance);
        }
        public static string GetAccountCurrencyType(string accountNumber)
        {
            string Query = $"SELECT Currency FROM Account WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            string type = (string)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return type;
        }
        public static void InsertTransaction(string bankId, string accountNumber, string transactionId, string senderAccountId, string receiverAccountId, string typeOfTransaction, decimal amount, DateTime pointOfTime)
        {
            string Query = @"INSERT INTO Transaction(BankId,AccountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TransactionType,Amount,TransactionTime,Avl_Bal) 
                              VALUES(@bankId,@accountNo,@txnId,@senderAccId,@receiverAccId,@txnType,@amount,@time,@balance)";
            decimal Balance = FetchAccountBalance(accountNumber);
            string balance = $"{Balance.ToString()}({GetAccountCurrencyType(accountNumber)})";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            cmd.Parameters.AddWithValue("@txnId", transactionId);
            cmd.Parameters.AddWithValue("@senderAccId", senderAccountId);
            cmd.Parameters.AddWithValue("@receiverAccId", receiverAccountId);
            cmd.Parameters.AddWithValue("@txnType", typeOfTransaction);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@time", pointOfTime);
            cmd.Parameters.AddWithValue("@balance", balance);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateAccountBalance(string accountNumber, string typeOfTransaction, decimal balance)
        {
            string Query = $"UPDATE Account SET balance = @balance WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@balance", balance);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static List<string> TransactionHistory(string bankName,string accountNumber,string password)
        {
            List<string> History = new List<string>();
            if (!ClerkService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            if (FetchAccountPassword(accountNumber)!=password)
                throw new Exception("Incorrect Password");
            History = ClerkService.FetchAccountTransactions(accountNumber);
            return History;
        }

        public static string FetchAccountPassword(string accountNumber)
        {
            string Query = $"SELECT  password FROM Account WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            string Key = (string)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Key;
        }
    }
}

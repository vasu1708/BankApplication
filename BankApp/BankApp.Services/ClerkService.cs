using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace BankApp.Services
{
    public class ClerkService
    {
        public  string GenerateAccountNumber(string bankName)
        {
            Random random = new Random();
            uint AccountNo = (UInt32)random.Next(111111111, 999999999);
            return AccountNo.ToString();
        }
        
        public  string CreateAccount(string bankName,string name, string mobileNumber,Enums.Gender gender,string address,string dob,string password)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string accountId = $"{name.Substring(0, 3)}{date}";
            string doc = date;
            string accountNumber = GenerateAccountNumber(bankName);
            string bankId = GetBankId(bankName);
            string query = @"INSERT INTO Account(bankId,accountId,accountNumber,Name,Gender,MobileNumber,Password,Address,balance,Currency,DOB,DOC) 
                             VALUES(@bankId,@accountId,@accountNo,@name,@gender,@mobileNo,@password,@address,@balance,@currency,@dob,@doc)";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            cmd.Parameters.AddWithValue("@accountId", accountId);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@gender", gender.ToString());
            cmd.Parameters.AddWithValue("@mobileNo", mobileNumber);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@balance", 0);
            cmd.Parameters.AddWithValue("@address",address);
            cmd.Parameters.AddWithValue("@currency", "INR");
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@doc", doc);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
            return accountNumber;
        }
        public  string FetchClerkPassword(string id)
        {
            string query = $"SELECT  password FROM Clerk WHERE ClerkId = @clerkid";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@clerkid", id);
            return (string)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  string GetBankId(string bankName)
        {
            string query = $"SELECT bankId FROM Bank WHERE BankName = @bankName";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankName", bankName);
            return (string)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  bool IsBankExist(string bankName)
        {
            return GetBankId(bankName) != null;
        }
        public  decimal FetchBankBalance(string bankId)
        {
            string query = $"SELECT balance FROM Bank WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  string GetAccountId(string bankName, string accountNumber)
        {
            string bankId = GetBankId(bankName);
            string query = $"SELECT  accountId FROM Account WHERE Accountnumber = @accountNo and bankId =@bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (string)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  bool IsAccountExist(string bankName, string accountNumber)
        {
            return GetAccountId(bankName,accountNumber) != null;
        }
        public  void UpdateBankBalance(string bankName, decimal balance)
        {
            string bankId = GetBankId(bankName);
            string query = $"UPDATE Account SET balance = @balance WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@balance", balance);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }
        public  void DeleteAccount(string bankName,string accountNumber)
        {
            if (!IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string query = $"DELETE FROM Account WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }
        public  List<string> TransactionHistory(string bankName,string accountNumber)
        {
            if (!IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            return FetchAccountTransactions(accountNumber);
        }
        public  void UpdateServiceCharges(string bankName,decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            string bankId = GetBankId(bankName);
            string query = @"UPDATE Bank SET IMPSsame=@impsSame,IMPSother=@impsOther,RTGSsame=@rtgsSame,RTGSother=@rtgsOther 
                              WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@impsSame", chargeOfIMPSForSame);
            cmd.Parameters.AddWithValue("@impsOther", chargeOfIMPSForOther);
            cmd.Parameters.AddWithValue("@rtgsSame", chargeOfRTGSForSame);
            cmd.Parameters.AddWithValue("@rtgsOther", chargeOfRTGSForOther);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }      
        public  void RevertTransaction(string bankName,string accountNumber, string transactionId)
        {
            if (!IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string query = $"DELETE FROM Transaction WHERE TransationId = @transactionId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@transactionId", transactionId);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);

        }
        public  void UpdateCurrency(string bankName,string accountNumber,Enums.CurrencyType currency)
        {
            if (!IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string CurrencyType = new CustomerService().GetAccountCurrencyType(accountNumber);
            if (CurrencyType == currency.ToString())
                throw new Exception("New Currency is same as Existing!");
            decimal conversionRatio = 1;
            if (currency == Enums.CurrencyType.INUSD)
                conversionRatio = 1 / 70;
            else if (Enums.CurrencyType.INR == currency)
                conversionRatio = 70;
            decimal balance = FetchAccountBalance(accountNumber);
            balance *= conversionRatio;
            string query = $"UPDATE Account SET Currency=@currency,balance=@balance WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@currency", currency);
            cmd.Parameters.AddWithValue("@balance", balance);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }
        public  decimal FetchAccountBalance(string accountNumber)
        {
            string query = $"SELECT balance FROM Account WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  List<string> FetchAccountTransactions(string accountNumber)
        {
            List<string> transactions = new List<string>();
            string query = $"SELECT * FROM Transaction WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DataTable dt = new DatabaseService().ExecuteDataAdapterAndCloseConnection(cmd);
            string transaction;
            foreach (DataRow TXN in dt.Rows)
            { 
                transaction  = $"{TXN["transactionId"]} {TXN["senderAccountId"]} {TXN["ReceiverAccountId"]} {TXN["TransactionType"]} {TXN["Amount"]} {TXN["TransactionTime"]} {TXN["Avl_Bal"]}";
                transactions.Add(transaction);
            }
            return transactions;
        }
        public  void UpdateAddress(string bankName,string accountNumber,string address)
        {
            if (!this.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string query = $"UPDATE Account SET Address=@address WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }
        public  void UpdateMobileNumber(string bankName,string accountNumber,string mobileNumber)

        {
            if (!IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string query = $"UPDATE Account SET MobileNumber = @mobileNo WHERE accountNumber = @accountNo";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@mobileNo", mobileNumber);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            new DatabaseService().ExecuteNonQueryAndCloseConnection(cmd);
        }
        public  decimal GetImpsChargesForSameBank(string bankId)
        {
            string query = $"SELECT IMPSsame FROM Bank WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  decimal GetImpsChargesForOtherBank(string bankId)
        {
            string query = $"SELECT IMPSother FROM Bank WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  decimal GetRtgsChargesForSameBank(string bankId)
        {
            string query = $"SELECT RTGSsame FROM Bank WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
        public  decimal GetRtgsChargesForOtherBank(string bankId)
        {
            string query = $"SELECT RTGSother FROM Bank WHERE bankId = @bankId";
            var cmd = new DatabaseService().OpenConnectionAndCreateCommand(query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            return (decimal)new DatabaseService().ExecuteScalarAndCloseConnection(cmd);
        }
    }
    
}

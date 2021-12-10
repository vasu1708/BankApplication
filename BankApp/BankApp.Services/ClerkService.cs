using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace BankApp.Services
{
    public class ClerkService
    {
        public static string GenerateAccountNumber(string bankName)
        {
            Random random = new Random();
            uint AccountNo = (UInt32)random.Next(111111111, 999999999);
            return AccountNo.ToString();
        }
        
        public static string CreateAccount(string bankName,string name, string mobileNumber,Enums.Gender gender,string address,string dob,string password)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string AccountId = $"{name.Substring(0, 3)}{date}";
            string doc = date;
            string AccountNumber = GenerateAccountNumber(bankName);
            string Password = password;
            string BankId = GetBankId(bankName);
            string Query = @"INSERT INTO Account(BankId,AccountId,AccountNumber,Name,Gender,MobileNumber,Password,Address,Balance,Currency,DOB,DOC) 
                             VALUES(@bankId,@accountId,@accountNo,@name,@gender,@mobileNo,@password,@address,@balance,@currency,@dob,@doc)";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", BankId);
            cmd.Parameters.AddWithValue("@accountId", AccountId);
            cmd.Parameters.AddWithValue("@accountNo", AccountNumber);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@gender", gender.ToString());
            cmd.Parameters.AddWithValue("@mobileNo", mobileNumber);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@balance", 0);
            cmd.Parameters.AddWithValue("@address",address);
            cmd.Parameters.AddWithValue("@currency", "INR");
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@doc", doc);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
            return AccountNumber;
        }
        public static string FetchClerkPassword(string id)
        {
            string Query = $"SELECT  password FROM Clerk WHERE ClerkId = @clerkid";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@clerkid", id);
            string Key = (string)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Key;
        }
        public static string GetBankId(string bankName)
        {
            string Query = $"SELECT BankId FROM Bank WHERE BankName = @bankName";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankName", bankName);
            string Id = (string)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Id;
        }
        public static bool IsBankExist(string bankName)
        {
            string Id = null;
            Id = GetBankId(bankName);
            if (Id == null)
                return false;
            return true;
        }
        public static decimal FetchBankBalance(string bankId)
        {
            string Query = $"SELECT balance FROM Bank WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            decimal Amount = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static string GetAccountId(string bankName, string accountNumber)
        {
            string BankId = GetBankId(bankName);
            string Query = $"SELECT  AccountId FROM Account WHERE Accountnumber = @accountNo and BankId =@bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            cmd.Parameters.AddWithValue("@bankId", BankId);
            string Id = (string)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Id;
        }
        public static bool IsAccountExist(string bankName, string accountNumber)
        {
            string Id = null;

            Id = GetAccountId(bankName, accountNumber);
            if (Id == null)
                return false;
            return true;
        }
        public static void UpdateBankBalance(string bankName, decimal balance)
        {
            string BankId = GetBankId(bankName);
            string Query = $"UPDATE Account SET Balance = @balance WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@balance", balance);
            cmd.Parameters.AddWithValue("@bankId", BankId);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void DeleteAccount(string bankName,string accountNumber)
        {
            if (!ClerkService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string Query = $"DELETE FROM Account WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static List<string> TransactionHistory(string bankName,string accountNumber)
        {
            List<string> History = new List<string>();
            if (!ClerkService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            History = FetchAccountTransactions(accountNumber);
            return History;
        }
        public static void UpdateServiceCharges(string bankName,decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            string BankId = ClerkService.GetBankId(bankName);
            string Query = @"UPDATE Bank SET IMPSsame=@impsSame,IMPSother=@impsOther,RTGSsame=@rtgsSame,RTGSother=@rtgsOther 
                              WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@impsSame", chargeOfIMPSForSame);
            cmd.Parameters.AddWithValue("@impsOther", chargeOfIMPSForOther);
            cmd.Parameters.AddWithValue("@rtgsSame", chargeOfRTGSForSame);
            cmd.Parameters.AddWithValue("@rtgsOther", chargeOfRTGSForOther);
            cmd.Parameters.AddWithValue("@bankId", BankId);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }      
        public static void RevertTransaction(string bankName,string accountNumber, string transactionId)
        {
            if (!ClerkService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string Query = $"DELETE FROM Transaction WHERE TransationId = @transactionId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@transactionId", transactionId);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);

        }
        public static void UpdateCurrency(string bankName,string accountNumber,Enums.CurrencyType currency)
        {
            if (!ClerkService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string CurrencyType = CustomerService.GetAccountCurrencyType(accountNumber);
            if (CurrencyType == currency.ToString())
                throw new Exception("New Currency is same as Existing!");
            decimal ConversionRatio = 1;
            if (currency == Enums.CurrencyType.INUSD)
                ConversionRatio = 1 / 70;
            else if (Enums.CurrencyType.INR == currency)
                ConversionRatio = 70;
            decimal Balance = FetchAccountBalance(accountNumber);
            Balance *= ConversionRatio;
            string Query = $"UPDATE Account SET Currency=@currency,Balance=@balance WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@currency", currency);
            cmd.Parameters.AddWithValue("@balance", Balance);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal FetchAccountBalance(string accountNumber)
        {
            string Query = $"SELECT balance FROM Account WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            decimal Amount = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static List<string> FetchAccountTransactions(string accountNumber)
        {
            List<string> Transactions = new List<string>();
            string Query = $"SELECT * FROM Transaction WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DataTable dt = DatabaseService.ExecuteDataAdapterAndCloseConnection(cmd);
            string transaction;
            foreach (DataRow TXN in dt.Rows)
            { 
                transaction  = $"{TXN["TransactionId"]} {TXN["SenderAccountId"]} {TXN["ReceiverAccountId"]} {TXN["TransactionType"]} {TXN["Amount"]} {TXN["TransactionTime"]} {TXN["Avl_Bal"]}";
                Transactions.Add(transaction);
            }
            return Transactions;
        }
        public static void UpdateAddress(string bankName,string accountNumber,string address)
        {
            if (!ClerkService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string Query = $"UPDATE Account SET Address=@address WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateMobileNumber(string bankName,string accountNumber,string mobileNumber)

        {
            if (!ClerkService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string Query = $"UPDATE Account SET MobileNumber = @mobileNo WHERE AccountNumber = @accountNo";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@mobileNo", mobileNumber);
            cmd.Parameters.AddWithValue("@accountNo", accountNumber);
            DatabaseService.ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal GetImpsChargesForSameBank(string bankId)
        {
            string Query = $"SELECT IMPSsame FROM Bank WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            decimal charge = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetImpsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT IMPSother FROM Bank WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            decimal charge = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForSameBank(string bankId)
        {
            string Query = $"SELECT RTGSsame FROM Bank WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            decimal charge = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT RTGSother FROM Bank WHERE BankId = @bankId";
            var cmd = DatabaseService.OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.AddWithValue("@bankId", bankId);
            decimal charge = (decimal)DatabaseService.ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
    }
    
}

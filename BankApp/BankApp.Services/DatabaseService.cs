using BankApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace BankApp.Services
{
    public class DatabaseService
    {
        private static MySqlConnection Connection { get; set; }
        
        public static MySqlCommand OpenConnectionAndCreateCommand(string query)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            return new MySqlCommand(query, Connection);
        }
        public static bool IsAuthorized(string id,string password)
        {
            string Query = $"SELECT  password FROM Clerk WHERE ClerkId = @clerkid";
            MySqlCommand cmd= OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@clerkid", MySqlDbType.VarChar).Value = id;
            string Key = (string)ExecuteScalarAndCloseConnection(cmd);
            if (password == Key)
                return true;
            return false;
        }
        public static void OpenConnection(string serverName,string databaseName,string userId,string password)
        {
            string ConnectionString = $"SERVER={serverName};DATABASE={databaseName};UID={userId};PASSWORD={password}";
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
        }
        
        public static void InsertIntoBank(string bankName,string bankId,decimal impsChargesSame,decimal impsChargesOther,decimal rtgsChargesSame,decimal rtgsChargesOther,string estdDate)
        {
            string Query = $"INSERT INTO Bank VALUES(@bankName,@bankId,@estdDate,@balance,@impsSame,@impsOther,@rtgsSame,@rtgsOther)";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankName", MySqlDbType.VarChar).Value = bankName;
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            cmd.Parameters.Add("@estdDate", MySqlDbType.Date).Value = estdDate;
            cmd.Parameters.Add("@balance", MySqlDbType.Decimal).Value = 0;
            cmd.Parameters.Add("@impsSame", MySqlDbType.Decimal).Value = impsChargesSame;
            cmd.Parameters.Add("@impsOther", MySqlDbType.Decimal).Value = impsChargesOther;
            cmd.Parameters.Add("@rtgsSame", MySqlDbType.Decimal).Value = rtgsChargesSame;
            cmd.Parameters.Add("@rtgsOther", MySqlDbType.Decimal).Value = rtgsChargesOther;
            ExecuteNonQueryAndCloseConnection(cmd);
        }

        public static void InsertIntoClerk(string bankId ,string clerkName,string clerkId,string password,string dob,string address,string mobileNumber,decimal salary,string doj)
        {
            string Query = @"INSERT INTO Clerk(clerkName,ClerkId,BankId,Password,DOB,DOJ,Address,MobileNumber,Salary) 
                             VALUES(@clerkName,@clerkId,@BankId,@password,@dob,@doj,@address,@mobileNo,@salary)";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@clerkName", MySqlDbType.VarChar).Value = clerkName;
            cmd.Parameters.Add("@clerkId", MySqlDbType.VarChar).Value = clerkId;
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
            cmd.Parameters.Add("@dob", MySqlDbType.Date).Value = dob;
            cmd.Parameters.Add("@doj", MySqlDbType.Date).Value = doj;
            cmd.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
            cmd.Parameters.Add("mobileNo",MySqlDbType.VarChar).Value = mobileNumber;
            cmd.Parameters.Add("@salary", MySqlDbType.Decimal).Value = salary;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateAccountBalance(string accountNumber,string typeOfTransaction,decimal balance)
        {
            string Query = $"UPDATE Account SET balance = @balance WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@balance", MySqlDbType.Decimal).Value = balance;
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal FetchAccountBalance(string accountNumber)
        {
            string Query = $"SELECT balance FROM Account WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            decimal Amount = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static void InsertTransaction(string bankId,string accountNumber,string transactionId,string senderAccountId,string receiverAccountId,string typeOfTransaction,decimal amount,DateTime pointOfTime)
        {
            string Query = @"INSERT INTO Transaction(BankId,AccountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TransactionType,Amount,TransactionTime,Avl_Bal) 
                              VALUES(@bankId,@accountNo,@txnId,@senderAccId,@receiverAccId,@txnType,@amount,@time,@balance)";
            decimal Balance = FetchAccountBalance(accountNumber);
            string balance = $"{Balance.ToString()}({GetAccountCurrencyType(accountNumber)})";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@txnId", MySqlDbType.VarChar).Value = transactionId;
            cmd.Parameters.Add("@senderAccId", MySqlDbType.VarChar).Value = senderAccountId;
            cmd.Parameters.Add("@receiverAccId", MySqlDbType.VarChar).Value = receiverAccountId;
            cmd.Parameters.Add("@txnType", MySqlDbType.VarChar).Value = typeOfTransaction;
            cmd.Parameters.Add("@amount", MySqlDbType.Decimal).Value = amount;
            cmd.Parameters.Add("@time", MySqlDbType.DateTime).Value = pointOfTime;
            cmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = balance;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static bool VerifyAccountPassword(string accountNumber,string password)
        {
            string Query = $"SELECT  password FROM Account WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            string Key = (string) ExecuteScalarAndCloseConnection(cmd);
            if (password == Key)
                return true;
            return false;
        }
        public static void UpdateBankBalance(string bankName,decimal balance)
        {
            string BankId = GetBankId(bankName);
            string Query = $"UPDATE Account SET Balance = @balance WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@balance", MySqlDbType.Decimal).Value = balance;
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = BankId;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal FetchBankBalance(string bankId)
        {
            string Query = $"SELECT balance FROM Bank WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            decimal Amount = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static List<string> FetchAccountTransactions(string accountNumber)
        {
            List<string> Records = new List<string>();
            string Query = $"SELECT * FROM Transaction WHERE AccountNumber = @accountNo";
            string record;
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                record = $"{dataReader["TransactionId"]} {dataReader["SenderAccountId"]} {dataReader["ReceiverAccountId"]} {dataReader["TransactionType"]} {dataReader["Amount"]} {dataReader["TransactionTime"]} {dataReader["Avl_Bal"]}";
                Records.Add(record);
            }
            dataReader.Close();
            Connection.Close();
            return Records;
        }
        public static string GetBankId(string bankName)
        {
            string Query = $"SELECT BankId FROM Bank WHERE BankName = @bankName";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankName", MySqlDbType.VarChar).Value = bankName;
            string Id = (string) ExecuteScalarAndCloseConnection(cmd);
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
        public static bool IsBankExist(string bankName)
        {
            string Id = null;
            Id = GetBankId(bankName);
            if (Id == null)
                return false;
            return true;
        }
        public static void InsertIntoAccount(string bankName,string name,string accountId,string accountNumber,string mobileNumber,string dob,string doc,Enums.Gender gender,string address,string password)
        {
            string BankId = GetBankId(bankName);
            string Query = @"INSERT INTO Account(BankId,AccountId,AccountNumber,Name,Gender,MobileNumber,Password,Balance,Currency,DOB,DOC) 
                             VALUES(@bankId,@accountId,@accountNo,@name,@gender,@mobileNo,@password,@balance,@currency,@dob,@doc)";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = BankId;
            cmd.Parameters.Add("@accountId", MySqlDbType.VarChar).Value = accountId;
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@gender", MySqlDbType.Enum).Value = gender;
            cmd.Parameters.Add("@mobileNo", MySqlDbType.VarChar).Value = mobileNumber;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
            cmd.Parameters.Add("@balance", MySqlDbType.VarChar).Value = 0;
            cmd.Parameters.Add("@currency", MySqlDbType.Enum).Value = "INR";
            cmd.Parameters.Add("@dob", MySqlDbType.Date).Value = dob;
            cmd.Parameters.Add("@doc", MySqlDbType.Date).Value = doc;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static string GetAccountId(string bankName,string accountNumber)
        {
            string BankId = GetBankId(bankName);
            string Query = $"SELECT  AccountId FROM Account WHERE Accountnumber = @accountNo and BankId =@bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = BankId;
            string Id = (string) ExecuteScalarAndCloseConnection(cmd);
            return Id;
        }
        public static void DeleteFromAccount(string accountNumber)
        {
            string Query = $"DELETE FROM Account WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        
        public static void DeleteFromTransaction(string transactionId)
        {
            string Query = $"DELETE FROM Transaction WHERE TransationId = @transactionId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@transactionId", MySqlDbType.VarChar).Value = transactionId;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateCurrency(string accountNumber,string currency,decimal Balance)
        {
            string Query = $"UPDATE Account SET Currency=@currency,Balance=@balance WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@currency", MySqlDbType.VarChar).Value = currency;
            cmd.Parameters.Add("@balance", MySqlDbType.Decimal).Value = Balance;
            cmd.Parameters.Add("@accountNo", MySqlDbType.Enum).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static string GetAccountCurrencyType(string accountNumber)
        {
            string Query = $"SELECT Currency FROM Account WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            string type = (string) ExecuteScalarAndCloseConnection(cmd);
            return type;
        }
        public static void UpdateCharges(string bankName,decimal impsSame,decimal impsOther,decimal rtgsSame,decimal rtgsOther)
        {
            string BankId = GetBankId(bankName);
            string Query = @"UPDATE Bank SET IMPSsame=@impsSame,IMPSother=@impsOther,RTGSsame=@rtgsSame,RTGSother=@rtgsOther 
                              WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@impsSame", MySqlDbType.Decimal).Value = impsSame;
            cmd.Parameters.Add("@impsOther", MySqlDbType.Decimal).Value = impsOther;
            cmd.Parameters.Add("@rtgsSame", MySqlDbType.Decimal).Value = rtgsSame;
            cmd.Parameters.Add("@rtgsOther", MySqlDbType.Decimal).Value = rtgsOther;
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = BankId;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static Object ExecuteScalarAndCloseConnection(MySqlCommand cmd)
        {
            var value = cmd.ExecuteScalar();
            Connection.Close();
            return value;
        }
        public static void ExecuteNonQueryAndCloseConnection(MySqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
            Connection.Close();
        }
        public static decimal GetImpsChargesForSameBank(string bankId)
        {
            string Query = $"SELECT IMPSsame FROM Bank WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetImpsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT IMPSother FROM Bank WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForSameBank(string bankId)
        {
            string Query = $"SELECT RTGSsame FROM Bank WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT RTGSother FROM Bank WHERE BankId = @bankId";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@bankId", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static void UpdateAddress(string accountNumber,string address)
        {
            string Query = $"UPDATE Account SET Address=@address WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            string Query = $"UPDATE Account SET MobileNumber = @mobileNo WHERE AccountNumber = @accountNo";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@mobileNo", MySqlDbType.VarChar).Value = mobileNumber;
            cmd.Parameters.Add("@accountNo", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
    }
}

using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using BankApp.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void OpenConnection(string serverName,string databaseName,string userId,string password)
        {
            string ConnectionString = $"SERVER={serverName};DATABASE={databaseName};UID={userId};PASSWORD={password}";
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
        }
        
        public static bool IsBankExist(string bankName)
        {
            string Id = null;
            Id = GetBankId(bankName);
            if (Id == null)
                return false;
            return true;
        }
        public static void InsertIntoBank(string bankName,string bankId,decimal impsChargesSame,decimal impsChargesOther,decimal rtgsChargesSame,decimal rtgsChargesOther)
        {
            string Query = $"INSERT INTO Bank VALUES(@1,@2,@3,@4,@5,@6,@7,@8)";
            string Date = DateTime.Now.ToString("yyyy-MM-dd");
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankName;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = bankId;
            cmd.Parameters.Add("@3", MySqlDbType.Date).Value = Date;
            cmd.Parameters.Add("@4", MySqlDbType.Decimal).Value = 0;
            cmd.Parameters.Add("@5", MySqlDbType.Decimal).Value = impsChargesSame;
            cmd.Parameters.Add("@6", MySqlDbType.Decimal).Value = impsChargesOther;
            cmd.Parameters.Add("@7", MySqlDbType.Decimal).Value = rtgsChargesSame;
            cmd.Parameters.Add("@8", MySqlDbType.Decimal).Value = rtgsChargesOther;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        
        /*public static void AddClerk(string bankId,string clerkName,string password)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");

            string Query = $"INSERT INTO Clerk(ClerkId,BankId,Password) VALUES(@1,@2,@3)";
            string ClerkId = clerkName;
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = ClerkId;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = bankId;
            cmd.Parameters.Add("@3", MySqlDbType.VarChar).Value = password;

            cmd.ExecuteNonQuery();
            ();
        }*/
        public static void UpdateAccountBalance(string accountNumber,string typeOfTransaction,decimal amount)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            decimal Balance = FetchAccountBalance(accountNumber);
            if (typeOfTransaction == "DEBIT")
                Balance -= amount;
            else
                Balance += amount;
            string Query = $"UPDATE Account SET balance = @1 WHERE AccountNumber = @2";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.Decimal).Value = Balance;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal FetchAccountBalance(string accountNumber)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT balance FROM Account WHERE AccountNumber = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
            decimal Amount = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static void InsertTransaction(string bankName,string accountNumber,string transactionId,string senderAccountId,string receiverAccountId,string typeOfTransaction,decimal amount,DateTime pointOfTime)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"INSERT INTO Transaction(BankId,AccountNumber,TransactionId,SenderAccountId,ReceiverAccountId,TransactionType,Amount,TransactionTime,Avl_Bal) VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9)";
            string BankId = GetBankId(bankName);
            decimal Balance = FetchAccountBalance(accountNumber);
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = BankId;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@3", MySqlDbType.VarChar).Value = transactionId;
            cmd.Parameters.Add("@4", MySqlDbType.VarChar).Value = senderAccountId;
            cmd.Parameters.Add("@5", MySqlDbType.VarChar).Value = receiverAccountId;
            cmd.Parameters.Add("@6", MySqlDbType.VarChar).Value = typeOfTransaction;
            cmd.Parameters.Add("@7", MySqlDbType.Decimal).Value = amount;
            cmd.Parameters.Add("@8", MySqlDbType.DateTime).Value = pointOfTime;
            cmd.Parameters.Add("@9", MySqlDbType.Decimal).Value = Balance;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static bool VerifyAccountPassword(string accountNumber,string password)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT  password FROM Account WHERE AccountNumber = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
            string Key = (string) ExecuteScalarAndCloseConnection(cmd);
            if (password == Key)
                return true;
            return false;
        }
        public static void UpdateBankBalance(string bankName,decimal charges)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string BankId = GetBankId(bankName);
            decimal Balance = FetchBankBalance(BankId);
            Balance += charges;
            string Query = $"UPDATE Account SET balance = @1 WHERE AccountNumber = @2";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.Decimal).Value = Balance;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = bankName;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static decimal FetchBankBalance(string bankId)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT balance FROM Bank WHERE BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankId;
            decimal Amount = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return Amount;
        }
        public static List<string> FetchAccountTransactions(string accountNumber)
        {
            List<string> Records = new List<string>();
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT * FROM Transaction WHERE AccountNumber = @1";
            string record;
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
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
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT BankId FROM Bank WHERE BankName = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankName;
            string Id = (string) ExecuteScalarAndCloseConnection(cmd);
            return Id;
        }
        public static bool IsAccountExist(string bankName,string accountNumber)
        {
            string Id = null;
            
            Id = GetAccountId(bankName, accountNumber);
            if (Id == null)
                return false;
            return true;
        }
        public static void InsertIntoAccount(string bankName,string name,string accountId,string accountNumber,string mobileNumber,Enums.Gender gender,string address,string password)
        {
            string BankId = GetBankId(bankName);
            string Query = $"INSERT INTO Account(BankId,AccountId,AccountNumber,Name,Gender,MobileNumber,Password,Balance,Currency) VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9)";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = BankId;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = accountId;
            cmd.Parameters.Add("@3", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@4", MySqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@5", MySqlDbType.Enum).Value = gender;
            cmd.Parameters.Add("@6", MySqlDbType.VarChar).Value = mobileNumber;
            cmd.Parameters.Add("@7", MySqlDbType.VarChar).Value = password;
            cmd.Parameters.Add("@8", MySqlDbType.Decimal).Value = 0;
            cmd.Parameters.Add("@9", MySqlDbType.Enum).Value = "INR";
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static string GetAccountId(string bankName,string accountNumber)
        {
            string BankId = GetBankId(bankName);
            string Query = $"SELECT  AccountId FROM Account WHERE Accountnumber = @1 and BankId =@2";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = BankId;
            string Id = (string) ExecuteScalarAndCloseConnection(cmd);
            return Id;
        }
        public static void DeleteFromAccount(string accountNumber)
        {
            string Query = $"DELETE FROM Account WHERE AccountNumber = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        
        public static void DeleteFromTransaction(string transactionId)
        {
            string Query = $"DELETE FROM Transaction WHERE TransationId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = transactionId;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateCurrency(string accountNumber,string currency)
        {
            decimal ConversionRatio = 1;
            if (currency == "INUSD")
                ConversionRatio = 1 / 70;
            else if (currency == "INR")
                ConversionRatio = 70;
            decimal Balance = FetchAccountBalance(accountNumber);
            Balance *= ConversionRatio;
            string Query = $"UPDATE Account SET Currency=@1,Balance=@2 WHERE AccountNumber = @3";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = currency;
            cmd.Parameters.Add("@2", MySqlDbType.Decimal).Value = Balance;
            cmd.Parameters.Add("@3", MySqlDbType.Enum).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static string GetAccountCurrencyType(string accountNumber)
        {
            string Query = $"SELECT Currency FROM Account WHERE AccountNumber = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = accountNumber;
            string type = (string) ExecuteScalarAndCloseConnection(cmd);
            return type;
        }
        public static void UpdateCharges(string bankName,decimal impsSame,decimal impsOther,decimal rtgsSame,decimal rtgsOther)
        {
            string BankId = GetBankId(bankName);
            string Query = $"UPDATE Bank SET IMPSsame=@1,IMPSother=@2,RTGSsame=@3,RTGSother=@4 WHERE BankId = @5";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.Decimal).Value = impsSame;
            cmd.Parameters.Add("@2", MySqlDbType.Decimal).Value = impsOther;
            cmd.Parameters.Add("@3", MySqlDbType.Decimal).Value = rtgsSame;
            cmd.Parameters.Add("@4", MySqlDbType.Decimal).Value = rtgsOther;
            cmd.Parameters.Add("@5", MySqlDbType.VarChar).Value = BankId;
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
            string Query = $"SELECT IMPSsame FROM Bank WHERE BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetImpsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT IMPSother FROM Bank WHERE BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForSameBank(string bankId)
        {
            string Query = $"SELECT RTGSsame FROM Bank WHERE BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static decimal GetRtgsChargesForOtherBank(string bankId)
        {
            string Query = $"SELECT RTGSother FROM Bank WHERE BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = bankId;
            decimal charge = (decimal) ExecuteScalarAndCloseConnection(cmd);
            return charge;
        }
        public static void UpdateAddress(string accountNumber,string address)
        {
            string Query = $"UPDATE Account SET Address=@1 WHERE AccountNumber = @2";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = address;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }
        public static void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            string Query = $"UPDATE Account SET MobileNumber = @1 WHERE AccountNumber = @2";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = mobileNumber;
            cmd.Parameters.Add("@2", MySqlDbType.VarChar).Value = accountNumber;
            ExecuteNonQueryAndCloseConnection(cmd);
        }

        /*public static string FetchAccountStartNo(string BankName)
        {
            OpenConnection("localhost", "BankDB", "srinivas", "Mysql@1234");
            string Query = $"SELECT AccountStartNo FROM Bank where BankId = @1";
            MySqlCommand cmd = OpenConnectionAndCreateCommand(Query);
            cmd.Parameters.Add("@1", MySqlDbType.VarChar).Value = currency;
            cmd.Parameters.Add("@2", MySqlDbType.Decimal).Value = Balance;
            cmd.Parameters.Add("@3", MySqlDbType.Enum).Value = accountNumber;
            string cmd.ExecuteNonQuery();
            ();
        }*/
    }
}

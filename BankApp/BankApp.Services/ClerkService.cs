using BankApp.Models;
using MySql.Data.MySqlClient;
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
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            parameterList.Add(new MySqlParameter("@accountId", accountId));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            parameterList.Add(new MySqlParameter("@name", name));
            parameterList.Add(new MySqlParameter("@gender", gender.ToString()));
            parameterList.Add(new MySqlParameter("@mobileNo", mobileNumber));
            parameterList.Add(new MySqlParameter("@password", password));
            parameterList.Add(new MySqlParameter("@balance", 0));
            parameterList.Add(new MySqlParameter("@address",address));
            parameterList.Add(new MySqlParameter("@currency", "INR"));
            parameterList.Add(new MySqlParameter("@dob", dob));
            parameterList.Add(new MySqlParameter("@doc", doc));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
            return accountNumber;
        }
        public  string FetchClerkPassword(string id)
        {
            string query = $"SELECT  password FROM Clerk WHERE ClerkId = @clerkid";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@clerkid", id));
            return (string)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public  string GetBankId(string bankName)
        {
            string query = $"SELECT bankId FROM Bank WHERE BankName = @bankName";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankName", bankName));
            return (string)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public  bool IsBankExist(string bankName)
        {
            return GetBankId(bankName) != null;
        }
        public  decimal FetchBankBalance(string bankId)
        {
            string query = $"SELECT balance FROM Bank WHERE bankId = @bankId";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            return (decimal)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public  string GetAccountId(string bankName, string accountNumber)
        {
            string bankId = GetBankId(bankName);
            string query = $"SELECT  accountId FROM Account WHERE Accountnumber = @accountNo and bankId =@bankId";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            return (string)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public  bool IsAccountExist(string bankName, string accountNumber)
        {
            return GetAccountId(bankName,accountNumber) != null;
        }
        public  void UpdateBankBalance(string bankName, decimal balance)
        {
            string bankId = GetBankId(bankName);
            string query = $"UPDATE Account SET balance = @balance WHERE bankId = @bankId";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@balance", balance));
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public  void DeleteAccount(string bankName,string accountNumber)
        {
            if (!IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string query = $"DELETE FROM Account WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
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
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@impsSame", chargeOfIMPSForSame));
            parameterList.Add(new MySqlParameter("@impsOther", chargeOfIMPSForOther));
            parameterList.Add(new MySqlParameter("@rtgsSame", chargeOfRTGSForSame));
            parameterList.Add(new MySqlParameter("@rtgsOther", chargeOfRTGSForOther));
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }      
        public  void RevertTransaction(string bankName,string accountNumber, string transactionId)
        {
            if (!IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string query = $"DELETE FROM Transaction WHERE TransationId = @transactionId";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@transactionId", transactionId));
            new DatabaseService().ExecuteNonQuery(query,parameterList);

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
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@currency", currency));
            parameterList.Add(new MySqlParameter("@balance", balance));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public  decimal FetchAccountBalance(string accountNumber)
        {
            string query = $"SELECT balance FROM Account WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            return (decimal)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public  List<string> FetchAccountTransactions(string accountNumber)
        {
            List<string> transactions = new List<string>();
            string query = $"SELECT * FROM Transaction WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            DataTable dt = new DatabaseService().ExecuteDataAdapter(query,parameterList);
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
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@address", address));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public  void UpdateMobileNumber(string bankName,string accountNumber,string mobileNumber)

        {
            if (!IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string query = $"UPDATE Account SET MobileNumber = @mobileNo WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@mobileNo", mobileNumber));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public  decimal GetCharges(string bankId,string chargeType)
        {
            string query = $"SELECT {chargeType} FROM Bank WHERE bankId = @bankId";
            List<MySqlParameter> parameterList= new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            return (decimal)new DatabaseService().ExecuteScalar(query,parameterList);
        }
    }
    
}

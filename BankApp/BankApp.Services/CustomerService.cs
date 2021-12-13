using BankApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
namespace BankApp.Services
{
    public class CustomerService
    {
        public  string AddBank(string bankName,string clerkName,string dob,string address,string password,string mobileNumber,decimal salary)
        {
            if (new ClerkService().IsBankExist(bankName))
                throw new Exception("Bank Name already exist");
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string bankId = $"{bankName.Substring(0, 3)}{date}";
            decimal RTGSChargesSame = 5;
            decimal RTGSChargesOther = 0;
            decimal IMPSChargesSame = 2;
            decimal IMPSChargesOther = 6;
            string query = $"INSERT INTO Bank VALUES(@bankName,@bankId,@estdDate,@balance,@impsSame,@impsOther,@rtgsSame,@rtgsOther)";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankName", bankName));
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            parameterList.Add(new MySqlParameter("@estdDate", date));
            parameterList.Add(new MySqlParameter("@balance", 0));
            parameterList.Add(new MySqlParameter("@impsSame", IMPSChargesSame));
            parameterList.Add(new MySqlParameter("@impsOther", IMPSChargesOther));
            parameterList.Add(new MySqlParameter("@rtgsSame", RTGSChargesSame));
            parameterList.Add(new MySqlParameter("@rtgsOther", RTGSChargesOther));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
            string clerkId = $"{clerkName}@{bankName.Replace(" ","").ToLower()}";
            string doj = date;
            AddClerk(bankId, clerkName, clerkId,password,dob,address,mobileNumber,salary,doj);
            return clerkId;
        }
        public void AddClerk(string bankId, string clerkName, string clerkId,string password,string dob,string address,string mobileNumber,decimal salary,string doj)
        {
            string query = @"INSERT INTO Clerk(clerkName,ClerkId,bankId,Password,DOB,DOJ,Address,MobileNumber,Salary) 
                             VALUES(@clerkName,@clerkId,@bankId,@password,@dob,@doj,@address,@mobileNo,@salary)";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@clerkName", clerkName));
            parameterList.Add(new MySqlParameter("@clerkId", clerkId));
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            parameterList.Add(new MySqlParameter("@password", password));
            parameterList.Add(new MySqlParameter("@dob", dob));
            parameterList.Add(new MySqlParameter("@doj", doj));
            parameterList.Add(new MySqlParameter("@address", address));
            parameterList.Add(new MySqlParameter("mobileNo", mobileNumber));
            parameterList.Add(new MySqlParameter("@salary", salary));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public void Deposit(string bankName,string accountNumber,decimal amount)
        {
            if (!new ClerkService().IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            string typeOfTransaction = Enums.TransactionType.CREDIT.ToString();
            decimal balance = FetchAccountBalance(accountNumber);
            balance += amount;
            UpdateAccountBalance(accountNumber,balance);
            DateTime pointOfTime = DateTime.Now;
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string bankId = new ClerkService().GetBankId(bankName);
            string accountId = new ClerkService().GetAccountId(bankName,accountNumber);
            string transactionId = $"TXN{bankId}{accountId}{date}";
            InsertTransaction(bankId,accountNumber,transactionId,"-","-",typeOfTransaction,amount,pointOfTime);
        }
        public decimal FetchAccountBalance(string accountNumber)
        {
            string query = $"SELECT balance FROM Account WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            return (decimal)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public void Withdraw(string bankName,string accountNumber,decimal amount,string password)
        {
            if (!new ClerkService().IsAccountExist(bankName, accountNumber))
                throw new Exception("Acount does not exist!");
            if (FetchAccountPassword(accountNumber) != password)
                throw new Exception("Incorrect Password!");
            decimal balance = FetchAccountBalance(accountNumber);
            if ( balance < amount)
                throw new Exception("Insufficient balance!");
            string typeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            balance -= amount;
            UpdateAccountBalance(accountNumber,balance);
            DateTime pointOfTime = DateTime.Now;
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string bankId = new ClerkService().GetBankId(bankName);
            string accountId = new ClerkService().GetAccountId(bankName, accountNumber);
            string transactionId = $"TXN{bankId}{accountId}{date}";
            InsertTransaction(bankId,accountNumber,transactionId, "-", "-", typeOfTransaction, amount,pointOfTime);
        }
        public void Transfer(string senderBankName,string senderAccountNumber,string receiverBankName,string receiverAccountNumber,decimal amount,string password,Enums.TypeOfTransfer typeOfTransfer)
        {
            if (!new ClerkService().IsAccountExist(senderBankName, senderAccountNumber))
                throw new Exception("Acount does not exist!");
            if (!new ClerkService().IsAccountExist(receiverBankName, receiverAccountNumber))
                throw new Exception("The Account of receiver does not exist!");
            if (FetchAccountPassword(senderAccountNumber) != password)
                throw new Exception("Incorrect Password!");
            decimal balance = FetchAccountBalance(senderAccountNumber);
            if (balance < amount)
                throw new Exception("Insufficient balance!");
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string senderBankId = new ClerkService().GetBankId(senderBankName);
            string senderAccountId = new ClerkService().GetAccountId(senderBankName,senderAccountNumber);
            string transactionId = $"TXN{senderBankId}{senderAccountId}{date}";
            string receiverBankId = new ClerkService().GetBankId(senderBankName);
            string receiverAccountId = new ClerkService().GetAccountId(receiverBankName,receiverAccountNumber);
            string typeOfTransaction = Enums.TransactionType.DEBIT.ToString();
            DateTime pointOfTime = DateTime.Now;
            balance -= amount;
            UpdateAccountBalance(senderAccountNumber ,amount);
            InsertTransaction(senderBankName, senderAccountNumber, transactionId, senderAccountId, receiverAccountId, typeOfTransaction, amount, pointOfTime);
            decimal Charges;
            if (senderBankName == receiverBankName)
            {
                if(typeOfTransfer == Enums.TypeOfTransfer.IMPS)
                    Charges = new ClerkService().GetCharges(senderBankId,"impsSame") * amount / 100;
                else
                    Charges = new ClerkService().GetCharges(senderBankId,"rtgsSame") * amount / 100;
            }
            else
            {
                if(typeOfTransfer== Enums.TypeOfTransfer.IMPS)
                    Charges = new ClerkService().GetCharges(senderBankId,"impsOther") * amount / 100;
                else
                    Charges = new ClerkService().GetCharges(senderBankId,"rtgsOther") * amount / 100;
                
            }
            amount -= Charges;
            balance = FetchAccountBalance(receiverAccountNumber);
            UpdateAccountBalance(receiverAccountNumber, amount);
            InsertTransaction(receiverBankName,receiverAccountNumber,transactionId, senderAccountId, receiverAccountId, typeOfTransaction, amount, pointOfTime);
            balance = new ClerkService().FetchBankBalance(senderBankId);
            balance += Charges;
            new ClerkService().UpdateBankBalance(senderBankName,balance);
        }
        public  string GetAccountCurrencyType(string accountNumber)
        {
            string query = $"SELECT Currency FROM Account WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            return (string)new DatabaseService().ExecuteScalar(query,parameterList);
        }
        public void InsertTransaction(string bankId, string accountNumber, string transactionId, string senderAccountId, string receiverAccountId, string typeOfTransaction, decimal amount, DateTime pointOfTime)
        {
            string query = @"INSERT INTO Transaction(bankId,accountNumber,transactionId,senderAccountId,ReceiverAccountId,TransactionType,Amount,TransactionTime,Avl_Bal) 
                              VALUES(@bankId,@accountNo,@txnId,@senderAccId,@receiverAccId,@txnType,@amount,@time,@balance)";
            decimal bal = FetchAccountBalance(accountNumber);
            string balance = $"{bal.ToString()}({GetAccountCurrencyType(accountNumber)})";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@bankId", bankId));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            parameterList.Add(new MySqlParameter("@txnId", transactionId));
            parameterList.Add(new MySqlParameter("@senderAccId", senderAccountId));
            parameterList.Add(new MySqlParameter("@receiverAccId", receiverAccountId));
            parameterList.Add(new MySqlParameter("@txnType", typeOfTransaction));
            parameterList.Add(new MySqlParameter("@amount", amount));
            parameterList.Add(new MySqlParameter("@time", pointOfTime));
            parameterList.Add(new MySqlParameter("@balance", balance));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public void UpdateAccountBalance(string accountNumber, decimal balance)
        {
            string query = $"UPDATE Account SET balance = @balance WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@balance", balance));
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            new DatabaseService().ExecuteNonQuery(query,parameterList);
        }
        public List<string> TransactionHistory(string bankName,string accountNumber,string password)
        {
            var clerk = new ClerkService();
            if (!clerk.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            if (FetchAccountPassword(accountNumber)!=password)
                throw new Exception("Incorrect Password");
            return clerk.FetchAccountTransactions(accountNumber);
        }
        public string FetchAccountPassword(string accountNumber)
        {
            string query = $"SELECT  password FROM Account WHERE accountNumber = @accountNo";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@accountNo", accountNumber));
            return (string)new DatabaseService().ExecuteScalar(query,parameterList);
        }
    }
}

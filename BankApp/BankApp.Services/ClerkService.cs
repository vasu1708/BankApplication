using BankApp.Models;
using System;
using System.Collections.Generic;

namespace BankApp.Services
{
    public class ClerkService
    {
        public static Dictionary<string,ClerkService> Clerks= new Dictionary<string,ClerkService>();
        public string ClerkId { get; set; }
        public string Password { get; set; }
        public string BankName { get; set; }

        public static string GenerateAccountNumber(string bankName)
        {
            Random random = new Random();
            uint AccountNo = (UInt32)random.Next(111111111, 999999999);
            return AccountNo.ToString();
        }
        
        public string CreateAccount(string name, string mobileNumber,Enums.Gender gender,string address,string password)
        {
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string AccountId = $"{name.Substring(0, 3)}{date}";
            string AccountNumber = GenerateAccountNumber(this.BankName);
            string Password = password;
            DatabaseConnectionService.InsertIntoAccount(this.BankName,name,AccountId,AccountNumber,mobileNumber,gender,address,password);
            return AccountNumber;
        }
        public void DeleteAccount(string accountNumber)
        {
            if (!DatabaseConnectionService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseConnectionService.DeleteFromAccount(accountNumber);
        }
        public List<string> TransactionHistory(string accountNumber)
        {
            List<string> History = new List<string>();
            if (!DatabaseConnectionService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Acount does not exist");
            History = DatabaseConnectionService.FetchAccountTransactions(accountNumber);
            return History;
        }
        public void UpdateServiceCharges(decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            DatabaseConnectionService.UpdateCharges(this.BankName,chargeOfIMPSForSame, chargeOfIMPSForOther,chargeOfRTGSForSame, chargeOfRTGSForOther);
        }      
        public void RevertTransaction(string accountNumber, string transactionId)
        {
            if (!DatabaseConnectionService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseConnectionService.DeleteFromTransaction(transactionId);

        }
        public void UpdateCurrency(string accountNumber,Enums.CurrencyType currency)
        {
            if (!DatabaseConnectionService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
            string CurrencyType = DatabaseConnectionService.GetAccountCurrencyType(accountNumber);
            if (CurrencyType == currency.ToString())
                throw new Exception("New Currency is same as Existing!");
            DatabaseConnectionService.UpdateCurrency(accountNumber, currency.ToString());
            
            
        }
        public void UpdateAddress(string accountNumber,string address)
        {
            if (!DatabaseConnectionService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
              DatabaseConnectionService.UpdateAddress(accountNumber, address);
        }
        public void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            if (!DatabaseConnectionService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
            DatabaseConnectionService.UpdateMobileNumber(accountNumber, mobileNumber);
        }
    }
    
}

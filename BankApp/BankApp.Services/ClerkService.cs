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
            DatabaseService.InsertIntoAccount(this.BankName,name,AccountId,AccountNumber,mobileNumber,gender,address,password);
            return AccountNumber;
        }
        public void DeleteAccount(string accountNumber)
        {
            if (!DatabaseService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.DeleteFromAccount(accountNumber);
        }
        public List<string> TransactionHistory(string accountNumber)
        {
            List<string> History = new List<string>();
            if (!DatabaseService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Acount does not exist");
            History = DatabaseService.FetchAccountTransactions(accountNumber);
            return History;
        }
        public void UpdateServiceCharges(decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            DatabaseService.UpdateCharges(this.BankName,chargeOfIMPSForSame, chargeOfIMPSForOther,chargeOfRTGSForSame, chargeOfRTGSForOther);
        }      
        public void RevertTransaction(string accountNumber, string transactionId)
        {
            if (!DatabaseService.IsAccountExist(this.BankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.DeleteFromTransaction(transactionId);

        }
        public void UpdateCurrency(string accountNumber,Enums.CurrencyType currency)
        {
            if (!DatabaseService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
            string CurrencyType = DatabaseService.GetAccountCurrencyType(accountNumber);
            if (CurrencyType == currency.ToString())
                throw new Exception("New Currency is same as Existing!");
            DatabaseService.UpdateCurrency(accountNumber, currency.ToString());
            
            
        }
        public void UpdateAddress(string accountNumber,string address)
        {
            if (!DatabaseService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
              DatabaseService.UpdateAddress(accountNumber, address);
        }
        public void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            if (!DatabaseService.IsAccountExist(this.BankName, accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.UpdateMobileNumber(accountNumber, mobileNumber);
        }
    }
    
}

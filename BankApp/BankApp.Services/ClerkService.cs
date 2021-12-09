using BankApp.Models;
using System;
using System.Collections.Generic;

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
        
        public string CreateAccount(string bankName,string name, string mobileNumber,Enums.Gender gender,string address,string dob,string password)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string AccountId = $"{name.Substring(0, 3)}{date}";
            string doc = date;
            string AccountNumber = GenerateAccountNumber(bankName);
            string Password = password;
            DatabaseService.InsertIntoAccount(bankName,name,AccountId,AccountNumber,mobileNumber,dob,doc,gender,address,password);
            return AccountNumber;
        }
        public void DeleteAccount(string bankName,string accountNumber)
        {
            if (!DatabaseService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.DeleteFromAccount(accountNumber);
        }
        public List<string> TransactionHistory(string bankName,string accountNumber)
        {
            List<string> History = new List<string>();
            if (!DatabaseService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Acount does not exist");
            History = DatabaseService.FetchAccountTransactions(accountNumber);
            return History;
        }
        public void UpdateServiceCharges(string bankName,decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            DatabaseService.UpdateCharges(bankName,chargeOfIMPSForSame, chargeOfIMPSForOther,chargeOfRTGSForSame, chargeOfRTGSForOther);
        }      
        public void RevertTransaction(string bankName,string accountNumber, string transactionId)
        {
            if (!DatabaseService.IsAccountExist(bankName,accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.DeleteFromTransaction(transactionId);

        }
        public void UpdateCurrency(string bankName,string accountNumber,Enums.CurrencyType currency)
        {
            if (!DatabaseService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            string CurrencyType = DatabaseService.GetAccountCurrencyType(accountNumber);
            if (CurrencyType == currency.ToString())
                throw new Exception("New Currency is same as Existing!");
            decimal ConversionRatio = 1;
            if (currency == Enums.CurrencyType.INUSD)
                ConversionRatio = 1 / 70;
            else if (Enums.CurrencyType.INR == currency)
                ConversionRatio = 70;
            decimal Balance = DatabaseService.FetchAccountBalance(accountNumber);
            Balance *= ConversionRatio;
            DatabaseService.UpdateCurrency(accountNumber, currency.ToString(),Balance);
            
            
        }
        
        public void UpdateAddress(string bankName,string accountNumber,string address)
        {
            if (!DatabaseService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
              DatabaseService.UpdateAddress(accountNumber, address);
        }
        public void UpdateMobileNumber(string bankName,string accountNumber,string mobileNumber)
        {
            if (!DatabaseService.IsAccountExist(bankName, accountNumber))
                throw new Exception("Account does not exist");
            DatabaseService.UpdateMobileNumber(accountNumber, mobileNumber);
        }
    }
    
}

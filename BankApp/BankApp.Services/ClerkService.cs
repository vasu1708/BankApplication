using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Models;

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
            CustomerService.Banks[bankName].AccountNumberGenerator += 1;
            UInt32 AccountNumber = CustomerService.Banks[bankName].AccountNumberGenerator;
            return AccountNumber.ToString();
        }
        
        public string CreateAccount(string name, string mobileNumber,string gender,Address address,string password)
        {
            if (CustomerService.Banks[this.BankName].Accounts.ContainsKey(name))
                throw new Exception("Account exist on this name");
            Account account = new Account();
            account.AccountHolderName = name;
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            account.AccountId = $"{name.Substring(0, 3)}{date}";
            account.AccountNumber = GenerateAccountNumber(this.BankName);
            account.MobileNumber = mobileNumber;
            account.Gender = gender;
            account.Address = address;
            //account.Message = "Your account is created Successfully!";
            account.Balance = 0;
            account.Password = password;
            CustomerService.Banks[this.BankName].Accounts[account.AccountNumber] = account;
            return account.AccountNumber;
        }
        public void DeleteAccount(string accountNumber)
        {
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts.Remove(accountNumber);
        }
        public List<string> TransactionHistory(string accountNumber)
        {
            List<string> History = new List<string>();
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Acount does not exist");
            foreach (var TxnKey in CustomerService.Banks[BankName].Transactions.Keys)
            {
                if (CustomerService.Banks[BankName].Transactions[TxnKey].SenderAccountId == CustomerService.Banks[BankName].Accounts[accountNumber].AccountId || CustomerService.Banks[BankName].Transactions[TxnKey].ReceiverAccountId == CustomerService.Banks[BankName].Accounts[accountNumber].AccountId)
                {
                    History.Add(TxnKey);
                }
            }
            return History;
        }
        public void UpdateServiceCharges(decimal chargeOfIMPSForSame,decimal chargeOfRTGSForSame, decimal chargeOfIMPSForOther, decimal chargeOfRTGSForOther)
        {
            CustomerService.Banks[this.BankName].IMPSForSameBank = chargeOfIMPSForSame;
            CustomerService.Banks[this.BankName].RTGSForSameBank = chargeOfRTGSForSame;
            CustomerService.Banks[this.BankName].IMPSForOtherBank = chargeOfIMPSForOther;
            CustomerService.Banks[this.BankName].RTGSForOtherBank = chargeOfRTGSForOther;
        }      
        public void UpdateMobileNumber(string accountNumber,string mobileNumber)
        {
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts[accountNumber].MobileNumber =  mobileNumber;
        }
        public void UpdateAddress(string accountNumber,Address address)
        {
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts[accountNumber].Address = address;

        }
        public void RevertTransaction(string accountNumber, string transactionId)
        {
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Account does not exist");
            foreach (var TxnId in CustomerService.Banks[this.BankName].Transactions.Keys)
            {
                if (TxnId == transactionId)
                {
                    CustomerService.Banks[this.BankName].Transactions.Remove(TxnId);
                    break;
                }
                    
            }

        }
        public void UpdateCurrency(Enums.CurrencyType currency)
        {
            if (CustomerService.Banks[this.BankName].Currency == currency)
                throw new Exception("New Currency is same as Existing!");
            CustomerService.Banks[this.BankName].Currency = currency;
            decimal ConversionRatio = 1;

            if (currency == Enums.CurrencyType.INUSD)
                ConversionRatio = 1 / 70;
            else if (currency == Enums.CurrencyType.INR)
                ConversionRatio = 70;
            CustomerService.Banks[this.BankName].Balance *= ConversionRatio;
            foreach (var account in CustomerService.Banks[this.BankName].Accounts.Keys)
            {
                CustomerService.Banks[this.BankName].Accounts[account].Balance *= ConversionRatio;
            }
                
        }
    }
    
}

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

        public static string GenerateAccountNumber(string BankName)
        {
            CustomerService.Banks[BankName].AccountNumberGenerator += 1;
            UInt32 AccountNumber = CustomerService.Banks[BankName].AccountNumberGenerator;
            return AccountNumber.ToString();
            
        }
        
        public string CreateAccount(string Name, string MobileNumber,string Gender,Address Address,string Password)
        {
            if (CustomerService.Banks[this.BankName].Accounts.ContainsKey(Name))
                throw new Exception("Account exist on this name");
            Account account = new Account();
            account.AccountHolderName = Name;
            string date = DateTime.Now.ToString("dd_MM_yyyy");
            account.AccountId = $"{Name.Substring(0, 3)}{date}";
            account.AccountNumber = GenerateAccountNumber(this.BankName);
            account.MobileNumber = MobileNumber;
            account.Gender = Gender;
            account.Address = Address;
            account.Message = "Your account is created Successfully!";
            account.Balance = 0;
            account.Password = Password;
            CustomerService.Banks[this.BankName].Accounts[account.AccountNumber] = account;
            return account.AccountNumber;
        }
        public void DeleteAccount(string AccountNumber)
        {
            if (CustomerService.Banks[this.BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts.Remove(AccountNumber);
        }
        public List<string> TransactionHistory(string AccountNumber)
        {
            List<string> History = new List<string>();
            Console.Write("Account Number : ");
            string accountNumber = Console.ReadLine();
            if (!CustomerService.Banks[this.BankName].Accounts.ContainsKey(accountNumber))
                throw new Exception("Acount does not exist");
            foreach (var TxnKey in CustomerService.Banks[BankName].Transactions.Keys)
            {
                if (CustomerService.Banks[BankName].Transactions[TxnKey].SenderAccountId == CustomerService.Banks[BankName].Accounts[AccountNumber].AccountId || CustomerService.Banks[BankName].Transactions[TxnKey].ReceiverAccountId == CustomerService.Banks[BankName].Accounts[AccountNumber].AccountId)
                {
                    History.Add(TxnKey);
                }
            }
            return History;
        }
        public void UpdateServiceChargeForSameBanks(decimal IMPSForSame,decimal RTGSForSame)
        {
            CustomerService.Banks[this.BankName].IMPSForSameBank = IMPSForSame;
            CustomerService.Banks[this.BankName].RTGSForSameBank = RTGSForSame;
        }
        public void UpdateServiceChargeForDifferentBanks(decimal IMPSForOther, decimal RTGSForOther)
        {
            CustomerService.Banks[this.BankName].IMPSForOtherBank = IMPSForOther;
            CustomerService.Banks[this.BankName].RTGSForOtherBank = RTGSForOther;
        } 
        public void UpdateMobileNumber(string AccountNumber,string MobileNumber)
        {
            if (CustomerService.Banks[this.BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts[AccountNumber].MobileNumber = MobileNumber;
        }
        public void UpdateAddress(string AccountNumber,Address Address)
        {
            if (CustomerService.Banks[this.BankName].Accounts.ContainsKey(AccountNumber))
                throw new Exception("Account does not exist");
            CustomerService.Banks[this.BankName].Accounts[AccountNumber].Address = Address;

        }
        public void RevertTransaction(string AccountNumber, string TransactionId)
        {
            foreach(var TxnId in CustomerService.Banks[this.BankName].Transactions.Keys)
            {
                if (TxnId == TransactionId)
                {
                    CustomerService.Banks[this.BankName].Transactions.Remove(TxnId);
                    break;
                }
                    
            }

        }
        public void UpdateCurrency(string AccountNumber,string Currency)
        {

        }
    }
    
}

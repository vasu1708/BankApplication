using System;
using System.Collections.Generic;

namespace BankApp.Models
{
    public class Bank
    {
        public string BankId { get; set; }
        public string BankName { get; set; }
        public  UInt32 AccountNumberGenerator { get; set; }
        public decimal IMPSForSameBank { get; set; }
        public decimal RTGSForSameBank { get; set; }
        public decimal IMPSForOtherBank { get; set; }
        public Enums.CurrencyType Currency { get; set; }
        public decimal RTGSForOtherBank { get; set; }
        public decimal Balance { get; set; }
        public Dictionary<string,Account> Accounts { get; set; }
        public Dictionary<string,Transaction> Transactions { get; set; }
    }
}

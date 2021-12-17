using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public decimal AccountBalance { get; set; }
        public string AccountPassword { get; set; }
        public Enums.Gender Gender { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfBirth { get; set; }
        public Enums.CurrencyType Currency { get; set; }
        public DateTime AccountCreationDate { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Bank Bank { get; set; }
    }
}

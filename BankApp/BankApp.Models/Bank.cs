using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
     public class Bank
    {
        
        public string BankId { get; set; }
        public string BankName { get; set; }
        public List<Account> Accounts { get; set; }
        public decimal BankBalance { get; set; }
        public decimal SameBankIMPS { get; set; }
        public decimal SameBankRTGS { get; set; }
        public decimal OtherBankIMPS { get; set; }
        public decimal OtherBankRTGS { get; set; }
        public DateTime EstablishedDate { get; set; }
        public List<Clerk> Clerks { get; set; }
    }
}

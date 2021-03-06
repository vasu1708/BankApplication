using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string SenderAccountId { get; set; }
        public string ReceiverAccountId { get; set; }
        public string TypeOfTransaction { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeOfTransaction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; }
        [ForeignKey("BankId")]
        public string BankId { get; set; }
        [ForeignKey("AccountId")]
        public string Accountid { get; set; }
        public string? SenderAccountId { get; set; }
        public string? ReceiverAccountId { get; set; }
        public Enums.TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeOfTransaction { get; set; }
    }
}

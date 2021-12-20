﻿using System;
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
        public string TransactionId { get; set; }
        public string SenderAccountId { get; set; }
        public string ReceiverAccountId { get; set; }
        public Enums.TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeOfTransaction { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
        public Enums.CurrencyType Currency { get; set; }
    }
}

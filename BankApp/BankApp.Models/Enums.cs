using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Enums
    {
        
        public enum TransactionType:byte { 
            Credit,
            Debit
        }
        public enum Login
        {
            AccountHolder = 1,
                Bankstaff,
                Exit
        }
        public enum CustomerOperation{
            Deposit = 1,
            Withdraw,
            Transfer,
            TransactionHistory,
            Exit
        }
        public enum ClerkOperation
        {
            CreateAccount = 1,
            DeleteAccount,
            TransactionHistory,
            Logout
        }

    }
}

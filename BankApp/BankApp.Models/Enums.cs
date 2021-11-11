using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Enums
    {

        public enum Action{
            NEWBANK = 1,
            LOGIN
        }
        public enum CurrencyType{ 
            INR,
            INUSD
        }
        public enum TransactionType{ 
            CREDIT,
            DEBIT
        }
        public enum Login
        {
            ACCOUNTHOLDER = 1,
                BANKSTAFF,
                EXIT
        }
        public enum CustomerOperation{
            DEPOSIT = 1,
            WITHDRAW,
            TRANSFER,
            TRANSACTIONHISTORY,
            EXIT
        }
        public enum ClerkOperation
        {
            CREATEACCOUNT = 1,
            UPDATEACCOUNT,
            DELETEACCOUNT,
            TRANSACTIONHISTORY,
            REVERTTRANSACTION,
            UPDATECHARGES,
            UPDATECURRENCY,
            LOGOUT
        }
        public enum UpdateAccount
        {
            ADDRESS = 1,
            MOBILENUMBER
        }
        public enum TypeOfTransfer {
            RTGS,IMPS
        }

    }
}

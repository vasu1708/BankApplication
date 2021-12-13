using BankApp.Models;
using BankApp.Services;
using System;
using System.Collections.Generic;

namespace BankApp.CLI
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string bankName,message,accountNumber,password;
            Enums.Action Action;
            while (true)
            {
                message = "\n1.New Bank Setup\n2.Login\nEnter Operation No : ";
                Action = IOMethods.GetEnum<Enums.Action>(message);
                try {
                    switch (Action)
                    {
                        case Enums.Action.NEWBANK:                        
                            bankName = IOMethods.GetName("Bank name To Setup : ");
                            /*IOMethods.DisplayOutputLine("Atleast one clerk should be added to maintain the bank");*/
                            string ClerkName = IOMethods.GetName("Add Clerk name : ");
                            password = IOMethods.GetString("Set password : ");
                            string dob = IOMethods.GetDOB("Enter date Of Birth(DD-MM-YYYY) : ");
                            decimal salary = IOMethods.GetDecimal("Enter Salary(INR) : ");
                            string address = IOMethods.GetAddress();
                            string mobileNumber = IOMethods.GetMobileNumber("Enter Mobile No : ");
                            string Id = new CustomerService().AddBank(bankName,ClerkName,dob,address,password,mobileNumber,salary);
                            IOMethods.DisplayOutputLine($"{bankName} is eshtablished, Remember! clerk Id : {Id}");
                            break;

                        case Enums.Action.LOGIN:                        
                            bankName = IOMethods.GetName("Enter Bank name : ");
                            if (!new ClerkService().IsBankExist(bankName))
                            {
                                IOMethods.DisplayOutputLine("No such Bank exist!");
                                break;
                            }
                            bool LoginFlag = true;
                            Enums.Login login;
                            while (LoginFlag)
                            {
                                message = "As:\n1.AccountHolder\n2.Bankstaff\n3.Exit\nEnter Operation No : ";                                
                                login = IOMethods.GetEnum<Enums.Login>(message);
                                try
                                {
                                    switch (login)
                                    {
                                        case Enums.Login.ACCOUNTHOLDER:
                                            Enums.CustomerOperation CustOp;
                                            decimal amount;
                                            bool customerFlag = true;
                                            accountNumber = IOMethods.GetString("Account Number : ");
                                            CustomerService customer = new CustomerService();
                                            while (customerFlag)
                                            {
                                                message = "\n1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction history\n5.Exit\nEnter Operation No : ";
                                                CustOp = IOMethods.GetEnum<Enums.CustomerOperation>(message);
                                                try
                                                {
                                                    switch (CustOp)
                                                    {
                                                        case Enums.CustomerOperation.DEPOSIT:
                                                            amount = IOMethods.GetDecimal($"Enter amount ({customer.GetAccountCurrencyType(accountNumber)}) : ");
                                                            customer.Deposit(bankName, accountNumber, amount);
                                                            IOMethods.DisplayOutputLine("Successfully! Deposited your amount");
                                                            break;

                                                        case Enums.CustomerOperation.WITHDRAW:
                                                            amount = IOMethods.GetDecimal($"Enter amount ({customer.GetAccountCurrencyType(accountNumber)}) : ");
                                                            password = IOMethods.GetString("Account password : ");
                                                            customer.Withdraw(bankName, accountNumber, amount, password);
                                                            IOMethods.DisplayOutputLine("Successfully! withdrawed your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSFER:
                                                            amount = IOMethods.GetDecimal($"Enter amount ({customer.GetAccountCurrencyType(accountNumber)}) : ");
                                                            string ReceiverBankName = IOMethods.GetName("Reciever Bank name : ");
                                                            string ReceiverAccountNumber = IOMethods.GetName("Receiver Account Number : ");
                                                            Enums.TypeOfTransfer TypeOfTransfer = IOMethods.GetEnum<Enums.TypeOfTransfer>("Type of Transfer (IMPS/RTGS) : ");
                                                            password = IOMethods.GetString("Account password : ");
                                                            customer.Transfer(bankName, accountNumber, ReceiverBankName, ReceiverAccountNumber, amount, password, TypeOfTransfer);
                                                            IOMethods.DisplayOutputLine("Successfully! Transfered your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSACTIONHISTORY:
                                                            password = IOMethods.GetString("Account password : ");
                                                            List<string> history;
                                                            history = customer.TransactionHistory(bankName, accountNumber, password);
                                                            IOMethods.DisplayOutput(history);
                                                            break;

                                                        case Enums.CustomerOperation.EXIT:
                                                            customerFlag = false;
                                                            break;
                                                        default:
                                                            IOMethods.DisplayOutputLine("Wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    IOMethods.DisplayOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.BANKSTAFF:
                                            Id = IOMethods.GetLowerCase("Clerk ID : ");
                                            password = IOMethods.GetString("password : ");
                                            ClerkService clerk = new ClerkService();
                                            if (password!=clerk.FetchClerkPassword(Id) || password=="")
                                            {
                                                IOMethods.DisplayOutputLine("Invalid Id or password");
                                                break;
                                            }
                                            
                                            string name;
                                            Enums.Gender gender;
                                            bool clerkFlag = true;
                                            Enums.ClerkOperation ClerkOp;
                                            while (clerkFlag)
                                            {
                                                message = "\n1.Create Account\n2.Update Account\n3.Delete Account\n4.Transaction history\n5.Revert Transaction\n6.Update Charges\n7.Update Currency\n8.Logout\nEnter Operation No : ";
                                                ClerkOp = IOMethods.GetEnum<Enums.ClerkOperation>(message);
                                                try
                                                {
                                                    switch (ClerkOp)
                                                    {
                                                        case Enums.ClerkOperation.CREATEACCOUNT:
                                                            name = IOMethods.GetString("name : ");
                                                            mobileNumber = IOMethods.GetMobileNumber("Mobile No : ");
                                                            gender = IOMethods.GetEnum<Enums.Gender>("gender (M/F/O) : ");
                                                            address = IOMethods.GetAddress();
                                                            password = IOMethods.GetString("set Account password : ");
                                                            dob = IOMethods.GetDOB("Enter date of Birth(DD-MM-YYYY) : ");
                                                            accountNumber = clerk.CreateAccount(bankName,name, mobileNumber, gender, address,dob, password);
                                                            IOMethods.DisplayOutputLine($"Account is created, Account Number is : {accountNumber}");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATEACCOUNT:
                                                            message = "\n1.Address\n2.Mobile Number\nEnter Operarion No : ";
                                                            Enums.UpdateAccount update = IOMethods.GetEnum<Enums.UpdateAccount>(message);
                                                            switch (update)
                                                            {
                                                                case Enums.UpdateAccount.ADDRESS:
                                                                    accountNumber = IOMethods.GetString("Account Number : ");
                                                                    address = IOMethods.GetAddress();
                                                                    clerk.UpdateAddress(bankName,accountNumber, address);
                                                                    IOMethods.DisplayOutputLine("Successfully! updated the address");
                                                                    break;
                                                                case Enums.UpdateAccount.MOBILENUMBER:
                                                                    accountNumber = IOMethods.GetString("Account Number : ");
                                                                    mobileNumber = IOMethods.GetMobileNumber("New Mobile Number : ");
                                                                    clerk.UpdateMobileNumber(bankName,accountNumber,mobileNumber);
                                                                    IOMethods.DisplayOutputLine("Successfully! Updated the mobile number");
                                                                    break;
                                                                default:
                                                                    IOMethods.DisplayOutputLine("Wrong Input!");
                                                                    break;
                                                            }
                                                            break;

                                                        case Enums.ClerkOperation.DELETEACCOUNT:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            clerk.DeleteAccount(bankName,accountNumber);
                                                            IOMethods.DisplayOutputLine("Successfully! Deleted the account");
                                                            break;

                                                        case Enums.ClerkOperation.TRANSACTIONHISTORY:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            List<string> history;
                                                            history = clerk.TransactionHistory(bankName,accountNumber);
                                                            IOMethods.DisplayOutput(history);
                                                            break;

                                                        case Enums.ClerkOperation.REVERTTRANSACTION:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            string transactionId = IOMethods.GetString("Account Number : ");
                                                            clerk.RevertTransaction(bankName,accountNumber, transactionId);
                                                            IOMethods.DisplayOutputLine("Successfully! Reverted the transaction");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECHARGES:
                                                            decimal ImpsSame, ImpsDiff, RtgsSame,RtgsDiff;
                                                            ImpsSame = IOMethods.GetDecimal("New IMPS Charges for Same Bank : ");
                                                            ImpsDiff = IOMethods.GetDecimal("New IMPS Charges for Diff Bank : ");
                                                            RtgsSame = IOMethods.GetDecimal("New RTGS Charges for Same Bank : ");
                                                            RtgsDiff = IOMethods.GetDecimal("New RTGS Charges for Same Bank : ");
                                                            clerk.UpdateServiceCharges(bankName,ImpsSame, RtgsSame, ImpsDiff, RtgsDiff);
                                                            IOMethods.DisplayOutputLine("Successfully! Updates the charges");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECURRENCY:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            Enums.CurrencyType currency = IOMethods.GetEnum<Enums.CurrencyType>("New Currency (INR/INUSD) : ");
                                                            clerk.UpdateCurrency(bankName,accountNumber,currency);
                                                            IOMethods.DisplayOutputLine("Successfully! Updated the Currency");
                                                            break;

                                                        case Enums.ClerkOperation.LOGOUT:
                                                            IOMethods.DisplayOutputLine("Logged out!");
                                                            clerkFlag = false;
                                                            break;

                                                        default:
                                                            IOMethods.DisplayOutputLine("wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    IOMethods.DisplayOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.EXIT:
                                            LoginFlag = false;
                                            break;

                                        default:
                                            IOMethods.DisplayOutputLine ("Wrong input!");
                                            break;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    IOMethods.DisplayOutputLine(exception.Message);
                                }
                            }
                            break;
                        default:
                            IOMethods.DisplayOutputLine("Wrong input!");
                            break;
                    }
                }
                catch(Exception exception)
                {
                    IOMethods.DisplayOutputLine(exception.Message);
                }                
            }
        }
    }
}

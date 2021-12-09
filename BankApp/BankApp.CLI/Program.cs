﻿using BankApp.Models;
using BankApp.Services;
using System;
using System.Collections.Generic;

namespace BankApp.CLI
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string BankName,Message,AccountNumber,Password;
            Enums.Action Action;
            CustomerService customer = new CustomerService();
            while (true)
            {
                Message = "\n1.New Bank Setup\n2.Login\nEnter Operation No : ";
                Action = IOMethods.GetEnum<Enums.Action>(Message);
                try {
                    switch (Action)
                    {
                        case Enums.Action.NEWBANK:                        
                            BankName = IOMethods.GetName("Bank Name To Setup : ");
                            string ClerkName = IOMethods.GetName("Add Clerk Name : ");
                            Password = IOMethods.GetString("Set Password : ");
                            string dob = IOMethods.GetDOB("Enter Date Of Birth(DD-MM-YYYY) : ");
                            decimal salary = IOMethods.GetDecimal("Enter Salary(INR) : ");
                            string Address = IOMethods.GetAddress();
                            string MobileNumber = IOMethods.GetMobileNumber("Enter Mobile No : ");
                            string Id = customer.AddBank(BankName,ClerkName,dob,Address,Password,MobileNumber,salary);
                            IOMethods.DisplayOutputLine($"{BankName} is eshtablished, Remember! clerk Id : {Id}");
                            break;

                        case Enums.Action.LOGIN:                        
                            BankName = IOMethods.GetName("Enter Bank Name : ");
                            if (!DatabaseService.IsBankExist(BankName))
                            {
                                IOMethods.DisplayOutputLine("No such Bank exist!");
                                break;
                            }
                            bool LoginFlag = true;
                            Enums.Login Login;
                            while (LoginFlag)
                            {
                                Message = "As:\n1.AccountHolder\n2.Bankstaff\n3.Exit\nEnter Operation No : ";                                
                                Login = IOMethods.GetEnum<Enums.Login>(Message);
                                try
                                {
                                    switch (Login)
                                    {
                                        case Enums.Login.ACCOUNTHOLDER:
                                            Enums.CustomerOperation CustOp;
                                            decimal Amount;
                                            bool CustomerFlag = true;
                                            AccountNumber = IOMethods.GetString("Account Number : ");
                                            while (CustomerFlag)
                                            {
                                                Message = "\n1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction History\n5.Exit\nEnter Operation No : ";
                                                CustOp = IOMethods.GetEnum<Enums.CustomerOperation>(Message);
                                                try
                                                {
                                                    switch (CustOp)
                                                    {
                                                        case Enums.CustomerOperation.DEPOSIT:
                                                            Amount = IOMethods.GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            customer.Deposit(BankName, AccountNumber, Amount);
                                                            IOMethods.DisplayOutputLine("Successfully! Deposited your amount");
                                                            break;

                                                        case Enums.CustomerOperation.WITHDRAW:
                                                            Amount = IOMethods.GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            Password = IOMethods.GetString("Account Password : ");
                                                            customer.Withdraw(BankName, AccountNumber, Amount, Password);
                                                            IOMethods.DisplayOutputLine("Successfully! withdrawed your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSFER:
                                                            Amount = IOMethods.GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            string ReceiverBankName = IOMethods.GetName("Reciever Bank Name : ");
                                                            string ReceiverAccountNumber = IOMethods.GetName("Receiver Account Number : ");
                                                            Enums.TypeOfTransfer TypeOfTransfer = IOMethods.GetEnum<Enums.TypeOfTransfer>("Type of Transfer (IMPS/RTGS) : ");
                                                            Password = IOMethods.GetString("Account Password : ");
                                                            customer.Transfer(BankName, AccountNumber, ReceiverBankName, ReceiverAccountNumber, Amount, Password, TypeOfTransfer);
                                                            IOMethods.DisplayOutputLine("Successfully! Transfered your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSACTIONHISTORY:
                                                            Password = IOMethods.GetString("Account Password : ");
                                                            List<string> History;
                                                            History = customer.TransactionHistory(BankName, AccountNumber, Password);
                                                            IOMethods.DisplayOutput(History);
                                                            break;

                                                        case Enums.CustomerOperation.EXIT:
                                                            CustomerFlag = false;
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
                                            Password = IOMethods.GetString("Password : ");
                                            if (Password!=DatabaseService.FetchClerkPassword(Id) || Password=="")
                                            {
                                                IOMethods.DisplayOutputLine("Invalid Id or Password");
                                                break;
                                            }
                                            ClerkService clerk = new ClerkService();
                                            string Name;
                                            Enums.Gender Gender;
                                            bool ClerkFlag = true;
                                            string address;
                                            Enums.ClerkOperation ClerkOp;
                                            while (ClerkFlag)
                                            {
                                                Message = "\n1.Create Account\n2.Update Account\n3.Delete Account\n4.Transaction History\n5.Revert Transaction\n6.Update Charges\n7.Update Currency\n8.Logout\nEnter Operation No : ";
                                                ClerkOp = IOMethods.GetEnum<Enums.ClerkOperation>(Message);
                                                try
                                                {
                                                    switch (ClerkOp)
                                                    {
                                                        case Enums.ClerkOperation.CREATEACCOUNT:
                                                            Name = IOMethods.GetString("Name : ");
                                                            MobileNumber = IOMethods.GetMobileNumber("Mobile No : ");
                                                            Gender = IOMethods.GetEnum<Enums.Gender>("Gender (M/F/O) : ");
                                                            address = IOMethods.GetAddress();
                                                            Password = IOMethods.GetString("set Account Password : ");
                                                            dob = IOMethods.GetDOB("Enter Date of Birth(DD-MM-YYYY)");
                                                            AccountNumber = clerk.CreateAccount(BankName,Name, MobileNumber, Gender, address,dob, Password);
                                                            IOMethods.DisplayOutputLine($"Account is created, Account Number is : {AccountNumber}");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATEACCOUNT:
                                                            Message = "\n1.Address\n2.Mobile Number\nEnter Operarion No : ";
                                                            Enums.UpdateAccount update = IOMethods.GetEnum<Enums.UpdateAccount>(Message);
                                                            switch (update)
                                                            {
                                                                case Enums.UpdateAccount.ADDRESS:
                                                                    AccountNumber = IOMethods.GetString("Account Number : ");
                                                                    address = IOMethods.GetAddress();
                                                                    clerk.UpdateAddress(BankName,AccountNumber, address);
                                                                    IOMethods.DisplayOutputLine("Successfully! updated the address");
                                                                    break;
                                                                case Enums.UpdateAccount.MOBILENUMBER:
                                                                    AccountNumber = IOMethods.GetString("Account Number : ");
                                                                    MobileNumber = IOMethods.GetMobileNumber("New Mobile Number : ");
                                                                    clerk.UpdateMobileNumber(BankName,AccountNumber,MobileNumber);
                                                                    IOMethods.DisplayOutputLine("Successfully! Updated the mobile number");
                                                                    break;
                                                                default:
                                                                    IOMethods.DisplayOutputLine("Wrong Input!");
                                                                    break;
                                                            }
                                                            break;

                                                        case Enums.ClerkOperation.DELETEACCOUNT:
                                                            AccountNumber = IOMethods.GetString("Account Number : ");
                                                            clerk.DeleteAccount(BankName,AccountNumber);
                                                            IOMethods.DisplayOutputLine("Successfully! Deleted the account");
                                                            break;

                                                        case Enums.ClerkOperation.TRANSACTIONHISTORY:
                                                            AccountNumber = IOMethods.GetString("Account Number : ");
                                                            List<string> History;
                                                            History = clerk.TransactionHistory(BankName,AccountNumber);
                                                            IOMethods.DisplayOutput(History);
                                                            break;

                                                        case Enums.ClerkOperation.REVERTTRANSACTION:
                                                            AccountNumber = IOMethods.GetString("Account Number : ");
                                                            string TransactionId = IOMethods.GetString("Account Number : ");
                                                            clerk.RevertTransaction(BankName,AccountNumber, TransactionId);
                                                            IOMethods.DisplayOutputLine("Successfully! Reverted the transaction");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECHARGES:
                                                            decimal ImpsSame, ImpsDiff, RtgsSame,RtgsDiff;
                                                            ImpsSame = IOMethods.GetDecimal("New IMPS Charges for Same Bank : ");
                                                            ImpsDiff = IOMethods.GetDecimal("New IMPS Charges for Diff Bank : ");
                                                            RtgsSame = IOMethods.GetDecimal("New RTGS Charges for Same Bank : ");
                                                            RtgsDiff = IOMethods.GetDecimal("New RTGS Charges for Same Bank : ");
                                                            clerk.UpdateServiceCharges(BankName,ImpsSame, RtgsSame, ImpsDiff, RtgsDiff);
                                                            IOMethods.DisplayOutputLine("Successfully! Updates the charges");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECURRENCY:
                                                            AccountNumber = IOMethods.GetString("Account Number : ");
                                                            Enums.CurrencyType currency = IOMethods.GetEnum<Enums.CurrencyType>("New Currency (INR/INUSD) : ");
                                                            clerk.UpdateCurrency(BankName,AccountNumber,currency);
                                                            IOMethods.DisplayOutputLine("Successfully! Updated the Currency");
                                                            break;

                                                        case Enums.ClerkOperation.LOGOUT:
                                                            IOMethods.DisplayOutputLine("Logged out!");
                                                            ClerkFlag = false;
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

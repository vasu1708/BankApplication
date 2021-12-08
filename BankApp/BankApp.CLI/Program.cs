using BankApp.Models;
using BankApp.Services;
using System;
using System.Collections.Generic;

namespace BankApp.CLI
{
    class Program
    {
        static void DisplayOutput(List<string> history)
        {
            DisplayOutputLine("<---TRANSACTION HISTORY--->");
            if (history.Count == 0)
                DisplayOutputLine("No Transactions yet!");
            else
            foreach (var Txn in history)
            {
                DisplayOutputLine($"TransactionId SenderId ReceiverId Type Amount Time Avl.Bal");
                DisplayOutputLine(Txn);
            }
        }
        static void DisplayOutput(string message)
        {
            Console.Write(message);
        }
        static void DisplayOutputLine(string message)
        {
            Console.WriteLine(message);
        }
        static string GetString(string message)
        {
            DisplayOutput(message);
            return Console.ReadLine();
        }
        static string GetLowerCase(string message)
        {
            return GetString(message).ToLower();
        }
        static string GetLowerCaseWithoutSpaces(string message)
        {
            return GetLowerCase(message).Replace(" ", "");
        }
        static decimal GetDecimal(string message)
        {
            if (Decimal.TryParse(GetString(message), out decimal value))
                return value;
            DisplayOutputLine("Invalid input!");
            return GetDecimal(message);
        }
        static int GetInteger(string message)
        {
            if (int.TryParse(GetString(message), out int value))
                return value;
            DisplayOutputLine("Invalid input!");
            return GetInteger(message);
        }
        static T GetEnum<T>(string message)
            where T : struct
        {
            if (Enum.TryParse<T>(GetString(message), out T value))
                return value;
            DisplayOutputLine("Wrong option!");
            return GetEnum<T>(message);
        }
        static string GetAddress()
        {
            
            string ColonyName = GetString("Colony Name : ");
            int StreetNo = GetInteger("Street No : ");
            string HouseNo = GetString("House No : ");
            string DistrictName = GetString("District Name : ");
            string StateName = GetString("State Name : ");
            return $"{ColonyName}\nStreet No:{StreetNo}\n{HouseNo}\n{DistrictName}\n{StateName}";
        }
        static void Main(string[] args)
        {
            string BankName,Message,AccountNumber,Password;
            Enums.Action Action;
            CustomerService customer = new CustomerService();
            while (true)
            {
                Message = "\n1.New Bank Setup\n2.Login\nEnter Operation No : ";
                Action = GetEnum<Enums.Action>(Message);
                try {
                    switch (Action)
                    {
                        case Enums.Action.NEWBANK:                        
                            BankName = GetLowerCaseWithoutSpaces("Bank Name To Setup : ");
                            if (BankName.Length<=3)
                            {
                                DisplayOutputLine("Bank Name is too short!");
                                break;
                            }
                            string ClerkName = GetLowerCaseWithoutSpaces("Add Clerk Name : ");
                            Password = GetString("Set Password : ");
                            string Id = customer.AddBank(BankName,ClerkName,Password);
                            DisplayOutputLine($"{BankName} is eshtablished, Remember! clerk Id : {Id}");
                            break;

                        case Enums.Action.LOGIN:                        
                            BankName = GetLowerCaseWithoutSpaces("Enter Bank Name : ");
                            if (!DatabaseService.IsBankExist(BankName))
                            {
                                DisplayOutputLine("Bank doesn't exist!");
                                break;
                            }
                            bool LoginFlag = true;
                            Enums.Login Login;
                            while (LoginFlag)
                            {
                                Message = "As:\n1.AccountHolder\n2.Bankstaff\n3.Exit\nEnter Operation No : ";                                
                                Login = GetEnum<Enums.Login>(Message);
                                try
                                {
                                    switch (Login)
                                    {
                                        case Enums.Login.ACCOUNTHOLDER:
                                            Enums.CustomerOperation CustOp;
                                            decimal Amount;
                                            bool CustomerFlag = true;
                                            AccountNumber = GetString("Account Number : ");
                                            while (CustomerFlag)
                                            {
                                                Message = "\n1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction History\n5.Exit\nEnter Operation No : ";
                                                CustOp = GetEnum<Enums.CustomerOperation>(Message);
                                                try
                                                {
                                                    switch (CustOp)
                                                    {
                                                        case Enums.CustomerOperation.DEPOSIT:
                                                            Amount = GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            customer.Deposit(BankName, AccountNumber, Amount);
                                                            DisplayOutputLine("Successfully! Deposited your amount");
                                                            break;

                                                        case Enums.CustomerOperation.WITHDRAW:
                                                            Amount = GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            Password = GetString("Account Password : ");
                                                            customer.Withdraw(BankName, AccountNumber, Amount, Password);
                                                            DisplayOutputLine("Successfully! withdrawed your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSFER:
                                                            Amount = GetDecimal($"Enter Amount ({DatabaseService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            string ReceiverBankName = GetLowerCaseWithoutSpaces("Reciever Bank Name : ");
                                                            string ReceiverAccountNumber = GetString("Receiver Account Number : ");
                                                            Enums.TypeOfTransfer TypeOfTransfer = GetEnum<Enums.TypeOfTransfer>("Type of Transfer (IMPS/RTGS) : ");
                                                            Password = GetString("Account Password : ");
                                                            customer.Transfer(BankName, AccountNumber, ReceiverBankName, ReceiverAccountNumber, Amount, Password, TypeOfTransfer);
                                                            DisplayOutputLine("Successfully! Transfered your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSACTIONHISTORY:
                                                            Password = GetString("Account Password : ");
                                                            List<string> History;
                                                            History = customer.TransactionHistory(BankName, AccountNumber, Password);
                                                            DisplayOutput(History);
                                                            break;

                                                        case Enums.CustomerOperation.EXIT:
                                                            CustomerFlag = false;
                                                            break;
                                                        default:
                                                            DisplayOutputLine("Wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    DisplayOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.BANKSTAFF:
                                            Id = GetLowerCase("Clerk ID : ");
                                            Password = GetString("Password : ");
                                            if (!ClerkService.Clerks.ContainsKey(Id) || ClerkService.Clerks[Id].Password != Password)
                                            {
                                                DisplayOutputLine("Invalid ID or Password");
                                                break;
                                            }
                                            ClerkService clerk = ClerkService.Clerks[Id];
                                            string Name, MobileNumber;
                                            Enums.Gender Gender;
                                            bool ClerkFlag = true;
                                            string address;
                                            Enums.ClerkOperation ClerkOp;
                                            while (ClerkFlag)
                                            {
                                                Message = "\n1.Create Account\n2.Update Account\n3.Delete Account\n4.Transaction History\n5.Revert Transaction\n6.Update Charges\n7.Update Currency\n8.Logout\nEnter Operation No : ";
                                                ClerkOp = GetEnum<Enums.ClerkOperation>(Message);
                                                try
                                                {
                                                    switch (ClerkOp)
                                                    {
                                                        case Enums.ClerkOperation.CREATEACCOUNT:
                                                            Name = GetString("Name : ");
                                                            if(Name.Length <= 3)
                                                            {
                                                                DisplayOutputLine("Name is too short!");
                                                                break;
                                                            }
                                                            MobileNumber = GetString("Mobile No : ");
                                                            Gender = GetEnum<Enums.Gender>("Gender (M/F/O) : ");
                                                            address = GetAddress();
                                                            Password = GetString("set Account Password : ");
                                                            AccountNumber = clerk.CreateAccount(Name, MobileNumber, Gender, address, Password);
                                                            DisplayOutputLine($"Account is created, Account Number is : {AccountNumber}");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATEACCOUNT:
                                                            Message = "\n1.Address\n2.Mobile Number\nEnter Operarion No : ";
                                                            Enums.UpdateAccount update = GetEnum<Enums.UpdateAccount>(Message);
                                                            switch (update)
                                                            {
                                                                case Enums.UpdateAccount.ADDRESS:
                                                                    AccountNumber = GetString("Account Number : ");
                                                                    address = GetAddress();
                                                                    clerk.UpdateAddress(AccountNumber, address);
                                                                    DisplayOutputLine("Successfully! updated the address");
                                                                    break;
                                                                case Enums.UpdateAccount.MOBILENUMBER:
                                                                    AccountNumber = GetString("Account Number : ");
                                                                    MobileNumber = GetString("New Mobile Number : ");
                                                                    clerk.UpdateMobileNumber(AccountNumber,MobileNumber);
                                                                    DisplayOutputLine("Successfully! Updated the mobile number");
                                                                    break;
                                                                default:
                                                                    DisplayOutputLine("Wrong Input!");
                                                                    break;
                                                            }
                                                            break;

                                                        case Enums.ClerkOperation.DELETEACCOUNT:
                                                            AccountNumber = GetString("Account Number : ");
                                                            clerk.DeleteAccount(AccountNumber);
                                                            DisplayOutputLine("Successfully! Deleted the account");
                                                            break;

                                                        case Enums.ClerkOperation.TRANSACTIONHISTORY:
                                                            AccountNumber = GetString("Account Number : ");
                                                            List<string> History;
                                                            History = clerk.TransactionHistory(AccountNumber);
                                                            DisplayOutput(History);
                                                            break;

                                                        case Enums.ClerkOperation.REVERTTRANSACTION:
                                                            AccountNumber = GetString("Account Number : ");
                                                            string TransactionId = GetString("Account Number : ");
                                                            clerk.RevertTransaction(AccountNumber, TransactionId);
                                                            DisplayOutputLine("Successfully! Reverted the transaction");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECHARGES:
                                                            decimal ImpsSame, ImpsDiff, RtgsSame,RtgsDiff;
                                                            ImpsSame = GetDecimal("New IMPS Charges for Same Bank : ");
                                                            ImpsDiff = GetDecimal("New IMPS Charges for Diff Bank : ");
                                                            RtgsSame = GetDecimal("New RTGS Charges for Same Bank : ");
                                                            RtgsDiff = GetDecimal("New RTGS Charges for Same Bank : ");
                                                            clerk.UpdateServiceCharges(ImpsSame, RtgsSame, ImpsDiff, RtgsDiff);
                                                            DisplayOutputLine("Successfully! Updates the charges");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECURRENCY:
                                                            AccountNumber = GetString("Account Number : ");
                                                            Enums.CurrencyType currency = GetEnum<Enums.CurrencyType>("New Currency (INR/INUSD) : ");
                                                            clerk.UpdateCurrency(AccountNumber,currency);
                                                            DisplayOutputLine("Successfully! Updated the Currency");
                                                            break;

                                                        case Enums.ClerkOperation.LOGOUT:
                                                            DisplayOutputLine("Logged out!");
                                                            ClerkFlag = false;
                                                            break;

                                                        default:
                                                            DisplayOutputLine("wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    DisplayOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.EXIT:
                                            LoginFlag = false;
                                            break;

                                        default:
                                            DisplayOutputLine ("Wrong input!");
                                            break;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    DisplayOutputLine(exception.Message);
                                }
                            }
                            break;
                        default:
                            DisplayOutputLine("Wrong input!");
                            break;
                    }
                }
                catch(Exception exception)
                {
                    DisplayOutputLine(exception.Message);
                }                
            }
        }
    }
}

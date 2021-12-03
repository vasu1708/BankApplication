using BankApp.Models;
using BankApp.Services;
using System;
using System.Collections.Generic;

namespace BankApp.CLI
{
    class Program
    {
        static void PutOutput(List<string> history)
        {
            PutOutputLine("<---TRANSACTION HISTORY--->");
            if (history.Count == 0)
                PutOutputLine("No Transactions yet!");
            else
            foreach (var Txn in history)
            {
                PutOutputLine($"TransactionId SenderId ReceiverId Type Amount Time Avl.Bal");
                PutOutputLine(Txn);
            }
        }
        static void PutOutput(string message)
        {
            Console.Write(message);
        }
        static void PutOutputLine(string message)
        {
            Console.WriteLine(message);
        }
        static string GetStringInput(string message)
        {
            PutOutput(message);
            return Console.ReadLine();
        }
        static string GetLowerCaseStringInput(string message)
        {
            return GetStringInput(message).ToLower();
        }
        static string GetLowerCaseStringInputWithoutSpaces(string message)
        {
            return GetLowerCaseStringInput(message).Replace(" ", "");
        }
        static decimal GetDecimalInput(string message)
        {
            decimal input;
            while (true)
            {
                try
                {
                    PutOutput(message);
                    input = decimal.Parse(Console.ReadLine());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static int GetIntegerInput(string message)
        {
            int input;
            while (true)
            {
                try
                {
                    PutOutput(message);
                    input = int.Parse(Console.ReadLine());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.Action GetActionInput(string message)
        {
            Enums.Action input;
            while (true)
            {
                try
                {
                    input = (Enums.Action)Enum.Parse(typeof(Enums.Action), GetStringInput(message));
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.Login GetLoginInput(string message)
        {
            Enums.Login input;
            while (true)
            {
                try
                {
                    input = (Enums.Login)Enum.Parse(typeof(Enums.Login), GetStringInput(message));
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.ClerkOperation GetClerkOpInput(string message)
        {
            Enums.ClerkOperation input;
            while (true)
            {
                try
                {
                    input = (Enums.ClerkOperation)Enum.Parse(typeof(Enums.ClerkOperation), GetStringInput(message));
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.CustomerOperation GetCustOpInput(string message)
        {
            Enums.CustomerOperation input;
            while (true)
            {
                try
                {
                    input = (Enums.CustomerOperation)Enum.Parse(typeof(Enums.CustomerOperation), GetStringInput(message));
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.TypeOfTransfer GetTransferTypeInput(string message)
        {
            Enums.TypeOfTransfer input;
            while (true)
            {
                try
                {
                    input = (Enums.TypeOfTransfer)Enum.Parse(typeof(Enums.TypeOfTransfer), GetStringInput(message).ToUpper());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.Gender GetGenderTypeInput(string message)
        {
            Enums.Gender input;
            while (true)
            {
                try
                {
                    input = (Enums.Gender)Enum.Parse(typeof(Enums.Gender), GetStringInput(message).ToUpper());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.CurrencyType GetCurrencyTypeInput(string message)
        {
            Enums.CurrencyType input;
            while (true)
            {
                try
                {
                    input = (Enums.CurrencyType)Enum.Parse(typeof(Enums.CurrencyType), GetStringInput(message).ToUpper());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static Enums.UpdateAccount GetUpdateOpInput(string message)
        {
            Enums.UpdateAccount input;
            while (true)
            {
                try
                {
                    input = (Enums.UpdateAccount)Enum.Parse(typeof(Enums.UpdateAccount), GetStringInput(message).ToUpper());
                    return input;
                }
                catch
                {
                    PutOutputLine("Wrong Input!");
                }
            }
        }
        static string GetAddress()
        {
            
            string ColonyName = GetStringInput("Colony Name : ");
            int StreetNo = GetIntegerInput("Street No : ");
            string HouseNo = GetStringInput("House No : ");
            string DistrictName = GetStringInput("District Name : ");
            string StateName = GetStringInput("State Name : ");
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
                Action = GetActionInput(Message);
                try {
                    switch (Action)
                    {
                        case Enums.Action.NEWBANK:                        
                            BankName = GetLowerCaseStringInputWithoutSpaces("Bank Name To Setup : ");
                            if (BankName.Length<=3)
                            {
                                PutOutputLine("Bank Name is too short!");
                                break;
                            }
                            string ClerkName = GetLowerCaseStringInputWithoutSpaces("Add Clerk Name : ");
                            Password = GetStringInput("Set Password : ");
                            string Id = customer.AddBank(BankName,ClerkName,Password);
                            PutOutputLine($"{BankName} is eshtablished, Remember! clerk Id : {Id}");
                            break;

                        case Enums.Action.LOGIN:                        
                            BankName = GetLowerCaseStringInputWithoutSpaces("Enter Bank Name : ");
                            if (!DatabaseConnectionService.IsBankExist(BankName))
                            {
                                PutOutputLine("Bank doesn't exist!");
                                break;
                            }
                            bool LoginFlag = true;
                            Enums.Login Login;
                            while (LoginFlag)
                            {
                                Message = "As:\n1.AccountHolder\n2.Bankstaff\n3.Exit\nEnter Operation No : ";                                
                                Login = GetLoginInput(Message);
                                try
                                {
                                    switch (Login)
                                    {
                                        case Enums.Login.ACCOUNTHOLDER:
                                            Enums.CustomerOperation CustOp;
                                            decimal Amount;
                                            bool CustomerFlag = true;
                                            AccountNumber = GetStringInput("Account Number : ");
                                            while (CustomerFlag)
                                            {
                                                Message = "\n1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction History\n5.Exit\nEnter Operation No : ";
                                                CustOp = GetCustOpInput(Message);
                                                try
                                                {
                                                    switch (CustOp)
                                                    {
                                                        case Enums.CustomerOperation.DEPOSIT:
                                                            Amount = GetDecimalInput($"Enter Amount ({DatabaseConnectionService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            customer.Deposit(BankName, AccountNumber, Amount);
                                                            PutOutputLine("Successfully! Deposited your amount");
                                                            break;

                                                        case Enums.CustomerOperation.WITHDRAW:
                                                            Amount = GetDecimalInput($"Enter Amount ({DatabaseConnectionService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            Password = GetStringInput("Account Password : ");
                                                            customer.Withdraw(BankName, AccountNumber, Amount, Password);
                                                            PutOutputLine("Successfully! withdrawed your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSFER:
                                                            Amount = GetDecimalInput($"Enter Amount ({DatabaseConnectionService.GetAccountCurrencyType(AccountNumber)}) : ");
                                                            string ReceiverBankName = GetLowerCaseStringInputWithoutSpaces("Reciever Bank Name : ");
                                                            string ReceiverAccountNumber = GetStringInput("Receiver Account Number : ");
                                                            Enums.TypeOfTransfer TypeOfTransfer = GetTransferTypeInput("Type of Transfer (IMPS/RTGS) : ");
                                                            Password = GetStringInput("Account Password : ");
                                                            customer.Transfer(BankName, AccountNumber, ReceiverBankName, ReceiverAccountNumber, Amount, Password, TypeOfTransfer);
                                                            PutOutputLine("Successfully! Transfered your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSACTIONHISTORY:
                                                            Password = GetStringInput("Account Password : ");
                                                            List<string> History;
                                                            History = customer.TransactionHistory(BankName, AccountNumber, Password);
                                                            PutOutput(History);
                                                            break;

                                                        case Enums.CustomerOperation.EXIT:
                                                            CustomerFlag = false;
                                                            break;
                                                        default:
                                                            PutOutputLine("Wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    PutOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.BANKSTAFF:
                                            Id = GetLowerCaseStringInput("Clerk ID : ");
                                            Password = GetStringInput("Password : ");
                                            if (!ClerkService.Clerks.ContainsKey(Id) || ClerkService.Clerks[Id].Password != Password)
                                            {
                                                PutOutputLine("Invalid ID or Password");
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
                                                ClerkOp = GetClerkOpInput(Message);
                                                try
                                                {
                                                    switch (ClerkOp)
                                                    {
                                                        case Enums.ClerkOperation.CREATEACCOUNT:
                                                            Name = GetStringInput("Name : ");
                                                            if(Name.Length <= 3)
                                                            {
                                                                PutOutputLine("Name is too short!");
                                                                break;
                                                            }
                                                            MobileNumber = GetStringInput("Mobile No : ");
                                                            Gender = GetGenderTypeInput("Gender (M/F/O) : ");
                                                            address = GetAddress();
                                                            Password = GetStringInput("set Account Password : ");
                                                            AccountNumber = clerk.CreateAccount(Name, MobileNumber, Gender, address, Password);
                                                            PutOutputLine($"Account is created, Account Number is : {AccountNumber}");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATEACCOUNT:
                                                            Message = "\n1.Address\n2.Mobile Number\nEnter Operarion No : ";
                                                            Enums.UpdateAccount update = GetUpdateOpInput(Message);
                                                            switch (update)
                                                            {
                                                                case Enums.UpdateAccount.ADDRESS:
                                                                    AccountNumber = GetStringInput("Account Number : ");
                                                                    address = GetAddress();
                                                                    clerk.UpdateAddress(AccountNumber, address);
                                                                    PutOutputLine("Successfully! updated the address");
                                                                    break;
                                                                case Enums.UpdateAccount.MOBILENUMBER:
                                                                    AccountNumber = GetStringInput("Account Number : ");
                                                                    MobileNumber = GetStringInput("New Mobile Number : ");
                                                                    clerk.UpdateMobileNumber(AccountNumber,MobileNumber);
                                                                    PutOutputLine("Successfully! Updated the mobile number");
                                                                    break;
                                                                default:
                                                                    PutOutputLine("Wrong Input!");
                                                                    break;
                                                            }
                                                            break;

                                                        case Enums.ClerkOperation.DELETEACCOUNT:
                                                            AccountNumber = GetStringInput("Account Number : ");
                                                            clerk.DeleteAccount(AccountNumber);
                                                            PutOutputLine("Successfully! Deleted the account");
                                                            break;

                                                        case Enums.ClerkOperation.TRANSACTIONHISTORY:
                                                            AccountNumber = GetStringInput("Account Number : ");
                                                            List<string> History;
                                                            History = clerk.TransactionHistory(AccountNumber);
                                                            PutOutput(History);
                                                            break;

                                                        case Enums.ClerkOperation.REVERTTRANSACTION:
                                                            AccountNumber = GetStringInput("Account Number : ");
                                                            string TransactionId = GetStringInput("Account Number : ");
                                                            clerk.RevertTransaction(AccountNumber, TransactionId);
                                                            PutOutputLine("Successfully! Reverted the transaction");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECHARGES:
                                                            decimal ImpsSame, ImpsDiff, RtgsSame,RtgsDiff;
                                                            ImpsSame = GetDecimalInput("New IMPS Charges for Same Bank : ");
                                                            ImpsDiff = GetDecimalInput("New IMPS Charges for Diff Bank : ");
                                                            RtgsSame = GetDecimalInput("New RTGS Charges for Same Bank : ");
                                                            RtgsDiff = GetDecimalInput("New RTGS Charges for Same Bank : ");
                                                            clerk.UpdateServiceCharges(ImpsSame, RtgsSame, ImpsDiff, RtgsDiff);
                                                            PutOutputLine("Successfully! Updates the charges");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECURRENCY:
                                                            AccountNumber = GetStringInput("Account Number : ");
                                                            Enums.CurrencyType currency = GetCurrencyTypeInput("New Currency (INR/INUSD) : ");
                                                            clerk.UpdateCurrency(AccountNumber,currency);
                                                            PutOutputLine("Successfully! Updated the Currency");
                                                            break;

                                                        case Enums.ClerkOperation.LOGOUT:
                                                            PutOutputLine("Logged out!");
                                                            ClerkFlag = false;
                                                            break;

                                                        default:
                                                            PutOutputLine("wrong input!");
                                                            break;
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    PutOutputLine(exception.Message);
                                                }
                                            }
                                            break;

                                        case Enums.Login.EXIT:
                                            LoginFlag = false;
                                            break;

                                        default:
                                            PutOutputLine ("Wrong input!");
                                            break;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    PutOutputLine(exception.Message);
                                }
                            }
                            break;
                        default:
                            PutOutputLine("Wrong input!");
                            break;
                    }
                }
                catch(Exception exception)
                {
                    PutOutputLine(exception.Message);
                }                
            }
        }
    }
}

using BankApp.Models;
using BankApp.Services;
using System;
using System.Collections.Generic;

namespace BankApp.CLI
{
    class Program
    {
        static void Print(string BankName,List<string> History)
        {
            foreach (var Txn in History)
            {
                Console.WriteLine("Id : ", CustomerService.Banks[BankName].Transactions[Txn].TransactionId);
                Console.WriteLine("Sender Id : ", CustomerService.Banks[BankName].Transactions[Txn].SenderAccountId);
                Console.WriteLine("Recciver Id : ", CustomerService.Banks[BankName].Transactions[Txn].ReceiverAccountId);
                Console.WriteLine("Amount : ", CustomerService.Banks[BankName].Transactions[Txn].Amount);
                Console.WriteLine("On : ", CustomerService.Banks[BankName].Transactions[Txn].TimeOfTransaction);
            }
        }
        static string GetInput(string Message)
        {
            Console.Write(Message);
            return Console.ReadLine();
        }
        static void Main(string[] args)
        {
            int Operation,Action,Login;
            decimal Amount;
            string BankName,Message,Id,AccountNumber, Password;
            bool flag;
            CustomerService customer = new CustomerService();
            while (true)
            {
                while (true)
                {
                    Message = "1.Setup New Bank\n2.Login\nEnter Operation : ";
                    try
                    {
                        Action = int.Parse(GetInput(Message));
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input!");
                    }
                }
                switch (Action)
                {
                    case 1:
                        
                        BankName = GetInput("Bank Name To Setup: ").ToUpper();
                        string ClerkName = GetInput("Add Clerk Name : ");                       
                        try
                        {
                            Password = GetInput("set PIN : ");
                            string ClerkId = customer.AddBank(BankName,ClerkName,Password);
                            Console.WriteLine($"Remember Your Id : {ClerkId}");
                        }
                        catch(Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                        
                        break;

                    case 2:                        
                        BankName = GetInput("Enter Bank Name : ").ToUpper();
                        if (!CustomerService.Banks.ContainsKey(BankName))
                        {
                            Console.WriteLine("Bank doesn't exist");
                            break;
                        }
                        flag = true;
                        while (flag)
                        {
                            while (true)
                            {
                                try
                                {
                                    Login = int.Parse(GetInput("As:\n1.Account Holder\n2.Bank staff\n3.Exit\nEnter Operation : "));
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid input!");
                                }
                            }
                            switch (Login)
                            {
                                case (int)Enums.Login.AccountHolder:
                                    flag = true;

                                    while (flag)
                                    {
                                        Message = "1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction History\n5.Exit\nEnter Operation : ";
                                        while (true)
                                        {
                                            try
                                            {
                                                Operation = int.Parse(GetInput(Message));
                                                break;
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Wrong Input!");
                                            }
                                        }
                                        AccountNumber = GetInput("Account Number : ");
                                        switch (Operation)
                                        {

                                            case (int)Enums.CustomerOperation.Deposit:
                                                Amount = decimal.Parse(GetInput("Enter Amount : "));
                                                try
                                                {
                                                    customer.Deposit(BankName, AccountNumber, Amount);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;

                                            case (int)Enums.CustomerOperation.Withdraw:
                                                Amount = decimal.Parse(GetInput("Enter Amount : "));
                                                Password = GetInput("Account PIN : ");
                                                try
                                                {
                                                    customer.Withdraw(BankName, AccountNumber, Amount, Password);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;

                                            case (int)Enums.CustomerOperation.Transfer:
                                                Amount = decimal.Parse(GetInput("Enter Amount : "));
                                                string SenderAccountNumber = GetInput("Sender Account Number : ");
                                                string ReceiverBankName = GetInput("Reciever Bank Name : ").ToUpper();
                                                string ReceiverAccountNumber = GetInput("Receiver Account Number : ");
                                                string TypeOfTransfer = GetInput("Type of Transfer (IMPS/RTGS) : ").ToUpper();
                                                if (TypeOfTransfer != "RTGS" && TypeOfTransfer != "IMPS")
                                                    Console.WriteLine("wrong Input!");
                                                Password = GetInput("Account Password: ");
                                                try
                                                {
                                                    customer.Transfer(BankName, SenderAccountNumber, ReceiverBankName,ReceiverAccountNumber, Amount, Password,TypeOfTransfer);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;

                                            case (int)Enums.CustomerOperation.TransactionHistory:
                                                Password = GetInput("Account Password : ");
                                                List<string> History;
                                                try
                                                {
                                                    History = customer.TransactionHistory(BankName, AccountNumber, Password);
                                                    Print(BankName,History);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }

                                                break;

                                            case (int)Enums.CustomerOperation.Exit:
                                                flag = false;
                                                break;
                                            default:
                                                Console.WriteLine("Wrong input!");
                                                break;
                                        }
                                    }
                                    break;
                                case (int)Enums.Login.Bankstaff:
                                    Id = GetInput("ID : ").ToUpper();
                                    Password = GetInput("Password : ");
                                    if (!ClerkService.Clerks.ContainsKey(Id) || ClerkService.Clerks[Id].Password != Password)
                                    {
                                        Console.WriteLine("Invalid ID or PIN");
                                        break;
                                    }
                                    ClerkService clerk = ClerkService.Clerks[Id];
                                    string Name, MobileNumber, Gender;
                                    flag = true;
                                    while (flag)
                                    {
                                        Message = "1.Create Account\n2.Delete Account\n3.TransactionHistory\n4.Logout\nEnter Operation : ";
                                        while (true)
                                        {
                                            try
                                            {
                                                Operation = int.Parse(GetInput(Message));
                                                break;
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Invalid Input!");
                                            }
                                        }
                                        switch (Operation)
                                        {
                                            case (int)Enums.ClerkOperation.CreateAccount:
                                                Name = GetInput("Name : ");
                                                MobileNumber = GetInput("Mobile No : ");
                                                Gender = GetInput("Gender : ");
                                                Address address = new Address();
                                                address.ColonyName = GetInput("Colony Name : ");
                                                address.StreetNo = int.Parse(GetInput("Street No : "));
                                                address.HouseNo = GetInput("House No : ");
                                                address.DistrictName = GetInput("District Name ");
                                                address.StateName = GetInput("State Name : ");
                                                Password = GetInput("set Account Password : ");
                                                try
                                                {
                                                   AccountNumber =  clerk.CreateAccount(Name, MobileNumber, Gender, address, Password);
                                                    Console.WriteLine($"Customer! Your Account Number is : {AccountNumber}");
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;

                                            case (int)Enums.ClerkOperation.DeleteAccount:
                                                AccountNumber = GetInput("Account Number : ");
                                                try
                                                {
                                                    clerk.DeleteAccount(AccountNumber);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;

                                            case (int)Enums.ClerkOperation.TransactionHistory:
                                                AccountNumber = GetInput("Account Number : ");
                                                List<string> History;
                                                try
                                                {
                                                    History = clerk.TransactionHistory(AccountNumber);
                                                    Print(BankName ,History);
                                                }
                                                catch (Exception exception)
                                                {
                                                    Console.WriteLine(exception.Message);
                                                }
                                                break;
                                            case (int)Enums.ClerkOperation.Logout:
                                                flag = false;
                                                break;
                                            default:
                                                Console.WriteLine("wrong input!");

                                                break;

                                        }
                                    }
                                    break;
                                case (int)Enums.Login.Exit:
                                    flag = false;
                                    break;
                                default:
                                    Console.WriteLine("Wrong input!");
                                    break;
                            }
                            
                        }
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }
        }
    }
}

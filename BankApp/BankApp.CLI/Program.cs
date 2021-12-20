using BankApp.Models;
using BankApp.Services;

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
                            bankName = IOMethods.GetNameWithoutSpaces("Bank name To Setup : ");
                            string ClerkName = IOMethods.GetName("Add Clerk name : ");
                            password = IOMethods.GetString("Set password : ");
                            string dob = IOMethods.GetDOB("Enter date Of Birth(DD-MM-YYYY) : ");
                            string address = IOMethods.GetAddress();
                            string mobileNumber = IOMethods.GetMobileNumber("Enter Mobile No : ");
                            string id = new CustomerService().AddBank(bankName,ClerkName,dob,address,password,mobileNumber);
                            IOMethods.DisplayOutputLine($"{bankName} is eshtablished, Remember! clerk Id : {id}");
                            break;

                        case Enums.Action.LOGIN:                        
                            bankName = IOMethods.GetNameWithoutSpaces("Enter Bank name : ");
                            CustomerService customer = new CustomerService();
                            customer.IsBankExist(bankName);
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
                                            new ClerkService().IsAccountExist(accountNumber);
                                            while (customerFlag)
                                            {
                                                message = "\n1.Deposit\n2.Withdraw\n3.Transfer\n4.Transaction history\n5.Exit\nEnter Operation No : ";
                                                CustOp = IOMethods.GetEnum<Enums.CustomerOperation>(message);
                                                try
                                                {
                                                    switch (CustOp)
                                                    {
                                                        case Enums.CustomerOperation.DEPOSIT:
                                                            amount = IOMethods.GetDecimal($"Enter amount : ");
                                                            customer.Deposit(accountNumber, amount);
                                                            IOMethods.DisplayOutputLine("Successfully! Deposited your amount");
                                                            break;

                                                        case Enums.CustomerOperation.WITHDRAW:
                                                            amount = IOMethods.GetDecimal($"Enter amount : ");
                                                            password = IOMethods.GetString("Account password : ");
                                                            customer.Withdraw(accountNumber, password,amount);
                                                            IOMethods.DisplayOutputLine("Successfully! withdrawed your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSFER:
                                                            amount = IOMethods.GetDecimal($"Enter amount : ");
                                                            string receiverBankName = IOMethods.GetName("Reciever Bank name : ");
                                                            string receiverAccountNumber = IOMethods.GetString("Receiver Account Number : ");
                                                            Enums.TypeOfTransfer transferType = IOMethods.GetEnum<Enums.TypeOfTransfer>("Type of Transfer (IMPS/RTGS) : ");
                                                            password = IOMethods.GetString("Account password : ");
                                                            customer.Transfer(bankName,accountNumber,receiverBankName, receiverAccountNumber, password,transferType, amount );
                                                            IOMethods.DisplayOutputLine("Successfully! Transfered your amount");
                                                            break;

                                                        case Enums.CustomerOperation.TRANSACTIONHISTORY:
                                                            password = IOMethods.GetString("Account password : ");
                                                            List<string> history;
                                                            history = customer.TransactionHistory(accountNumber, password);
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
                                            id = IOMethods.GetLowerCase("Clerk ID : ");
                                            password = IOMethods.GetString("password : ");
                                            ClerkService clerk = new ClerkService();
                                            if (!clerk.VerifyPassword(id,password))
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
                                                            dob = IOMethods.GetDOB("Enter date of Birth(DD/MM/YYYY) : ");
                                                            accountNumber = clerk.CreateAccount(bankName,name, password,address, gender, dob, mobileNumber);
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
                                                                    clerk.UpdateAddress(accountNumber, address);
                                                                    IOMethods.DisplayOutputLine("Successfully! updated the address");
                                                                    break;
                                                                case Enums.UpdateAccount.MOBILENUMBER:
                                                                    accountNumber = IOMethods.GetString("Account Number : ");
                                                                    mobileNumber = IOMethods.GetMobileNumber("New Mobile Number : ");
                                                                    clerk.UpdateMobileNumber(accountNumber,mobileNumber);
                                                                    IOMethods.DisplayOutputLine("Successfully! Updated the mobile number");
                                                                    break;
                                                                default:
                                                                    IOMethods.DisplayOutputLine("Wrong Input!");
                                                                    break;
                                                            }
                                                            break;

                                                        case Enums.ClerkOperation.DELETEACCOUNT:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            clerk.DeleteAccount(accountNumber);
                                                            IOMethods.DisplayOutputLine("Successfully! Deleted the account");
                                                            break;

                                                        case Enums.ClerkOperation.TRANSACTIONHISTORY:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            List<string> history;
                                                            history = clerk.TransactionHistory(accountNumber);
                                                            IOMethods.DisplayOutput(history);
                                                            break;

                                                        case Enums.ClerkOperation.REVERTTRANSACTION:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            string transactionId = IOMethods.GetString("Account Number : ");
                                                            clerk.RevertTransaction(accountNumber, transactionId);
                                                            IOMethods.DisplayOutputLine("Successfully! Reverted the transaction");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECHARGES:
                                                            Enums.ChargeType type = IOMethods.GetEnum<Enums.ChargeType>("Enter Type of charge(SameBankIMPS/OtherBankIMPS/SameBankRTGS/OtherBankRTGS) ");
                                                            decimal charge = IOMethods.GetDecimal("Enter charge for that type : ");
                                                            clerk.UpdateCharges(bankName,type,charge);
                                                            IOMethods.DisplayOutputLine("Successfully! Updates the charges");
                                                            break;

                                                        case Enums.ClerkOperation.UPDATECURRENCY:
                                                            accountNumber = IOMethods.GetString("Account Number : ");
                                                            Enums.CurrencyType currency = IOMethods.GetEnum<Enums.CurrencyType>("New Currency (INR/INUSD) : ");
                                                            clerk.UpdateCurrency(accountNumber,currency);
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

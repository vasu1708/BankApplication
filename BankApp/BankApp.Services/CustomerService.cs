using BankApp.Models;

namespace BankApp.Services
{
    public class CustomerService
    {
       public void Deposit(string accountNo,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            Account account = clerk.IsAccountExist(accountNo);
            clerk.PerformTransaction(accountNo,"-","-",Enums.TransactionType.CREDIT,amount);
            account.AccountBalance += amount;
            new DbContextService().SaveChanges();
        } 
        public string AddBank(string bankName,string ClerkName,string dob,string address,string password,string mobileNumber,decimal salary)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            Bank bank = new Bank()
            {
                BankId = $"{bankName.Substring(0, 3)}{date}",
                BankName = bankName,
                BankBalance = 0,
                SameBankIMPS = 3,
                SameBankRTGS = 4,
                OtherBankIMPS = 5,
                OtherBankRTGS = 6,
                EstablishedDate = DateTime.Now.Date,
                Clerks = new List<Clerk>(),
                Accounts = new List<Account>()
            };
            return AddClerk(bank, ClerkName, password, dob, address, mobileNumber);
        }
       public string AddClerk(Bank bank,string name,string password,string dob,string address,string mobileNumebr)
        {
            string clerkId = $"{name}@{bank.BankName}";
            Clerk clerk = new Clerk()
            {
                ClerkId = clerkId,
                ClerkName = name,
                Password = password,
                DateOfBirth = dob,
                DateOfJoin = DateTime.Now.Date,
                Address = address,
                MobileNumber = mobileNumebr,
                Bank = bank
            };
            return clerkId;
        }
       public void  Withdraw(string accountNo,string password,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            Account account = clerk.IsAccountExist(accountNo);
            VerifyPassword(accountNo, password);
            if (amount < account.AccountBalance)
                throw new Exception("Insufficient Balance!");
            clerk.PerformTransaction(accountNo, "-", "-", Enums.TransactionType.DEBIT, amount);
            account.AccountBalance -= amount;
            new DbContextService().SaveChanges();
        }
       public void Transfer(string senderAccountNo, string receiverAccountNo, string passsword, decimal amount)
        {
            ClerkService clerk = new ClerkService();
            Account senderAccnt = clerk.IsAccountExist(senderAccountNo);
            Account receiverAccnt = clerk.IsAccountExist (receiverAccountNo);
            VerifyPassword(senderAccnt.AccountNumber, passsword);
            if (amount < senderAccnt.AccountBalance)
                throw new Exception("Insufficient Balance!");
            clerk.PerformTransaction(senderAccountNo, senderAccnt.AccountId, receiverAccnt.AccountId, Enums.TransactionType.DEBIT,amount);
            clerk.PerformTransaction(receiverAccnt.AccountNumber, senderAccnt.AccountId, receiverAccnt.AccountId, Enums.TransactionType.CREDIT, amount);
            senderAccnt.AccountBalance -= amount;
            receiverAccnt.AccountBalance += amount;
            new DbContextService().SaveChanges();
        }
       public List<string> TransactionHistory(string accountNumber,string password)
        {
            VerifyPassword(accountNumber,password);
           return new ClerkService().TransactionHistory(accountNumber);
        }
       public bool VerifyPassword(string accountNumber,string password)
        {
            Account account = new ClerkService().IsAccountExist(accountNumber);
            if (account.AccountPassword != password)
                throw new Exception("Incorrect Password!");
            return true;
        }
    }
}
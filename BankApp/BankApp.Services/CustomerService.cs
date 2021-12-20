using BankApp.Models;

namespace BankApp.Services
{
    public class CustomerService
    {
       public void Deposit(string accountNo,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            string accountId = clerk.IsAccountExist(accountNo);
            clerk.PerformTransaction(accountId,"-","-",Enums.TransactionType.CREDIT,amount);
        } 
        public string AddBank(string bankName,string clerkName,string dob,string address,string password,string mobileNumber)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            DbContextService context = new DbContextService();
            string bankId = $"{bankName.Substring(0, 3)}{date}";
            Bank bank = new Bank()
            {
                BankId = bankId,
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
            context.Banks.Add(bank);
            context.SaveChanges();
            return AddClerk(bankId, clerkName, password, dob, address, mobileNumber);
        }
       public string AddClerk(string bankId,string name,string password,string dob,string address,string mobileNumber)
        {
            
            DbContextService context = new DbContextService();
            Bank bank = context.Banks.Single(b => b.BankId == bankId);
            string clerkId = $"{name}@{bank.BankName}";
            Clerk clerk = new Clerk()
            {
                ClerkId = clerkId,
                ClerkName = name,
                Password = password,
                DateOfBirth = dob,
                DateOfJoin = DateTime.Now.Date,
                Address = address,
                MobileNumber = mobileNumber,
                Bank = bank
            };
            context.Clerks.Add(clerk);
            context.SaveChanges();
            return clerkId;
        }
       public void  Withdraw(string accountNo,string password,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            string accountId = clerk.IsAccountExist(accountNo);
            VerifyPassword(accountNo, password);
            clerk.PerformTransaction(accountId, "-", "-", Enums.TransactionType.DEBIT, amount);
        }
       public void Transfer(string senderBankName,string senderAccountNo, string receiverBankName,string receiverAccountNo, string passsword, Enums.TypeOfTransfer type ,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            string senderAccntId = clerk.IsAccountExist(senderAccountNo);
            string receiverAccntId = clerk.IsAccountExist (receiverAccountNo);
            VerifyPassword(senderAccountNo, passsword);
            DbContextService context = new DbContextService();
            decimal rate;
            clerk.PerformTransaction(senderAccntId, senderAccntId, receiverAccntId, Enums.TransactionType.DEBIT, amount);
            string bankId = GetBankId(senderBankName);
            switch (type)
            {
                case Enums.TypeOfTransfer.IMPS:
                    if (senderBankName == receiverBankName)
                        rate = context.Banks.Single(b => b.BankId == bankId).SameBankIMPS;
                    else
                        rate = context.Banks.Single(b => b.BankId == bankId).OtherBankIMPS;
                    break;
                default:
                    if (senderBankName == receiverBankName)
                        rate = context.Banks.Single(b => b.BankId == bankId).SameBankRTGS;
                    else
                        rate = context.Banks.Single(b => b.BankId == bankId).OtherBankRTGS;
                    break;
            }
            decimal charges = amount * rate/100; 
            clerk.PerformTransaction(receiverAccntId, senderAccntId, receiverAccntId, Enums.TransactionType.CREDIT, amount-charges);
            UpdateBankBalance(senderAccntId, charges);
        }
        public void UpdateBankBalance(string bankId,decimal charges)
        {
            DbContextService context = new DbContextService();
            context.Banks.Single(b => b.BankId == bankId).BankBalance += charges;
            context.SaveChanges();
        }
       public List<string> TransactionHistory(string accountNumber,string password)
        {
            VerifyPassword(accountNumber,password);
           return new ClerkService().TransactionHistory(accountNumber);
        }
       public bool VerifyPassword(string accountNumber,string password)
        {
            string accountId = new ClerkService().IsAccountExist(accountNumber);
            if (new DbContextService().Accounts.Single(a => a.AccountId == accountId).AccountPassword != password)
                throw new Exception("Incorrect Password!");
            return true;
        }
       public string GetBankId(string bankName)
        {
            return new DbContextService().Banks.Single(b => b.BankName == bankName).BankId;
        }
        public bool IsBankExist(string bankName)
        {
            string bankId = GetBankId(bankName);
            if (bankId == null)
                throw new Exception("Bank does not exist!");
            return true;
        }
    }
}
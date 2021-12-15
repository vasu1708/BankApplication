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
        }  
       public void  Withdraw(string accountNo,string password,decimal amount)
        {
            ClerkService clerk = new ClerkService();
            Account account = clerk.IsAccountExist(accountNo);
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
            if (amount < senderAccnt.AccountBalance)
                throw new Exception("Insufficient Balance!");
            clerk.PerformTransaction(senderAccountNo, senderAccnt.AccountId, receiverAccnt.AccountId, Enums.TransactionType.DEBIT,amount);
            clerk.PerformTransaction(receiverAccnt.AccountNumber, senderAccnt.AccountId, receiverAccnt.AccountId, Enums.TransactionType.CREDIT, amount);
            senderAccnt.AccountBalance -= amount;
            receiverAccnt.AccountBalance += amount;
            new DbContextService().SaveChanges();
        }
       public void TransactionHistory(string accountNumber,string password)
        {

        }
       public bool VerifyPassword(string string passwor)
        {

        }
    }
}
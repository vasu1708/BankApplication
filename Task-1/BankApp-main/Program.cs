using System;
using System.Collections.Generic;
using System.IO;
namespace BankApp
{
    
    public class Account
    {
        private string name;
        private string phoneNo;
        private string accountNo;
        private string accountPIN;
        private float balance;
        private string fileName;
        public Account(string nm,string mobNo,string accPin,string accNo,string fn){
            name = nm;
            phoneNo =mobNo;
            accountNo = accNo;
            accountPIN = accPin;
            balance = 0.0F;
            fileName = fn;
        }
        public void Deposit(float amount){
            StreamWriter sw = File.AppendText(this.fileName);
            this.balance +=amount;
            DateTime time = DateTime.Now;
            Console.WriteLine("Amount deposited Successfully!");
            string stmt = amount.ToString()+"Rs. is credited on "+time+"\nAvl Bal: Rs."+this.balance.ToString();
            sw.WriteLine(stmt);
            sw.Close();
        }
        public void WithDraw(float amount){
            if(amount>this.balance){
                Console.WriteLine("No sufficient Amount");
            }
            else{
                StreamWriter sw = File.AppendText(this.fileName);
                this.balance-=amount;
                DateTime time = DateTime.Now;
                Console.WriteLine("Amount withdraw Successfully!");
                string stmt = amount.ToString()+"Rs. is debited on "+time+"\nAvl Bal: "+this.balance.ToString();
                sw.WriteLine(stmt);
                sw.Close();
            }
        }
        public string GetPin(){
            return this.accountPIN;
        }
        public void Display(){
            StreamReader sr = File.OpenText(this.fileName);
            string stmt;
            Console.WriteLine("<--Transaction History-->");
            while((stmt=sr.ReadLine())!=null){
                Console.WriteLine(stmt);
            }
            sr.Close();
        }
        public void BalanceEnquiry(){
            Console.WriteLine("Avl Bal: Rs."+this.balance.ToString());
        }
        public void TransferAmount(Account accRef,float amount){
            if(this.balance<amount){
                Console.WriteLine("Not Sufficient balance");
            }
            else{

                if(amount>this.balance){
                Console.WriteLine("No sufficient Amount");
                }
                else{
                    StreamWriter sf = File.AppendText(this.fileName);
                    StreamWriter df = File.AppendText(accRef.fileName);
                    this.balance-=amount;
                    accRef.balance +=amount;
                    DateTime time = DateTime.Now;
                    int dl = accRef.accountNo.Length;
                    int sl = this.accountNo.Length;
                    string stmt = amount.ToString()+"Rs. is debited to A/c no."+accRef.accountNo+" on "+time+"\nAvl Bal: "+this.balance.ToString();
                    sf.WriteLine(stmt);
                    sf.Close();
                    Console.WriteLine("Amount Transferred Successfully!");
                    stmt = amount.ToString()+"Rs. is credited from A/c no."+this.accountNo+" on "+time+"\nAvl Bal: Rs."+accRef.balance.ToString();
                    df.WriteLine(stmt);
                    df.Close();
                }

            }
        }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            Dictionary<string,Account> dic = new Dictionary<string,Account>();
            string accNo,name,accPin,mobNo;
            int op;
            float amount;
            Account user;
            Console.WriteLine("Bank Application");
            while(true){
                Console.WriteLine("-----------------------------------------------------------------------------------------------");
                Console.WriteLine("1.Create Account\n2.Deposit\n3.Withdraw\n4.Transaction history\n5.Transfer\nEnter operation no:");
                op = Int16.Parse(Console.ReadLine());
                Console.WriteLine("-----------------------------------------------------------------------------------------------");
                switch (op)
                {
                    case 1:
                        Console.WriteLine("Enter Account No:");
                        accNo = Console.ReadLine();
                        if(dic.ContainsKey(accNo))
                        {
                            Console.WriteLine("sorry existed account");
                            break;
                        }
                        Console.WriteLine("Enter Name:");
                        name = Console.ReadLine();                       
                        Console.WriteLine("Enter mobile No:");
                        mobNo = Console.ReadLine();
                        Console.WriteLine("Set Account PIN:");
                        accPin = Console.ReadLine();
                        string fn = @"c:\users\srinivasa reddy\Desktop\THDB\"+name+"@TH.txt";
                        var myFile = File.Create(fn);
                        myFile.Close();
                        user =new Account(name,mobNo,accPin,accNo,fn);
                        dic[accNo] = user;
                        break;

                    case 2:
                    Console.WriteLine("Enter account no:");
                    accNo = Console.ReadLine();
                    if(!dic.ContainsKey(accNo)){
                        Console.WriteLine("sorry not existed account");
                    }
                    else
                    {
                        Console.WriteLine("Enter Amount:");
                        float.TryParse(Console.ReadLine(),out amount);
                        dic[accNo].Deposit(amount);
                    }
                    break;

                    case 3:
                    Console.WriteLine("Enter account no:");
                    accNo = Console.ReadLine();
                    if(!dic.ContainsKey(accNo)){
                        Console.WriteLine("sorry not existed account");
                    }
                    else
                    {
                        Console.WriteLine("Enter PIN:");
                        accPin = Console.ReadLine();
                        if(accPin!=dic[accNo].GetPin()){
                            Console.WriteLine("Incorrect PIN");
                        }
                        else{
                        Console.WriteLine("Enter Amount:");
                        float.TryParse(Console.ReadLine(),out amount);
                        dic[accNo].WithDraw(amount);
                        }
                    }
                    break;
                    
                    case 4:
                    Console.WriteLine("Enter account no:");
                    accNo = Console.ReadLine();
                    if(!dic.ContainsKey(accNo))
                    {
                        Console.WriteLine("sorry not existed account");
                    }
                    else
                    {
                        Console.WriteLine("Enter PIN:");
                        accPin = Console.ReadLine();
                        if(accPin!=dic[accNo].GetPin()){
                            Console.WriteLine("Incorrect PIN");
                        }
                        else{
                        dic[accNo].Display();
                        }
                    }
                    break;

                    case 5:
                    Console.WriteLine("Enter your account no:");
                    accNo = Console.ReadLine();
                    if(!dic.ContainsKey(accNo))
                    {
                        Console.WriteLine("sorry not existed account");
                    }
                    else
                    {
                        Console.WriteLine("Enter PIN:");
                        accPin = Console.ReadLine();
                        if(accPin!=dic[accNo].GetPin()){
                            Console.WriteLine("Incorrect PIN");
                        }
                        else{
                            Console.WriteLine("Enter the recipient account no:");
                            string accNo1 = Console.ReadLine();
                            if(!dic.ContainsKey(accNo1))
                            {
                            Console.WriteLine("sorry not existed account");
                            }
                            else
                            {
                            Console.WriteLine("Enter Amount:");
                            float.TryParse(Console.ReadLine(),out amount);
                            dic[accNo].TransferAmount(dic[accNo1],amount);
                            }
                        }
                    }
                    break;

                    default:
                    return ;
                }
            }
        }
    }
}
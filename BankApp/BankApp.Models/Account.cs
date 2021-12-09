namespace BankApp.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public string  AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string Gender { get; set; }
        public string AcceptedCurrency { get; set; }
        public decimal Balance { get; set; }
        public Address Address { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
    }
}

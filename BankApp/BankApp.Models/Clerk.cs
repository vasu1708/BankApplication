namespace BankApp.Models
{
    public class Clerk
    {
        public string ClerkId { get; set; }
        public string ClerkName { get; set; }
        public string Password { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly DateOfJoin { get; set; }
        public string Address { get; set; }
        public Bank Bank { get; set; }
        public string MobileNumber { get; set; }
    }
}

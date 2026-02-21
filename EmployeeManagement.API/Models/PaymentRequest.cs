namespace EmployeeManagement.API.Models
{
    public class PaymentRequest
    {
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string UpiId { get; set; }
        public string PaymentMethod { get; set; } // Optional: Card, NetBanking, Wallet
    }
}

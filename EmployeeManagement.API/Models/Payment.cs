namespace EmployeeManagement.API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed, Refunded
        public string PaymentMethod { get; set; } // Card, NetBanking, Wallet, etc.
        public string Currency { get; set; } = "INR";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
        public int? EmployeeId_Salary { get; set; }
    }
}
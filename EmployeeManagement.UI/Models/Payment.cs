namespace EmployeeManagement.UI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
    }
}
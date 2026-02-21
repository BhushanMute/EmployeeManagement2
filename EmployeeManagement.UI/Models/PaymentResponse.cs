namespace EmployeeManagement.UI.Models
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentUrl { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
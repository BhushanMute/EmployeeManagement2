namespace EmployeeManagement.API.Models
{
    public class PaymentCallbackRequest
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; } // success, failed, pending
        public string Signature { get; set; }
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
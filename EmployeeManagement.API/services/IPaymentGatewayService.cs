namespace EmployeeManagement.API.Services
{
    using EmployeeManagement.API.Models;

    public interface IPaymentGatewayService
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request);
        Task<bool> VerifyPaymentSignatureAsync(PaymentCallbackRequest callback);
        Task<PaymentResponse> GetPaymentStatusAsync(string orderId);
        Task<PaymentResponse> ProcessRefundAsync(string transactionId, decimal amount);
    }
} 
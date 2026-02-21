namespace EmployeeManagement.API.Services
{
    using EmployeeManagement.API.Models;

    public interface IDummyUpiPaymentService
    {
        Task<PaymentResponse> InitiateUpiPaymentAsync(PaymentRequest request);
        Task<PaymentResponse> VerifyUpiPaymentAsync(string orderId, string upiTransactionId);
        Task<PaymentResponse> SimulateUpiPaymentAsync(string orderId, bool success = true);
    }
}
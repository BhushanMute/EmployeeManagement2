namespace EmployeeManagement.UI.Services
{
    using EmployeeManagement.UI.Models;

    public interface IPaymentService
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request);
        Task<PaymentResponse> GetPaymentStatusAsync(string orderId);
        Task<List<Payment>> GetEmployeePaymentsAsync(int employeeId);
        Task<decimal> GetTotalPaymentsAsync(int employeeId);
    }
}
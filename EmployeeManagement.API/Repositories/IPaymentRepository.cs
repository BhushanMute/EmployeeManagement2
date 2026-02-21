namespace EmployeeManagement.API.Repositories
{
    using EmployeeManagement.API.Models;


    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(PaymentRequest request, string orderId, string transactionId);
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<Payment> GetPaymentByOrderIdAsync(string orderId);
        Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);
        Task<List<Payment>> GetPaymentsByEmployeeAsync(int employeeId);
        Task UpdatePaymentStatusAsync(int id, string status, string transactionId);
        Task<List<Payment>> GetPendingPaymentsAsync();
        Task<decimal> GetTotalPaymentsByEmployeeAsync(int employeeId);
    }
}
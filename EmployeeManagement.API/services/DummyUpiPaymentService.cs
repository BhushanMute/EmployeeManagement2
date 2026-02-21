using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.API.Services
{
    public class DummyUpiPaymentService : IDummyUpiPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<DummyUpiPaymentService> _logger;
        private readonly IConfiguration _configuration;

        public DummyUpiPaymentService(
            IPaymentRepository paymentRepository, 
            ILogger<DummyUpiPaymentService> logger,
            IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<PaymentResponse> InitiateUpiPaymentAsync(PaymentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.UpiId))
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "UPI ID is required"
                    };
                }

                // Generate unique OrderId and TransactionId
                string orderId = GenerateOrderId();
                string transactionId = GenerateTransactionId();
                string upiTransactionId = GenerateUpiTransactionId();

                _logger.LogInformation($"Initiating UPI Payment - OrderId: {orderId}, UPI: {MaskUpiId(request.UpiId)}");

                // Create payment record in database
                var payment = await _paymentRepository.CreatePaymentAsync(request, orderId, transactionId);

                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Failed to create payment record"
                    };
                }

                // Generate dummy UPI payment URL
                string paymentUrl = GenerateDummyUpiPaymentUrl(orderId, request.Amount, request.UpiId, upiTransactionId);

                _logger.LogInformation($"UPI Payment initiated successfully - OrderId: {orderId}");

                return new PaymentResponse
                {
                    Success = true,
                    Message = "UPI Payment initiated successfully. Redirecting to payment gateway...",
                    OrderId = orderId,
                    TransactionId = transactionId,
                    Amount = request.Amount,
                    PaymentStatus = "Pending",
                    PaymentUrl = paymentUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initiating UPI payment: {ex.Message}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error initiating UPI payment: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> VerifyUpiPaymentAsync(string orderId, string upiTransactionId)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for OrderId: {orderId}");
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment record not found"
                    };
                }

                _logger.LogInformation($"Verifying UPI Payment - OrderId: {orderId}, UpiTxnId: {upiTransactionId}");

                // Update payment status to Completed
                await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, "Completed", upiTransactionId);

                return new PaymentResponse
                {
                    Success = true,
                    Message = "UPI Payment verified successfully",
                    OrderId = orderId,
                    TransactionId = upiTransactionId,
                    Amount = payment.Amount,
                    PaymentStatus = "Completed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying UPI payment: {ex.Message}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error verifying payment: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> SimulateUpiPaymentAsync(string orderId, bool success = true)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);

                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment not found"
                    };
                }

                if (success)
                {
                    string simulatedTxnId = GenerateUpiTransactionId();
                    await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, "Completed", simulatedTxnId);

                    _logger.LogInformation($"Payment simulation successful - OrderId: {orderId}");

                    return new PaymentResponse
                    {
                        Success = true,
                        Message = "Payment simulation completed successfully",
                        OrderId = orderId,
                        TransactionId = simulatedTxnId,
                        Amount = payment.Amount,
                        PaymentStatus = "Completed"
                    };
                }
                else
                {
                    await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, "Failed", "FAILED_" + GenerateTransactionId());

                    _logger.LogInformation($"Payment simulation failed - OrderId: {orderId}");

                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment simulation failed",
                        OrderId = orderId,
                        PaymentStatus = "Failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error simulating UPI payment: {ex.Message}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error simulating payment: {ex.Message}"
                };
            }
        }

        private string GenerateDummyUpiPaymentUrl(string orderId, decimal amount, string upiId, string upiTransactionId)
        {
            // Generate a dummy payment URL that redirects to a test page
            string baseUrl = _configuration["PaymentGateway:DummyUPI:BaseUrl"] 
                ?? "http://localhost:5089";
            
            string encodedUpiId = Uri.EscapeDataString(upiId);
            
            return $"{baseUrl}/api/payment/dummy-upi-payment?" +
                   $"orderId={orderId}&" +
                   $"amount={amount}&" +
                   $"upiId={encodedUpiId}&" +
                   $"txnId={upiTransactionId}&" +
                   $"timestamp={DateTime.UtcNow:O}";
        }

        private string GenerateOrderId()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{GenerateRandomString(6).ToUpper()}";
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{GenerateRandomString(8).ToUpper()}";
        }

        private string GenerateUpiTransactionId()
        {
            return $"{DateTime.UtcNow:yyMMdd}{GenerateRandomString(10).ToUpper()}";
        }

        private string GenerateRandomString(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];
                rng.GetBytes(data);

                var result = new StringBuilder(length);
                foreach (byte b in data)
                {
                    result.Append(chars[b % chars.Length]);
                }
                return result.ToString();
            }
        }

        private string MaskUpiId(string upiId)
        {
            if (string.IsNullOrEmpty(upiId) || upiId.Length < 5)
                return "****";

            var parts = upiId.Split('@');
            if (parts.Length != 2)
                return "****@****";

            string username = parts[0];
            string provider = parts[1];

            string maskedUsername = username.Length > 2 
                ? $"{username[0]}****{username[^1]}" 
                : "****";

            return $"{maskedUsername}@{provider}";
        }
    }
}
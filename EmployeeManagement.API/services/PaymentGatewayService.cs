using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EmployeeManagement.API.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IDummyUpiPaymentService _dummyUpiService;
        
        private readonly string _ptmApiKey;
        private readonly string _ptmMerchantId;
        private readonly string _ptmBaseUrl;
        private readonly bool _useDummyPayment;
         

        public PaymentGatewayService(
            IConfiguration configuration, 
            HttpClient httpClient, 
            IPaymentRepository paymentRepository,
            IDummyUpiPaymentService dummyUpiService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _paymentRepository = paymentRepository;
            _dummyUpiService = dummyUpiService;
    
            _ptmApiKey = configuration["PaymentGateway:PTM:ApiKey"]
                ?? throw new ArgumentNullException("PaymentGateway:PTM:ApiKey not configured");
            _ptmMerchantId = configuration["PaymentGateway:PTM:MerchantId"]
                ?? throw new ArgumentNullException("PaymentGateway:PTM:MerchantId not configured");
            _ptmBaseUrl = configuration["PaymentGateway:PTM:BaseUrl"]
                ?? throw new ArgumentNullException("PaymentGateway:PTM:BaseUrl not configured");
        }

        public async Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request)
        {
            try
            {
                // Generate unique OrderId and TransactionId
                string orderId = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
                string transactionId = Guid.NewGuid().ToString();

                // Create payment record in database
                var payment = await _paymentRepository.CreatePaymentAsync(request, orderId, transactionId);

                // Create PTM Payment payload
                var paymentPayload = new
                {
                    merchantId = _ptmMerchantId,
                    orderId = orderId,
                    amount = request.Amount,
                    currency = "INR",
                    transactionId = transactionId,
                    customerEmail = request.Description,
                    description = $"Payment for Employee ID: {request.EmployeeId}",
                    returnUrl = $"{_configuration["PaymentGateway:ReturnUrl"]}/payment/callback",
                    notifyUrl = $"{_configuration["PaymentGateway:NotifyUrl"]}/api/payment/webhook"
                };

                // Create signature
                string signature = GenerateSignature(paymentPayload);
                paymentPayload.GetType().GetProperty("signature")?.SetValue(paymentPayload, signature);

                // Make request to PTM API
                var json = JsonSerializer.Serialize(paymentPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_ptmBaseUrl}/initiate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var ptmResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentResponse
                    {
                        Success = true,
                        Message = "Payment initiated successfully",
                        OrderId = orderId,
                        TransactionId = transactionId,
                        Amount = request.Amount,
                        PaymentStatus = "Pending",
                        PaymentUrl = ptmResponse.GetProperty("paymentUrl").GetString()
                    };
                }

                return new PaymentResponse
                {
                    Success = false,
                    Message = "Failed to initiate payment with PTM",
                    Amount = request.Amount
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error initiating payment: {ex.Message}"
                };
            }
        }

        public async Task<bool> VerifyPaymentSignatureAsync(PaymentCallbackRequest callback)
        {
            try
            {
                // Recreate signature with callback data
                var signatureData = new
                {
                    orderId = callback.OrderId,
                    transactionId = callback.TransactionId,
                    status = callback.Status,
                    amount = callback.Amount
                };

                string expectedSignature = GenerateSignature(signatureData);

                // Verify signature matches
                return callback.Signature == expectedSignature;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string orderId)
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

                return new PaymentResponse
                {
                    Success = true,
                    OrderId = payment.OrderId,
                    TransactionId = payment.TransactionId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error retrieving payment status: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> ProcessRefundAsync(string transactionId, decimal amount)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);

                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment not found for refund"
                    };
                }

                // Create refund payload for PTM
                var refundPayload = new
                {
                    merchantId = _ptmMerchantId,
                    transactionId = transactionId,
                    orderId = payment.OrderId,
                    amount = amount,
                    reason = "Customer requested refund"
                };

                string signature = GenerateSignature(refundPayload);

                var json = JsonSerializer.Serialize(new { refundPayload, signature });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_ptmBaseUrl}/refund", content);

                if (response.IsSuccessStatusCode)
                {
                    await _paymentRepository.UpdatePaymentStatusAsync(payment.Id, "Refunded", transactionId);

                    return new PaymentResponse
                    {
                        Success = true,
                        Message = "Refund processed successfully",
                        TransactionId = transactionId,
                        PaymentStatus = "Refunded"
                    };
                }

                return new PaymentResponse
                {
                    Success = false,
                    Message = "Refund processing failed"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error processing refund: {ex.Message}"
                };
            }
        }

        private string GenerateSignature(object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var signatureString = $"{json}{_ptmApiKey}";

                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
                }
            }
            catch
            {
                throw new InvalidOperationException("Failed to generate payment signature");
            }
        }
    }
}
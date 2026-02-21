using EmployeeManagement.UI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.UI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(HttpClient httpClient, ITokenService tokenService, ILogger<PaymentService> logger)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _logger = logger;
        }

        private async Task<bool> SetAuthorizationHeaderAsync()
        {
            try
            {
                var token = await _tokenService.GetTokenAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No token available for API request");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting authorization header: {ex.Message}");
                return false;
            }
        }

        public async Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request)
        {
            try
            {
                if (!await SetAuthorizationHeaderAsync())
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Authentication failed. Please login again."
                    };
                }

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/payment/initiate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PaymentResponse>(responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    _logger.LogInformation($"Payment initiated successfully: {result?.OrderId}");
                    return result ?? new PaymentResponse 
                    { 
                        Success = false, 
                        Message = "Failed to parse payment response" 
                    };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Payment initiation failed: {response.StatusCode} - {errorContent}");

                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Payment initiation failed: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initiating payment: {ex.Message}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string orderId)
        {
            try
            {
                if (!await SetAuthorizationHeaderAsync())
                {
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Authentication failed. Please login again."
                    };
                }

                var response = await _httpClient.GetAsync($"/api/payment/status/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                _logger.LogWarning($"Failed to get payment status for OrderId: {orderId}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Failed to retrieve payment status"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving payment status: {ex.Message}");
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<List<Payment>> GetEmployeePaymentsAsync(int employeeId)
        {
            try
            {
                if (!await SetAuthorizationHeaderAsync())
                {
                    _logger.LogWarning("Cannot retrieve payments - authentication failed");
                    return new List<Payment>();
                }

                var response = await _httpClient.GetAsync($"/api/payment/employee/{employeeId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var payments = JsonSerializer.Deserialize<List<Payment>>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    return payments ?? new List<Payment>();
                }

                _logger.LogWarning($"Failed to retrieve payments for EmployeeId: {employeeId}");
                return new List<Payment>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving employee payments: {ex.Message}");
                return new List<Payment>();
            }
        }

        public async Task<decimal> GetTotalPaymentsAsync(int employeeId)
        {
            try
            {
                if (!await SetAuthorizationHeaderAsync())
                {
                    return 0;
                }

                var response = await _httpClient.GetAsync($"/api/payment/total/{employeeId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (result.TryGetProperty("totalPayments", out var totalProperty))
                    {
                        return totalProperty.GetDecimal();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving total payments: {ex.Message}");
                return 0;
            }
        }
    }
}
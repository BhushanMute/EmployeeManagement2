using EmployeeManagement.UI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmployeeManagement.UI.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiService> _logger;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        public async  Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync(endpoint);
            return await ProcessResponse<T>(response);
        }

        public async  Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            return await ProcessResponse<T>(response);
        }

        public async  Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);
            return await ProcessResponse<T>(response);
        }

        public async  Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync(endpoint);
            return await ProcessResponse<T>(response);
        }

        private HttpClient CreateClient(string? token)
        {
            var client = _httpClientFactory.CreateClient("API");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }



        private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("API Response - StatusCode: {StatusCode}, Body: {Content}",
                response.StatusCode, content);

            // Handle empty response
            if (string.IsNullOrWhiteSpace(content))
            {
                return new ApiResponse<T>
                {
                    StatusCode = (int)response.StatusCode,
                    Status = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode
                        ? "Operation completed successfully"
                        : $"Request failed with status {response.StatusCode}",
                    Data = default
                };
            }

            try
            {
                // ✅ Parse JSON dynamically first
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                var apiResponse = new ApiResponse<T>
                {
                    StatusCode = (int)response.StatusCode  // ✅ FIX: Initialize StatusCode
                };

                // Handle "status" - could be bool, int, or string
                if (root.TryGetProperty("status", out var statusProp))
                {
                    apiResponse.Status = statusProp.ValueKind switch
                    {
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Number => statusProp.GetInt32() == 200 || statusProp.GetInt32() == 1,
                        JsonValueKind.String => statusProp.GetString()?.ToLower() == "success",
                        _ => response.IsSuccessStatusCode
                    };
                }
                // Handle "success" property
                else if (root.TryGetProperty("success", out var successProp))
                {
                    apiResponse.Status = successProp.GetBoolean();
                }
                // Handle "isSuccess" property
                else if (root.TryGetProperty("isSuccess", out var isSuccessProp))
                {
                    apiResponse.Status = isSuccessProp.GetBoolean();
                }
                // Handle "succeeded" property
                else if (root.TryGetProperty("succeeded", out var succeededProp))
                {
                    apiResponse.Status = succeededProp.GetBoolean();
                }
                else
                {
                    apiResponse.Status = response.IsSuccessStatusCode;
                }

                // Handle "message" - could be various property names
                if (root.TryGetProperty("message", out var msgProp))
                {
                    apiResponse.Message = msgProp.GetString();
                }
                else if (root.TryGetProperty("error", out var errorProp))
                {
                    apiResponse.Message = errorProp.GetString();
                }
                else if (root.TryGetProperty("errors", out var errorsProp) &&
                         errorsProp.ValueKind == JsonValueKind.Array)
                {
                    var errors = errorsProp.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(e => !string.IsNullOrEmpty(e));
                    apiResponse.Message = string.Join(", ", errors);
                }
                else
                {
                    apiResponse.Message = response.IsSuccessStatusCode
                        ? "Success"
                        : "Operation failed";  // ✅ FIX: Provide default message
                }

                // Handle "data" - could be various property names
                if (root.TryGetProperty("data", out var dataProp) &&
                    dataProp.ValueKind != JsonValueKind.Null)  // ✅ FIX: Check for null
                {
                    try
                    {
                        apiResponse.Data = JsonSerializer.Deserialize<T>(
                            dataProp.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize data property");
                        apiResponse.Data = default;  // ✅ FIX: Handle deserialization failure
                    }
                }
                else if (root.TryGetProperty("result", out var resultProp) &&
                         resultProp.ValueKind != JsonValueKind.Null)  // ✅ FIX: Check for null
                {
                    try
                    {
                        apiResponse.Data = JsonSerializer.Deserialize<T>(
                            resultProp.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize result property");
                        apiResponse.Data = default;  // ✅ FIX: Handle deserialization failure
                    }
                }
                else if (root.TryGetProperty("payload", out var payloadProp) &&
                         payloadProp.ValueKind != JsonValueKind.Null)  // ✅ FIX: Check for null
                {
                    try
                    {
                        apiResponse.Data = JsonSerializer.Deserialize<T>(
                            payloadProp.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize payload property");
                        apiResponse.Data = default;  // ✅ FIX: Handle deserialization failure
                    }
                }
                else
                {
                    apiResponse.Data = default;  // ✅ FIX: Ensure Data is set
                }

                return apiResponse;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed. Content: {Content}", content);

                return new ApiResponse<T>
                {
                    StatusCode = (int)response.StatusCode,  // ✅ FIX: Added StatusCode
                    Status = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? content : $"Error: {content}",
                    Data = default
                };
            }
        }

    }
}
 

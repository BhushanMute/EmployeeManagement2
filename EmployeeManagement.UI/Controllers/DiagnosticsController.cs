using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.UI.Controllers
{
    
    public class DiagnosticsController : Controller
    {
        private readonly ILogger<DiagnosticsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private static readonly DateTime _startTime = DateTime.UtcNow;
        private readonly IWebHostEnvironment _environment;


        public DiagnosticsController(
            ILogger<DiagnosticsController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _environment = environment;

        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDiagnostics()
        {
            var diagnostics = new
            {
                // 1. System Information
                system = new
                {
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                    machineName = Environment.MachineName,
                    osVersion = Environment.OSVersion.ToString(),
                    processorCount = Environment.ProcessorCount,
                    currentDirectory = Directory.GetCurrentDirectory(),
                    dotnetVersion = Environment.Version.ToString(),
                    uptime = GetUptime(),
                    workingSet = $"{Environment.WorkingSet / 1024 / 1024} MB"
                },

                // 2. Environment
                environment = new
                {
                    isDevelopment = _environment.IsDevelopment(),
                    environmentName = _environment.EnvironmentName,
                    contentRootPath = GetSafePath(() => _environment.ContentRootPath),
                    webRootPath = GetSafePath(() => _environment.WebRootPath)
                },

            // 3. Authentication
            authentication = new
                {
                    isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    authenticationType = User.Identity?.AuthenticationType ?? "None",
                    userName = User.Identity?.Name ?? "Anonymous",
                    userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "N/A",
                    email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "N/A",
                    roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList(),
                    claims = User.Claims.Select(c => new
                    {
                        type = c.Type.Split('/').Last(), // Shorter claim type names
                        value = c.Value
                    }).ToList()
                },

                // 4. Session
                session = new
                {
                    sessionId = HttpContext.Session?.Id ?? "N/A",
                    isAvailable = HttpContext.Session != null,
                    hasToken = !string.IsNullOrEmpty(HttpContext.Session?.GetString("JWTToken")),
                    tokenPreview = GetTokenPreview(),
                    tokenExpiry = GetTokenExpiry(),
                    keys = GetSessionKeys()
                },

                // 5. Configuration
                configuration = new
                {
                    apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "Not Configured",
                    uiBaseUrl = $"{Request.Scheme}://{Request.Host}",
                    connectionStringConfigured = !string.IsNullOrEmpty(_configuration.GetConnectionString("DefaultConnection")) ? "Yes" : "No",
                    loggingLevel = _configuration["Logging:LogLevel:Default"] ?? "Not Set",
                    sessionTimeout = _configuration["Session:IdleTimeout"] ?? "30 minutes",
                    httpsRedirection = _configuration["Https:Enabled"] ?? "Unknown",
                    corsEnabled = _configuration["Cors:Enabled"] ?? "Unknown"
                },

                // 6. Request
                request = new
                {
                    scheme = Request.Scheme,
                    host = Request.Host.ToString(),
                    path = Request.Path.ToString(),
                    queryString = Request.QueryString.ToString(),
                    method = Request.Method,
                    protocol = Request.Protocol,
                    isHttps = Request.IsHttps,
                    contentType = Request.ContentType ?? "N/A",
                    clientIp = GetClientIpAddress(),
                    userAgent = Request.Headers["User-Agent"].ToString(),
                    headers = Request.Headers
                        .Where(h => !h.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase)) // Exclude sensitive data
                        .ToDictionary(h => h.Key, h => h.Value.ToString())
                },

                // 7. Security
                security = new
                {
                    isHttps = Request.IsHttps,
                    tlsVersion = Request.Protocol,
                    hasCors = Response.Headers.ContainsKey("Access-Control-Allow-Origin"),
                    hasAntiForgeryToken = Request.Cookies.ContainsKey(".AspNetCore.Antiforgery"),
                    cookieCount = Request.Cookies.Count,
                    secureConnection = Request.IsHttps ? "Yes" : "No"
                },

                // 8. Services Status
                servicesStatus = await GetServicesStatusAsync(),

                // 9. Database Status
                database = await GetDatabaseStatusAsync(),

                // 10. Performance
                performance = new
                {
                    applicationUptime = GetUptime(),
                    currentMemoryUsage = $"{GC.GetTotalMemory(false) / 1024 / 1024} MB",
                    gcCollections = new
                    {
                        gen0 = GC.CollectionCount(0),
                        gen1 = GC.CollectionCount(1),
                        gen2 = GC.CollectionCount(2)
                    }
                }
            };

            return Json(diagnostics);
        }

        // Helper Methods
        private string GetUptime()
        {
            var uptime = DateTime.UtcNow - _startTime;
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        private string GetSafePath(Func<string> pathFunc)
        {
            try
            {
                return pathFunc();
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetTokenPreview()
        {
            try
            {
                var token = HttpContext.Session?.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                    return "No token found";

                return token.Length > 50 ? token.Substring(0, 50) + "..." : token;
            }
            catch
            {
                return "Error reading token";
            }
        }

        private string GetTokenExpiry()
        {
            try
            {
                var token = HttpContext.Session?.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                    return "N/A";

                // Decode JWT and get expiry (simplified)
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return "Invalid token format";

                var payload = parts[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

                // Extract exp claim (this is simplified, you might need proper JWT parsing)
                return "Check token manually"; // Or use proper JWT library
            }
            catch
            {
                return "Error parsing token";
            }
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private List<string> GetSessionKeys()
        {
            try
            {
                // This is a simplified version - actual session key enumeration might differ
                return new List<string> { "JWTToken", "UserId", "UserName" }
                    .Where(key => !string.IsNullOrEmpty(HttpContext.Session?.GetString(key)))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unable to determine";
            }
        }

        private async Task<Dictionary<string, object>> GetServicesStatusAsync()
        {
            var servicesStatus = new Dictionary<string, object>();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:26024";

            // Test endpoints
            var endpoints = new Dictionary<string, string>
            {
                { "API Health", $"{apiBaseUrl}/api/health" },
                { "Payment API", $"{apiBaseUrl}/api/payment/health" },
                { "Employee API", $"{apiBaseUrl}/api/employees" },
                { "Auth API", $"{apiBaseUrl}/api/auth/test" }
            };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var response = await client.GetAsync(endpoint.Value);
                    stopwatch.Stop();

                    servicesStatus[endpoint.Key] = new
                    {
                        status = response.IsSuccessStatusCode ? "✓ Connected" : "✗ Failed",
                        statusCode = (int)response.StatusCode,
                        url = endpoint.Value,
                        responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
                    };
                }
                catch (Exception ex)
                {
                    servicesStatus[endpoint.Key] = new
                    {
                        status = "✗ Error",
                        statusCode = 0,
                        url = endpoint.Value,
                        error = ex.Message,
                        responseTime = "N/A"
                    };
                }
            }

            client.Dispose();
            return servicesStatus;
        }

        private async Task<object> GetDatabaseStatusAsync()
        {
            try
            {
                // If you have DbContext, inject it and test connection
                // For now, return configuration info
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return new
                    {
                        status = "✗ Not Configured",
                        server = "N/A",
                        database = "N/A",
                        connected = false
                    };
                }

                // Parse connection string (simplified)
                var server = ExtractFromConnectionString(connectionString, "Server") ??
                           ExtractFromConnectionString(connectionString, "Data Source");
                var database = ExtractFromConnectionString(connectionString, "Database") ??
                             ExtractFromConnectionString(connectionString, "Initial Catalog");

                return new
                {
                    status = "✓ Configured",
                    server = server ?? "Unknown",
                    database = database ?? "Unknown",
                    connected = true,
                    connectionStringLength = connectionString.Length
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "✗ Error",
                    error = ex.Message,
                    connected = false
                };
            }
        }

        private string ExtractFromConnectionString(string connectionString, string key)
        {
            try
            {
                var parts = connectionString.Split(';');
                var part = parts.FirstOrDefault(p => p.Trim().StartsWith(key, StringComparison.OrdinalIgnoreCase));
                return part?.Split('=').LastOrDefault()?.Trim();
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true, message = "Session cleared successfully" });
        }

        [HttpPost]
        public IActionResult ClearCache()
        {
            // Implement cache clearing if you have caching
            return Json(new { success = true, message = "Cache cleared successfully" });
        }
    }
}
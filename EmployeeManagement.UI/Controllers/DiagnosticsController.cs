using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.UI.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly ILogger<DiagnosticsController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DiagnosticsController(
            ILogger<DiagnosticsController> logger,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var diagnostics = new Dictionary<string, object>
            {
                { "Timestamp", DateTime.UtcNow },
                { "Environment", new { IsDevelopment = true } },
                { "Configuration", new Dictionary<string, string>
                    {
                        { "API Base URL", "https://localhost:26024/" },
                        { "UI Base URL", $"{Request.Scheme}://{Request.Host}" }
                    }
                },
                { "Services Status", new Dictionary<string, string>() }
            };

            // Test API connectivity
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:26024/api/payment/health");
                ((Dictionary<string, string>)diagnostics["Services Status"])["API Health Check"] = 
                    response.IsSuccessStatusCode ? "? Connected" : "? Failed";
            }
            catch (Exception ex)
            {
                ((Dictionary<string, string>)diagnostics["Services Status"])["API Health Check"] = 
                    $"? Error: {ex.Message}";
            }

            return Ok(diagnostics);
        }
    }
}
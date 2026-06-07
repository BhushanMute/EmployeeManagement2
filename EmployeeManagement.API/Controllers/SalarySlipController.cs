using Dapper;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalarySlipController : ControllerBase
    {
        private readonly ISalarySlipService _salarySlipService;
        private readonly SsrsReportService _ssrsReportService;
        private readonly IConfiguration _config;

        public SalarySlipController(
            ISalarySlipService salarySlipService,
            SsrsReportService ssrsReportService,
            IConfiguration config)
        {
            _salarySlipService = salarySlipService;
            _ssrsReportService = ssrsReportService;
            _config = config;
        }

        [HttpGet("{slipId:int}")]
        [ProducesResponseType(typeof(ApiResponse<SalarySlipResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<SalarySlipResponse>>> GetSalarySlip(int slipId)
        {
            var result = await _salarySlipService.GetSalarySlipAsync(slipId);
            return result.Status ? Ok(result) : NotFound(result);
        }

        [HttpGet("my-slips")]
        [ProducesResponseType(typeof(ApiResponse<List<SalarySlipResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalarySlipResponse>>>> GetMySalarySlips(
            [FromQuery] int? year,
            [FromQuery] int? month)
        {
            var employeeId = GetCurrentUserId();

            if (employeeId <= 0)
            {
                return Unauthorized(ApiResponse<List<SalarySlipResponse>>.Fail("Invalid user token."));
            }

            var result = await _salarySlipService.GetEmployeeSalarySlipsAsync(employeeId, year, month);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId:int}")]
        [Authorize(Roles = "Admin,HR,Payroll")]
        [ProducesResponseType(typeof(ApiResponse<List<SalarySlipResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalarySlipResponse>>>> GetEmployeeSalarySlips(
            int employeeId,
            [FromQuery] int? year,
            [FromQuery] int? month)
        {
            var result = await _salarySlipService.GetEmployeeSalarySlipsAsync(employeeId, year, month);
            return Ok(result);
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin,HR,Payroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> GenerateSalarySlips(
            [FromBody] GenerateSalarySlipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail(
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));
            }

            var userId = GetCurrentUserId();

            var result = await _salarySlipService.GenerateSalarySlipsAsync(request, userId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("send-email")]
        [Authorize(Roles = "Admin,HR,Payroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> SendSalarySlipEmail(
            [FromBody] SendSalarySlipEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail(
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));
            }

            var result = await _salarySlipService.SendSalarySlipEmailAsync(request);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("send-bulk-email")]
        [Authorize(Roles = "Admin,HR,Payroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> SendBulkSalarySlipEmail(
            [FromBody] SendBulkSalarySlipEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail(
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));
            }

            var result = await _salarySlipService.SendBulkSalarySlipEmailAsync(request);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{slipId:int}/report-url")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public ActionResult<ApiResponse<string>> GetSalarySlipReportUrl(int slipId)
        {
            var result = _salarySlipService.GetReportUrl(slipId);
            return Ok(result);
        }

        [HttpPost("{slipId:int}/track-view")]
        public async Task<ActionResult<ApiResponse<bool>>> TrackView(int slipId)
        {
            var result = await _salarySlipService.TrackViewAsync(slipId);
            return Ok(result);
        }

        [HttpPost("{slipId:int}/track-download")]
        public async Task<ActionResult<ApiResponse<bool>>> TrackDownload(int slipId)
        {
            var result = await _salarySlipService.TrackDownloadAsync(slipId);
            return Ok(result);
        }

        [HttpGet("{slipId:int}/download")]
        [Authorize(Roles = "Admin,HR,Payroll,Employee")]
        public async Task<IActionResult> DownloadPdf(int slipId)
        {
            var pdf = await _ssrsReportService.GenerateSalarySlipPdfAsync(slipId);

            if (pdf == null || pdf.Length == 0)
            {
                return NotFound(ApiResponse<bool>.Fail("PDF could not be generated."));
            }

            return File(pdf, "application/pdf", $"SalarySlip_{slipId}.pdf");
        }

        [HttpGet("email-progress/{cycleId:int}")]
        [Authorize(Roles = "Admin,HR,Payroll")]
        public async Task<ActionResult<ApiResponse<EmailQueueStatusDto>>> GetEmailProgress(int cycleId)
        {
            try
            {
                using var connection = new SqlConnection(
                    _config.GetConnectionString("DefaultConnection"));

                var sql = @"
                    SELECT 
                        COUNT(*) AS Total,
                        SUM(CASE WHEN Status = 'Sent' THEN 1 ELSE 0 END) AS Sent,
                        SUM(CASE WHEN Status = 'Failed' THEN 1 ELSE 0 END) AS Failed,
                        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Pending
                    FROM PayrollEmailQueue
                    WHERE CycleId = @CycleId;
                ";

                var status = await connection.QueryFirstOrDefaultAsync<EmailQueueStatusDto>(
                    sql,
                    new { CycleId = cycleId });

                return Ok(ApiResponse<EmailQueueStatusDto>.Success(
                    status ?? new EmailQueueStatusDto()));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<EmailQueueStatusDto>.Fail("Server error while fetching email progress."));
            }
        }

        private int GetCurrentUserId()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("userId")?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userId, out var parsedUserId) ? parsedUserId : 0;
        }
    }

    public class EmailQueueStatusDto
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public string? Status { get; set; }

        public double ProgressPercentage =>
            Total > 0 ? Math.Round((double)(Sent + Failed) / Total * 100, 2) : 0;
    }
}
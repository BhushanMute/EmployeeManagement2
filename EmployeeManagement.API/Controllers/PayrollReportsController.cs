using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    /// <summary>
    /// Payroll Reports Controller
    /// वेतन अहवाल नियंत्रक
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Payroll.ViewReports")]
    public class PayrollReportsController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly ILogger<PayrollReportsController> _logger;

        public PayrollReportsController(IPayrollService payrollService, ILogger<PayrollReportsController> logger)
        {
            _payrollService = payrollService;
            _logger = logger;
        }

        #region Payroll Reports

        /// <summary>
        /// Get Payroll Register Report
        /// वेतन नोंदणी अहवाल मिळवा
        /// </summary>
        [HttpGet("payroll-register/{cycleId}")]
        [ProducesResponseType(typeof(ApiResponse<PayrollRegisterResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PayrollRegisterResponse>>> GetPayrollRegisterReport(int cycleId)
        {
            var result = await _payrollService.GetPayrollRegisterReportAsync(cycleId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Bank Transfer Report
        /// बँक हस्तांतरण अहवाल मिळवा
        /// </summary>
        [HttpGet("bank-transfer/{cycleId}")]
        [ProducesResponseType(typeof(ApiResponse<BankTransferResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<BankTransferResponse>>> GetBankTransferReport(int cycleId)
        {
            var result = await _payrollService.GetBankTransferReportAsync(cycleId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Department-wise Salary Report
        /// विभागनिहाय वेतन अहवाल मिळवा
        /// </summary>
        [HttpGet("department-wise/{cycleId}")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentWiseSalaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<DepartmentWiseSalaryResponse>>> GetDepartmentWiseSalaryReport(int cycleId)
        {
            var result = await _payrollService.GetDepartmentWiseSalaryReportAsync(cycleId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Statutory Compliance Report
        /// वैधानिक अनुपालन अहवाल मिळवा
        /// </summary>
        [HttpGet("statutory-compliance")]
        [ProducesResponseType(typeof(ApiResponse<StatutoryComplianceResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<StatutoryComplianceResponse>>> GetStatutoryComplianceReport(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(ApiResponse<StatutoryComplianceResponse>.Fail("Invalid month. Must be between 1 and 12."));
            }

            var result = await _payrollService.GetStatutoryComplianceReportAsync(month, year);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        #endregion

        #region Export Reports

        /// <summary>
        /// Export Payroll Register to Excel
        /// वेतन नोंदणी Excel मध्ये निर्यात करा
        /// </summary>
        [HttpGet("payroll-register/{cycleId}/export/excel")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportPayrollRegisterToExcel(int cycleId)
        {
            try
            {
                var result = await _payrollService.GetPayrollRegisterReportAsync(cycleId);

                if (!result.Status || result.Data == null)
                {
                    return NotFound(ApiResponse<bool>.Fail("Payroll register not found"));
                }

                // Implementation: Use EPPlus or ClosedXML to generate Excel
                // Placeholder for now
                byte[] excelBytes = Array.Empty<byte>();

                var fileName = $"PayrollRegister_{cycleId}_{DateTime.Now:yyyyMMdd}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting payroll register to Excel: {CycleId}", cycleId);
                return BadRequest(ApiResponse<bool>.Fail("Error exporting report"));
            }
        }

        /// <summary>
        /// Download Bank Transfer File (NEFT/RTGS format)
        /// बँक हस्तांतरण फाइल डाउनलोड करा
        /// </summary>
        [HttpGet("bank-transfer/{cycleId}/download")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadBankTransferFile(int cycleId, [FromQuery] string format = "Excel")
        {
            try
            {
                var result = await _payrollService.GetBankTransferReportAsync(cycleId);

                if (!result.Status || result.Data == null)
                {
                    return NotFound(ApiResponse<bool>.Fail("Bank transfer data not found"));
                }

                // Implementation: Generate bank file format
                byte[] fileBytes = Array.Empty<byte>();
                string contentType = format.ToLower() == "excel"
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "text/plain";
                string fileName = $"BankTransfer_{cycleId}_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "excel" ? "xlsx" : "txt")}";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading bank transfer file: {CycleId}", cycleId);
                return BadRequest(ApiResponse<bool>.Fail("Error downloading bank file"));
            }
        }

        #endregion

        #region SSRS Report URLs

        /// <summary>
        /// Get SSRS Payroll Register Report URL
        /// </summary>
        [HttpGet("ssrs/payroll-register/{cycleId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public ActionResult<ApiResponse<string>> GetSSRSPayrollRegisterUrl(int cycleId)
        {
            var reportUrl = $"/ReportServer?/PayrollReports/PayrollRegister&CycleId={cycleId}&rs:Format=PDF";

            return Ok(ApiResponse<string>.Success(reportUrl));
        }

        /// <summary>
        /// Get SSRS Statutory Compliance Report URL
        /// </summary>
        [HttpGet("ssrs/statutory-compliance")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public ActionResult<ApiResponse<string>> GetSSRSStatutoryComplianceUrl([FromQuery] int month, [FromQuery] int year)
        {
            var reportUrl = $"/ReportServer?/PayrollReports/StatutoryCompliance&Month={month}&Year={year}&rs:Format=PDF";

            return Ok(ApiResponse<string>.Success(reportUrl));
        }

        #endregion
    }
}
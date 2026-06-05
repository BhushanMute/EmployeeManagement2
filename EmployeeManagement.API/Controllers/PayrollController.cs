using EmployeeManagement.API.Common;
 
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    /// <summary>
    /// Payroll Controller
    /// वेतन प्रक्रिया नियंत्रक
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly ILogger<PayrollController> _logger;

        public PayrollController(IPayrollService payrollService, ILogger<PayrollController> logger)
        {
            _payrollService = payrollService;
            _logger = logger;
        }

        #region Payroll Cycle Management

        /// <summary>
        /// Create new payroll cycle
        /// नवीन वेतन सायकल तयार करा
        /// </summary>
        /// <param name="month">Month (1-12)</param>
        /// <param name="year">Year (e.g., 2024)</param>
        [HttpPost("cycle/create")]
        [Authorize(Policy = "Payroll.CreateCycle")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<int>>> CreatePayrollCycle([FromQuery] int month, [FromQuery] int year)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.CreatePayrollCycleAsync(month, year, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get payroll summary by cycle ID
        /// सायकल ID द्वारे वेतन सारांश मिळवा
        /// </summary>
        [HttpGet("cycle/{cycleId}/summary")]
        [Authorize(Policy = "Payroll.ViewCycle")]
        [ProducesResponseType(typeof(ApiResponse<PayrollSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PayrollSummaryResponse>>> GetPayrollSummary(int cycleId)
        {
            var result = await _payrollService.GetPayrollSummaryAsync(cycleId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all payroll cycles for a year
        /// वर्षाच्या सर्व वेतन सायकल मिळवा
        /// </summary>
        [HttpGet("cycles")]
        [Authorize(Policy = "Payroll.ViewCycle")]
        [ProducesResponseType(typeof(ApiResponse<List<PayrollSummaryResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PayrollSummaryResponse>>>> GetPayrollCycles([FromQuery] int year)
        {
            var result = await _payrollService.GetPayrollCyclesAsync(year);

            return Ok(result);
        }

        /// <summary>
        /// Lock payroll cycle
        /// वेतन सायकल लॉक करा
        /// </summary>
        [HttpPost("cycle/{cycleId}/lock")]
        [Authorize(Policy = "Payroll.LockPayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> LockPayrollCycle(int cycleId)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.LockPayrollCycleAsync(cycleId, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Approve payroll cycle
        /// वेतन सायकल मंजूर करा
        /// </summary>
        [HttpPost("cycle/{cycleId}/approve")]
        [Authorize(Policy = "Payroll.ApprovePayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> ApprovePayrollCycle(int cycleId, [FromBody] string? remarks = null)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.ApprovePayrollCycleAsync(cycleId, userId, remarks);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Payroll Processing

        /// <summary>
        /// Process payroll for single employee
        /// एकल कर्मचाऱ्यासाठी वेतन प्रक्रिया करा
        /// </summary>
        [HttpPost("process/employee")]
        [Authorize(Policy = "Payroll.ProcessPayroll")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> ProcessSingleEmployeePayroll(
            [FromBody] ProcessSingleEmployeePayrollRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<int>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _payrollService.ProcessSingleEmployeePayrollAsync(request, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Process bulk payroll
        /// एकत्रित वेतन प्रक्रिया करा
        /// </summary>
        [HttpPost("process/bulk")]
        [Authorize(Policy = "Payroll.ProcessPayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> ProcessBulkPayroll([FromBody] ProcessPayrollRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _payrollService.ProcessBulkPayrollAsync(request, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get employee payroll details
        /// कर्मचारी वेतन तपशील मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/cycle/{cycleId}")]
        [Authorize(Policy = "Payroll.ViewCycle")]
        [ProducesResponseType(typeof(ApiResponse<EmployeePayrollDetailResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<EmployeePayrollDetailResponse>>> GetEmployeePayrollDetails(
            int cycleId, int employeeId)
        {
            var result = await _payrollService.GetEmployeePayrollDetailsAsync(cycleId, employeeId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get payroll register
        /// वेतन नोंदणी मिळवा
        /// </summary>
        [HttpGet("cycle/{cycleId}/register")]
        [Authorize(Policy = "Payroll.ViewCycle")]
        [ProducesResponseType(typeof(ApiResponse<List<EmployeePayrollDetailResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<EmployeePayrollDetailResponse>>>> GetPayrollRegister(int cycleId)
        {
            var result = await _payrollService.GetPayrollRegisterAsync(cycleId);

            return Ok(result);
        }

        #endregion

        #region Payroll Actions

        /// <summary>
        /// Recalculate payroll
        /// वेतन पुन्हा गणना करा
        /// </summary>
        [HttpPost("{processingId}/recalculate")]
        [Authorize(Policy = "Payroll.ProcessPayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> RecalculatePayroll(int processingId)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.RecalculatePayrollAsync(processingId, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Hold payroll
        /// वेतन होल्ड करा
        /// </summary>
        [HttpPost("{processingId}/hold")]
        [Authorize(Policy = "Payroll.ProcessPayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> HoldPayroll(int processingId, [FromBody] string reason)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.HoldPayrollAsync(processingId, reason, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Release payroll hold
        /// वेतन होल्ड सोडवा
        /// </summary>
        [HttpPost("{processingId}/release")]
        [Authorize(Policy = "Payroll.ProcessPayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> ReleasePayroll(int processingId)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.ReleasePayrollAsync(processingId, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Mark payroll as paid
        /// वेतन पेड म्हणून चिन्हांकित करा
        /// </summary>
        [HttpPost("{processingId}/mark-paid")]
        [Authorize(Policy = "Payroll.ApprovePayroll")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> MarkPayrollAsPaid(
            int processingId,
            [FromQuery] string paymentMode,
            [FromQuery] string? referenceNo = null)
        {
            var userId = GetCurrentUserId();
            var result = await _payrollService.MarkPayrollAsPaidAsync(processingId, paymentMode, referenceNo, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Dashboard

        /// <summary>
        /// Get payroll dashboard
        /// वेतन डॅशबोर्ड मिळवा
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Policy = "Payroll.ViewCycle")]
        [ProducesResponseType(typeof(ApiResponse<PayrollDashboardResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PayrollDashboardResponse>>> GetPayrollDashboard()
        {
            var result = await _payrollService.GetPayrollDashboardAsync();

            return Ok(result);
        }

        #endregion

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        #endregion
    }
}
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Services;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace EmployeeManagement.API.Controllers
{
    /// <summary>
    /// Loan Management Controller
    /// कर्ज व्यवस्थापन नियंत्रक
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoanController> _logger;

        public LoanController(ILoanService loanService, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        #region Loan Types

        /// <summary>
        /// Get all loan types
        /// सर्व कर्ज प्रकार मिळवा
        /// </summary>
        [HttpGet("types")]
        [Authorize(Policy = "Loan.View")]
        [OutputCache(PolicyName = "LoanTypes")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanType>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LoanType>>>> GetAllLoanTypes([FromQuery] bool activeOnly = true)
        {
            var result = await _loanService.GetAllLoanTypesAsync(activeOnly);

            return Ok(result);
        }

        /// <summary>
        /// Get loan type by ID
        /// </summary>
        [HttpGet("types/{loanTypeId}")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<LoanType>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<LoanType>>> GetLoanTypeById(int loanTypeId)
        {
            var result = await _loanService.GetLoanTypeByIdAsync(loanTypeId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        #endregion

        #region Loan Application

        /// <summary>
        /// Apply for loan
        /// कर्जासाठी अर्ज करा
        /// </summary>
        [HttpPost("apply")]
        [Authorize(Policy = "Loan.Apply")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<int>>> ApplyForLoan([FromForm] LoanApplicationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<int>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            // Handle file upload if provided
            if (request.SupportingDocument != null && request.SupportingDocument.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "loan-documents");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{request.SupportingDocument.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.SupportingDocument.CopyToAsync(stream);
                }

                // Store relative path in request (you'll need to add this property to the request model)
                // request.AttachmentPath = $"/uploads/loan-documents/{uniqueFileName}";
            }

            var userId = GetCurrentUserId();
            var result = await _loanService.ApplyForLoanAsync(request, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get loan details
        /// कर्ज तपशील मिळवा
        /// </summary>
        [HttpGet("{loanId}")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<LoanDetailsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<LoanDetailsResponse>>> GetLoanDetails(int loanId)
        {
            var result = await _loanService.GetLoanDetailsAsync(loanId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get employee loans
        /// कर्मचारी कर्जे मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanDetailsResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LoanDetailsResponse>>>> GetEmployeeLoans(
            int employeeId,
            [FromQuery] string? status = null)
        {
            var result = await _loanService.GetEmployeeLoansAsync(employeeId, status);

            return Ok(result);
        }

        /// <summary>
        /// Get my loans (for logged-in employee)
        /// माझी कर्जे मिळवा
        /// </summary>
        [HttpGet("my-loans")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<LoanDetailsResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LoanDetailsResponse>>>> GetMyLoans([FromQuery] string? status = null)
        {
            var employeeId = GetCurrentUserId();
            var result = await _loanService.GetEmployeeLoansAsync(employeeId, status);

            return Ok(result);
        }

        /// <summary>
        /// Get pending loan approvals
        /// प्रलंबित कर्ज मंजुरी मिळवा
        /// </summary>
        [HttpGet("pending-approvals")]
        [Authorize(Policy = "Loan.Approve")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanDetailsResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LoanDetailsResponse>>>> GetPendingLoanApprovals()
        {
            var result = await _loanService.GetPendingLoanApprovalsAsync();

            return Ok(result);
        }

        #endregion

        #region Loan Approval

        /// <summary>
        /// Approve loan
        /// कर्ज मंजूर करा
        /// </summary>
        [HttpPost("{loanId}/approve")]
        [Authorize(Policy = "Loan.Approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveLoan(int loanId, [FromBody] ApproveLoanRequest request)
        {
            if (loanId != request.LoanId)
            {
                return BadRequest(ApiResponse<bool>.Fail("Loan ID mismatch"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _loanService.ApproveLoanAsync(request, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reject loan
        /// कर्ज नाकारा
        /// </summary>
        [HttpPost("{loanId}/reject")]
        [Authorize(Policy = "Loan.Approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> RejectLoan(int loanId, [FromBody] RejectLoanRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(ApiResponse<bool>.Fail("Rejection reason is required"));
            }

            var userId = GetCurrentUserId();
            var result = await _loanService.RejectLoanAsync(loanId, request.Reason, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Loan Disbursement

        /// <summary>
        /// Disburse loan
        /// कर्ज वितरित करा
        /// </summary>
        [HttpPost("{loanId}/disburse")]
        [Authorize(Policy = "Loan.Disburse")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> DisburseLoan(int loanId, [FromBody] DisburseLoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _loanService.DisburseLoanAsync(
                loanId,
                request.DisbursementDate,
                request.Mode,
                request.ReferenceNo,
                userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region EMI Management

        /// <summary>
        /// Get loan EMI schedule
        /// कर्ज EMI शेड्यूल मिळवा
        /// </summary>
        [HttpGet("{loanId}/emi-schedule")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanEMIResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<LoanEMIResponse>>>> GetLoanEMISchedule(int loanId)
        {
            var result = await _loanService.GetLoanEMIScheduleAsync(loanId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get monthly EMI deduction for employee
        /// कर्मचाऱ्यासाठी मासिक EMI कपात मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/monthly-emi")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetMonthlyEMIDeduction(int employeeId)
        {
            var result = await _loanService.GetMonthlyEMIDeductionAsync(employeeId);

            return Ok(result);
        }

        #endregion

        #region Loan Summary

        /// <summary>
        /// Get loan summary for employee
        /// कर्मचाऱ्यासाठी कर्ज सारांश मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/summary")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<LoanSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<LoanSummaryResponse>>> GetLoanSummary(int employeeId)
        {
            var result = await _loanService.GetLoanSummaryAsync(employeeId);

            return Ok(result);
        }

        /// <summary>
        /// Get my loan summary
        /// माझा कर्ज सारांश मिळवा
        /// </summary>
        [HttpGet("my-summary")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<LoanSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<LoanSummaryResponse>>> GetMyLoanSummary()
        {
            var employeeId = GetCurrentUserId();
            var result = await _loanService.GetLoanSummaryAsync(employeeId);

            return Ok(result);
        }

        /// <summary>
        /// Get total loan outstanding
        /// एकूण कर्ज थकबाकी मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/outstanding")]
        [Authorize(Policy = "Loan.View")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalLoanOutstanding(int employeeId)
        {
            var result = await _loanService.GetTotalLoanOutstandingAsync(employeeId);

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

    #region Request Models

    
    

    #endregion
}
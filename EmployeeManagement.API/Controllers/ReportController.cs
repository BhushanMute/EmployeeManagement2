using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepo;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportRepository reportRepo, ILogger<ReportController> logger)
        {
            _reportRepo = reportRepo;
            _logger = logger;
        }

        /// <summary>
        /// Employee Report
        /// </summary>
        [HttpGet("employee")]
        public async Task<ActionResult<ApiResponse<EmployeeReportData>>> GetEmployeeReport( [FromQuery] int? departmentId = null, [FromQuery] bool? isActive = null, [FromQuery] DateTime? joiningFrom = null, [FromQuery] DateTime? joiningTo = null, [FromQuery] string? searchTerm = null)
        {
            try
            {
                var report = await _reportRepo.GetEmployeeReport(departmentId, isActive, joiningFrom, joiningTo, searchTerm);
                return Ok(ApiResponse<EmployeeReportData>.Success(report, "Employee report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating employee report");
                return StatusCode(500, ApiResponse<EmployeeReportData>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Attendance Report
        /// </summary>
        [HttpGet("attendance")]
        public async Task<ActionResult<ApiResponse<AttendanceReportData>>> GetAttendanceReport( [FromQuery] int? month = null, [FromQuery] int? year = null, [FromQuery] int? departmentId = null, [FromQuery] int? employeeId = null)
        {
            try
            {
                var m = month ?? DateTime.Now.Month;
                var y = year ?? DateTime.Now.Year;
                var report = await _reportRepo.GetAttendanceReport(m, y, departmentId, employeeId);
                return Ok(ApiResponse<AttendanceReportData>.Success(report, "Attendance report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating attendance report");
                return StatusCode(500, ApiResponse<AttendanceReportData>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Salary Report
        /// </summary>
        [HttpGet("salary")]
        public async Task<ActionResult<ApiResponse<SalaryReportData>>> GetSalaryReport( [FromQuery] int? departmentId = null, [FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            try
            {
                var report = await _reportRepo.GetSalaryReport(departmentId, month, year);
                return Ok(ApiResponse<SalaryReportData>.Success(report, "Salary report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating salary report");
                return StatusCode(500, ApiResponse<SalaryReportData>.Fail("An error occurred"));
            }
        }
    }
}
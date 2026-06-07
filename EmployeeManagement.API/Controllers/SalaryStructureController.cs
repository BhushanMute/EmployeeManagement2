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
    /// Salary Structure Controller
    /// वेतन रचना नियंत्रक
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalaryStructureController : ControllerBase
    {
        private readonly ISalaryStructureService _salaryStructureService;
        private readonly ILogger<SalaryStructureController> _logger;

        public SalaryStructureController(
            ISalaryStructureService salaryStructureService,
            ILogger<SalaryStructureController> logger)
        {
            _salaryStructureService = salaryStructureService;
            _logger = logger;
        }

        #region Salary Components

        /// <summary>
        /// Get all salary components
        /// सर्व वेतन घटक मिळवा
        /// </summary>
        [HttpGet("components")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [OutputCache(PolicyName = "SalaryComponents")]
        [ProducesResponseType(typeof(ApiResponse<List<SalaryComponent>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalaryComponent>>>> GetAllSalaryComponents(
            [FromQuery] bool activeOnly = true)
        {
            var result = await _salaryStructureService.GetAllSalaryComponentsAsync(activeOnly);

            return Ok(result);
        }

        /// <summary>
        /// Get salary component by ID
        /// </summary>
        [HttpGet("components/{componentId}")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<SalaryComponent>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<SalaryComponent>>> GetSalaryComponentById(int componentId)
        {
            var result = await _salaryStructureService.GetSalaryComponentByIdAsync(componentId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new salary component
        /// नवीन वेतन घटक तयार करा
        /// </summary>
        [HttpPost("components")]
        [Authorize(Policy = "Salary.ManageComponents")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> CreateSalaryComponent([FromBody] SalaryComponent component)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<int>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.CreateSalaryComponentAsync(component, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update salary component
        /// वेतन घटक अपडेट करा
        /// </summary>
        [HttpPut("components/{componentId}")]
        [Authorize(Policy = "Salary.ManageComponents")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateSalaryComponent(
            int componentId,
            [FromBody] SalaryComponent component)
        {
            if (componentId != component.Id)
            {
                return BadRequest(ApiResponse<bool>.Fail("Component ID mismatch"));
            }

            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.UpdateSalaryComponentAsync(component, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete salary component
        /// वेतन घटक हटवा
        /// </summary>
        [HttpDelete("components/{componentId}")]
        [Authorize(Policy = "Salary.ManageComponents")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteSalaryComponent(int componentId)
        {
            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.DeleteSalaryComponentAsync(componentId, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Salary Templates

        /// <summary>
        /// Get all salary templates
        /// सर्व वेतन टेम्पलेट मिळवा
        /// </summary>
        [HttpGet("templates")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<List<SalaryTemplate>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalaryTemplate>>>> GetAllTemplates([FromQuery] bool activeOnly = true)
        {
            var result = await _salaryStructureService.GetAllTemplatesAsync(activeOnly);

            return Ok(result);
        }

        /// <summary>
        /// Get salary template by ID
        /// </summary>
        [HttpGet("templates/{templateId}")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<SalaryTemplate>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<SalaryTemplate>>> GetTemplateById(int templateId)
        {
            var result = await _salaryStructureService.GetTemplateByIdAsync(templateId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get template components
        /// टेम्पलेट घटक मिळवा
        /// </summary>
        [HttpGet("templates/{templateId}/components")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<List<SalaryTemplateComponent>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalaryTemplateComponent>>>> GetTemplateComponents(int templateId)
        {
            var result = await _salaryStructureService.GetTemplateComponentsAsync(templateId);

            return Ok(result);
        }

        #endregion

        #region Employee Salary Structure

        /// <summary>
        /// Get employee's current salary structure
        /// कर्मचाऱ्याची सध्याची वेतन रचना मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/current")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<SalaryStructureResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<SalaryStructureResponse>>> GetEmployeeCurrentSalary(int employeeId)
        {
            var result = await _salaryStructureService.GetEmployeeCurrentSalaryAsync(employeeId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get employee salary history
        /// कर्मचारी वेतन इतिहास मिळवा
        /// </summary>
        [HttpGet("employee/{employeeId}/history")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<List<SalaryStructureResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<SalaryStructureResponse>>>> GetEmployeeSalaryHistory(int employeeId)
        {
            var result = await _salaryStructureService.GetEmployeeSalaryHistoryAsync(employeeId);

            return Ok(result);
        }

        /// <summary>
        /// Assign salary structure to employee
        /// कर्मचाऱ्याला वेतन रचना नियुक्त करा
        /// </summary>
        [HttpPost("employee/assign")]
        [Authorize(Policy = "Salary.AssignStructure")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> AssignSalaryToEmployee([FromBody] AssignSalaryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<int>.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.AssignSalaryToEmployeeAsync(request, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get salary structure details
        /// वेतन रचना तपशील मिळवा
        /// </summary>
        [HttpGet("structure/{structureId}")]
        [Authorize(Policy = "Salary.ViewStructure")]
        [ProducesResponseType(typeof(ApiResponse<SalaryStructureResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<SalaryStructureResponse>>> GetSalaryStructureDetails(int structureId)
        {
            var result = await _salaryStructureService.GetSalaryStructureDetailsAsync(structureId);

            if (!result.Status)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update salary component amount
        /// वेतन घटक रक्कम अपडेट करा
        /// </summary>
        [HttpPut("component/{componentId}/amount")]
        [Authorize(Policy = "Salary.UpdateStructure")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateComponentAmount(
            int componentId,
            [FromBody] decimal amount)
        {
            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.UpdateSalaryComponentAsync(componentId, amount, userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Bulk assign salary to multiple employees
        /// अनेक कर्मचाऱ्यांना एकत्रित वेतन नियुक्त करा
        /// </summary>
        [HttpPost("employee/bulk-assign")]
        [Authorize(Policy = "Salary.AssignStructure")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> BulkAssignSalary([FromBody] BulkSalaryAssignRequest request)
        {
            if (!ModelState.IsValid || request.EmployeeIds == null || !request.EmployeeIds.Any())
            {
                return BadRequest(ApiResponse<bool>.Fail("Invalid request. Employee IDs are required."));
            }

            var userId = GetCurrentUserId();
            var result = await _salaryStructureService.BulkAssignSalaryAsync(
                request.EmployeeIds,
                request.TemplateId,
                request.EffectiveFrom,
                userId);

            if (!result.Status)
            {
                return BadRequest(result);
            }

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

    /// <summary>
    /// Bulk Salary Assignment Request
    /// </summary>
    public class BulkSalaryAssignRequest
    {
        public List<int> EmployeeIds { get; set; } = new();
        public int TemplateId { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}
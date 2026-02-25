using EmployeeManagement.API.Attributes;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient.Diagnostics;
namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAuditService _auditService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController( IEmployeeRepository repo, IAuditService auditService, ILogger<EmployeeController> logger) { _repo = repo; _auditService = auditService; _logger = logger; }

        #region Helper Methods

        /// <summary>
        /// Get current logged-in user ID
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

         
        private string? GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        #endregion

        #region GET Methods

        /// <summary>
        /// Get all employees - All authenticated users with Employee.View permission
        /// </summary>

        [HttpGet]
        //[Authorize(Roles = "Employee,HR,Admin")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> GetAll()
        {
            try
            {
                _logger.LogInformation("User {UserId} requesting all employees", GetCurrentUserId());

                var employees = await _repo.GetAll();

                return Ok(ApiResponse<List<Employee>>.Success(employees, "Employees retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                return StatusCode(500, ApiResponse<List<Employee>>.Fail("An error occurred while retrieving employees"));
            }
        }

        /// <summary>
        /// Get all employees with pagination, search, and filtering
        /// </summary>
        [HttpGet("paged")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<PagedResult<Employee>>>> GetAllPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? department = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] string? sortOrder = "asc")
        {
            try
            {
                _logger.LogInformation("User {UserId} requesting paged employees - Page: {Page}, Size: {Size}",
                    GetCurrentUserId(), pageNumber, pageSize);

                var result = await _repo.GetAllPaged(pageNumber, pageSize, searchTerm, departmentId:department, isActive, sortBy, sortOrder);

                return Ok(ApiResponse<PagedResult<Employee>>.Success(result, "Employees retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged employees");
                return StatusCode(500, ApiResponse<PagedResult<Employee>>.Fail("An error occurred while retrieving employees"));
            }
        }

        /// <summary>
        /// Get employee by ID - All authenticated users with Employee.ViewDetails permission
        /// </summary>
        [HttpGet("{id:int}")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<Employee>>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("User {UserId} requesting employee {EmployeeId}", GetCurrentUserId(), id);

                var employee = await _repo.GetById(id);

                if (employee == null)
                {
                    return NotFound(ApiResponse<Employee>.Fail($"Employee with ID {id} not found"));
                }

                return Ok(ApiResponse<Employee>.Success(employee, "Employee retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<Employee>.Fail("An error occurred while retrieving the employee"));
            }
        }

        /// <summary>
        /// Search employees by name or email
        /// </summary>
        [HttpGet("search")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(ApiResponse<List<Employee>>.Fail("Search term is required"));
                }

                var employees = await _repo.Search(term);

                return Ok(ApiResponse<List<Employee>>.Success(employees, $"Found {employees.Count} employees"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching employees with term: {Term}", term);
                return StatusCode(500, ApiResponse<List<Employee>>.Fail("An error occurred while searching employees"));
            }
        }

        /// <summary>
        /// Get employees by department
        /// </summary>
        [HttpGet("department/{department}")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> GetByDepartment(string department)
        {
            try
            {
                var employees = await _repo.GetByDepartment( Convert.ToInt32(department));

                return Ok(ApiResponse<List<Employee>>.Success(employees, $"Found {employees.Count} employees in {department}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees by department: {Department}", department);
                return StatusCode(500, ApiResponse<List<Employee>>.Fail("An error occurred while retrieving employees"));
            }
        }

        /// <summary>
        /// Get active employees count
        /// </summary>
        [HttpGet("count")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<object>>> GetCount()
        {
            try
            {
                var totalCount = await _repo.GetTotalCount();
                var activeCount = await _repo.GetActiveCount();

                var result = new
                {
                    TotalEmployees = totalCount,
                    ActiveEmployees = activeCount,
                    InactiveEmployees = totalCount - activeCount
                };

                return Ok(ApiResponse<object>.Success(result, "Employee count retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee count");
                return StatusCode(500, ApiResponse<object>.Fail("An error occurred while retrieving employee count"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>
        /// Create employee - HR and Admin only
        /// </summary>
        [HttpPost]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<Employee>>> Create([FromBody] EmployeeCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<Employee>.Fail("Validation failed", errors));
                }

                var currentUserId = GetCurrentUserId();

                // Check if email already exists
                var existingEmployee = await _repo.GetByEmail(request.Email);
                if (existingEmployee != null)
                {
                    return BadRequest(ApiResponse<Employee>.Fail("An employee with this email already exists"));
                }

                var employee = new Employee
                {
                    Name = request.Name,
                    Email = request.Email,
                    DepartmentName = request.Department,
                    Role = request.Role,
                    Salary = request.Salary,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    JoiningDate = request.JoiningDate ?? DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = currentUserId,
                    CreatedDate = DateTime.Now
                };

                var createdId = await _repo.Add(employee);
                employee.Id = createdId;

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Created",
                    "Employees",
                    createdId,
                    null,employee.Email);

                _logger.LogInformation("Employee {EmployeeId} created by User {UserId}", createdId, currentUserId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdId },
                    ApiResponse<Employee>.Success(employee, "Employee created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, ApiResponse<Employee>.Fail("An error occurred while creating the employee"));
            }
        }

        /// <summary>
        /// Bulk create employees - HR and Admin only
        /// </summary>
        [HttpPost("bulk")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkCreate([FromBody] List<EmployeeCreateRequest> requests)
        {
            try
            {
                if (requests == null || !requests.Any())
                {
                    return BadRequest(ApiResponse<BulkOperationResult>.Fail("No employees provided"));
                }

                var currentUserId = GetCurrentUserId();
                var successCount = 0;
                var failedCount = 0;
                var errors = new List<string>();

                foreach (var request in requests)
                {
                    try
                    {
                        // Check if email already exists
                        var existingEmployee = await _repo.GetByEmail(request.Email);
                        if (existingEmployee != null)
                        {
                            failedCount++;
                            errors.Add($"Email {request.Email} already exists");
                            continue;
                        }

                        var employee = new Employee
                        {
                            Name = request.Name,
                            Email = request.Email,
                            DepartmentName = request.Department,
                            Role = request.Role,
                            Salary = request.Salary,
                            PhoneNumber = request.PhoneNumber,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = currentUserId,
                            CreatedDate = DateTime.Now
                        };

                        await _repo.Add(employee);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        errors.Add($"Failed to create employee {request.Email}: {ex.Message}");
                    }
                }

                var result = new BulkOperationResult
                {
                    TotalRecords = requests.Count,
                    SuccessCount = successCount,
                    FailedCount = failedCount,
                    Errors = errors
                };

                _logger.LogInformation("Bulk create: {Success} succeeded, {Failed} failed", successCount, failedCount);

                return Ok(ApiResponse<BulkOperationResult>.Success(result, $"Bulk operation completed: {successCount} succeeded, {failedCount} failed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk create employees");
                return StatusCode(500, ApiResponse<BulkOperationResult>.Fail("An error occurred during bulk creation"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>
        /// Update employee - HR, Manager, and Admin
        /// </summary>
        [HttpPut("{id:int}")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<Employee>>> Update(int id, [FromBody] EmployeeUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<Employee>.Fail("Validation failed", errors));
                }

                var currentUserId = GetCurrentUserId();
                var existingEmployee = await _repo.GetById(id);

                if (existingEmployee == null)
                {
                    return NotFound(ApiResponse<Employee>.Fail($"Employee with ID {id} not found"));
                }

                // Store old values for audit
                var oldEmployee = new
                {
                    existingEmployee.Name,
                    existingEmployee.Email,
                    existingEmployee.DepartmentName,
                    existingEmployee.Role,
                    existingEmployee.Salary
                };

                // Update fields
                existingEmployee.Name = request.Name ?? existingEmployee.Name;
                existingEmployee.Email = request.Email ?? existingEmployee.Email;
                existingEmployee.DepartmentName = request.Department ?? existingEmployee.DepartmentName;
                existingEmployee.Role = request.Role ?? existingEmployee.Role;
                existingEmployee.Salary = request.Salary ?? existingEmployee.Salary;
                existingEmployee.PhoneNumber = request.PhoneNumber ?? existingEmployee.PhoneNumber;
                existingEmployee.Address = request.Address ?? existingEmployee.Address;
                existingEmployee.IsActive = request.IsActive ?? existingEmployee.IsActive;
                existingEmployee.UpdatedBy = currentUserId;
                existingEmployee.UpdatedDate = DateTime.Now;

                await _repo.Update(existingEmployee);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Updated",
                    "Employees",
                    id);

                _logger.LogInformation("Employee {EmployeeId} updated by User {UserId}", id, currentUserId);

                return Ok(ApiResponse<Employee>.Success(existingEmployee, "Employee updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<Employee>.Fail("An error occurred while updating the employee"));
            }
        }

        /// <summary>
        /// Update employee status (activate/deactivate)
        /// </summary>
        [HttpPut("{id:int}/status")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<Employee>>> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var employee = await _repo.GetById(id);

                if (employee == null)
                {
                    return NotFound(ApiResponse<Employee>.Fail($"Employee with ID {id} not found"));
                }

                employee.IsActive = request.IsActive;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedDate = DateTime.Now;

                await _repo.Update(employee);

                var statusText = request.IsActive ? "activated" : "deactivated";

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    $"Employee {statusText}",
                    "Employees",
                    id);

                _logger.LogInformation("Employee {EmployeeId} {Status} by User {UserId}", id, statusText, currentUserId);

                return Ok(ApiResponse<Employee>.Success(employee, $"Employee {statusText} successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee status {EmployeeId}", id);
                return StatusCode(500, ApiResponse<Employee>.Fail("An error occurred while updating the employee status"));
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>
        /// Soft delete employee - Admin only
        /// </summary>
        [HttpDelete("{id:int}")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var employee = await _repo.GetById(id);

                if (employee == null)
                {
                    return NotFound(ApiResponse<bool>.Fail($"Employee with ID {id} not found"));
                }

                // Soft delete
                employee.IsDeleted = true;
                employee.IsActive = false;
                employee.DeletedBy = currentUserId;
                employee.DeletedDate = DateTime.Now;

                await _repo.Update(employee);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Deleted (Soft)",
                    "Employees",
                    id);

                _logger.LogInformation("Employee {EmployeeId} soft deleted by User {UserId}", id, currentUserId);

                return Ok(ApiResponse<bool>.Success(true, "Employee deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while deleting the employee"));
            }
        }

        /// <summary>
        /// Hard delete employee - Admin only (permanent)
        /// </summary>
        [HttpDelete("{id:int}/permanent")]
        [AuthorizePermission("Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> HardDelete(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var employee = await _repo.GetById(id);

                if (employee == null)
                {
                    return NotFound(ApiResponse<bool>.Fail($"Employee with ID {id} not found"));
                }

                await _repo.Delete(id);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Deleted (Permanent)",
                    "Employees",
                    id,
                    employee.Email,
                    null);

                _logger.LogWarning("Employee {EmployeeId} permanently deleted by User {UserId}", id, currentUserId);

                return Ok(ApiResponse<bool>.Success(true, "Employee permanently deleted"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while permanently deleting the employee"));
            }
        }

        /// <summary>
        /// Restore soft-deleted employee - Admin only
        /// </summary>
        [HttpPost("{id:int}/restore")]
        [AuthorizePermission("Admin")]
        public async Task<ActionResult<ApiResponse<Employee>>> Restore(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var employee = await _repo.GetByIdIncludeDeleted(id);

                if (employee == null)
                {
                    return NotFound(ApiResponse<Employee>.Fail($"Employee with ID {id} not found"));
                }

                if (!employee.IsDeleted)
                {
                    return BadRequest(ApiResponse<Employee>.Fail("Employee is not deleted"));
                }

                employee.IsDeleted = false;
                employee.IsActive = true;
                employee.DeletedBy = null;
                employee.DeletedDate = null;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedDate = DateTime.Now;

                await _repo.Update(employee);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Restored",
                    "Employees",
                    id);

                _logger.LogInformation("Employee {EmployeeId} restored by User {UserId}", id, currentUserId);

                return Ok(ApiResponse<Employee>.Success(employee, "Employee restored successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<Employee>.Fail("An error occurred while restoring the employee"));
            }
        }

        #endregion

        #region Export/Import Methods

        /// <summary>
        /// Export employees to Excel - HR and Admin
        /// </summary>
        [HttpGet("export")]
        [AuthorizePermission("Admin")]
        public async Task<IActionResult> Export(
            [FromQuery] string? department = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var employees = await _repo.GetAllFiltered(department, isActive);

                // Generate Excel file using ClosedXML or EPPlus
                var fileBytes = await GenerateExcelFile(employees);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Data Exported",
                    "Employees");

                _logger.LogInformation("Employee data exported by User {UserId}", currentUserId);

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Employees_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employees");
                return StatusCode(500, "An error occurred while exporting employees");
            }
        }

        /// <summary>
        /// Export employees to CSV
        /// </summary>
        [HttpGet("export/csv")]
        [AuthorizePermission("Admin")]
        public async Task<IActionResult> ExportCsv(
            [FromQuery] string? department = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var employees = await _repo.GetAllFiltered(department, isActive);

                var csv = GenerateCsvFile(employees);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Data Exported (CSV)",
                    "Employees");

                return File(
                    System.Text.Encoding.UTF8.GetBytes(csv),
                    "text/csv",
                    $"Employees_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employees to CSV");
                return StatusCode(500, "An error occurred while exporting employees");
            }
        }

        /// <summary>
        /// Import employees from Excel - HR and Admin
        /// </summary>
        [HttpPost("import")]
        [AuthorizePermission("Employee.Import")]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> Import(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<BulkOperationResult>.Fail("No file uploaded"));
                }

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls" && extension != ".csv")
                {
                    return BadRequest(ApiResponse<BulkOperationResult>.Fail("Invalid file format. Please upload Excel or CSV file."));
                }

                var currentUserId = GetCurrentUserId();
                var result = await ProcessImportFile(file, currentUserId);

                // Log audit
                await _auditService.LogAsync(
                    currentUserId,
                    "Employee Data Imported",
                    "Employees",
                    null,
                    null, result.TotalRecords.ToString());

                return Ok(ApiResponse<BulkOperationResult>.Success(result, "Import completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing employees");
                return StatusCode(500, ApiResponse<BulkOperationResult>.Fail("An error occurred while importing employees"));
            }
        }

        #endregion

        #region Photo Upload

        /// <summary>
        /// Upload employee profile photo
        /// </summary>
        [HttpPost("{id:int}/upload-photo")]
        [AuthorizePermission("Employee.Update")]
        public async Task<ActionResult<ApiResponse<string>>> UploadPhoto(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<string>.Fail("No file uploaded"));
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(ApiResponse<string>.Fail("Invalid file type. Allowed: jpg, jpeg, png, gif"));
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(ApiResponse<string>.Fail("File size exceeds 5MB limit"));
                }

                var employee = await _repo.GetById(id);
                if (employee == null)
                {
                    return NotFound(ApiResponse<string>.Fail($"Employee with ID {id} not found"));
                }

                // Generate unique filename
                var fileName = $"{id}_{Guid.NewGuid()}{extension}";
                var uploadPath = Path.Combine("wwwroot", "uploads", "employees");

                // Create directory if not exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Delete old photo if exists
                if (!string.IsNullOrEmpty(employee.ProfileImagePath))
                {
                    var oldFilePath = Path.Combine("wwwroot", employee.ProfileImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new photo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update database
                var relativePath = $"/uploads/employees/{fileName}";
                employee.ProfileImagePath = relativePath;
                employee.UpdatedBy = GetCurrentUserId();
                employee.UpdatedDate = DateTime.Now;

                await _repo.Update(employee);

                _logger.LogInformation("Photo uploaded for employee {EmployeeId}", id);

                return Ok(ApiResponse<string>.Success(relativePath, "Photo uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo for employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<string>.Fail("An error occurred while uploading the photo"));
            }
        }

        /// <summary>
        /// Delete employee profile photo
        /// </summary>
        [HttpDelete("{id:int}/photo")]
        [AuthorizePermission("Employee.Update")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePhoto(int id)
        {
            try
            {
                var employee = await _repo.GetById(id);
                if (employee == null)
                {
                    return NotFound(ApiResponse<bool>.Fail($"Employee with ID {id} not found"));
                }

                if (string.IsNullOrEmpty(employee.ProfileImagePath))
                {
                    return BadRequest(ApiResponse<bool>.Fail("Employee has no profile photo"));
                }

                // Delete file
                var filePath = Path.Combine("wwwroot", employee.ProfileImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Update database
                employee.ProfileImagePath = null;
                employee.UpdatedBy = GetCurrentUserId();
                employee.UpdatedDate = DateTime.Now;

                await _repo.Update(employee);

                return Ok(ApiResponse<bool>.Success(true, "Photo deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo for employee {EmployeeId}", id);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while deleting the photo"));
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<byte[]> GenerateExcelFile(List<Employee> employees)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            // Headers
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Department";
            worksheet.Cell(1, 5).Value = "Role";
            worksheet.Cell(1, 6).Value = "Salary";
            worksheet.Cell(1, 7).Value = "Phone";
            worksheet.Cell(1, 8).Value = "Status";
            worksheet.Cell(1, 9).Value = "Joining Date";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;

            // Data
            var row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cell(row, 1).Value = emp.Id;
                worksheet.Cell(row, 2).Value = emp.Name;
                worksheet.Cell(row, 3).Value = emp.Email;
                worksheet.Cell(row, 4).Value = emp.DepartmentName;
                worksheet.Cell(row, 5).Value = emp.Role;
                worksheet.Cell(row, 6).Value = emp.Salary;
                worksheet.Cell(row, 7).Value = emp.PhoneNumber;
                worksheet.Cell(row, 8).Value = emp.IsActive ? "Active" : "Inactive";
                worksheet.Cell(row, 9).Value = emp.JoiningDate?.ToString("yyyy-MM-dd");
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private string GenerateCsvFile(List<Employee> employees)
        {
            var sb = new System.Text.StringBuilder();

            // Headers
            sb.AppendLine("ID,Name,Email,Department,Role,Salary,Phone,Status,JoiningDate");

            // Data
            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.Id},\"{emp.Name}\",\"{emp.Email}\",\"{emp.DepartmentName}\",\"{emp.Role}\",{emp.Salary},\"{emp.PhoneNumber}\",{(emp.IsActive ? "Active" : "Inactive")},{emp.JoiningDate?.ToString("yyyy-MM-dd")}");
            }

            return sb.ToString();
        }

        private async Task<BulkOperationResult> ProcessImportFile(IFormFile file, int currentUserId)
        {
            var result = new BulkOperationResult();
            var errors = new List<string>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                try
                {
                    var employee = new Employee
                    {
                        Name = row.Cell(1).GetString(),
                        Email = row.Cell(2).GetString(),
                        DepartmentName = row.Cell(3).GetString(),
                        Role = row.Cell(4).GetString(),
                        Salary = row.Cell(5).GetValue<decimal>(),
                        PhoneNumber = row.Cell(6).GetString(),
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    // Check duplicate email
                    var existing = await _repo.GetByEmail(employee.Email);
                    if (existing != null)
                    {
                        errors.Add($"Row {row.RowNumber()}: Email {employee.Email} already exists");
                        result.FailedCount++;
                        continue;
                    }

                    await _repo.Add(employee);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                    result.FailedCount++;
                }
            }

            result.TotalRecords = result.SuccessCount + result.FailedCount;
            result.Errors = errors;

            return result;
        }

        #endregion
    }
}
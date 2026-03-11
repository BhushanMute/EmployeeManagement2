using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IExcelService _excelService;
        private readonly IFileUploadService _fileService;
        private readonly IStudentIdGenerator _idGenerator;
        private readonly ILogger<StudentController> _logger;

        public StudentController( IStudentRepository studentRepo, IExcelService excelService, IFileUploadService fileService, IStudentIdGenerator idGenerator, ILogger<StudentController> logger)
        {
            _studentRepo = studentRepo;
            _excelService = excelService;
            _fileService = fileService;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Get all students
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Student>>>> GetAll()
        {
            try
            {
                var students = await _studentRepo.GetAllAsync();
                return Ok(ApiResponse<List<Student>>.Success(students, "Students retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all students");
                return StatusCode(500, ApiResponse<List<Student>>.Fail("An error occurred while retrieving students"));
            }
        }

        /// <summary>
        /// Get student by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Student>>> GetById(int id)
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(id);

                if (student == null)
                    return NotFound(ApiResponse<Student>.Fail($"Student with ID {id} not found"));

                return Ok(ApiResponse<Student>.Success(student, "Student retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student {Id}", id);
                return StatusCode(500, ApiResponse<Student>.Fail("An error occurred while retrieving the student"));
            }
        }

        /// <summary>
        /// Get students by class
        /// </summary>
        [HttpGet("by-class/{className}")]
        public async Task<ActionResult<ApiResponse<List<Student>>>> GetByClass(string className)
        {
            try
            {
                var students = await _studentRepo.GetByClassAsync(className);
                return Ok(ApiResponse<List<Student>>.Success(students, $"Found {students.Count} students in {className}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students by class {Class}", className);
                return StatusCode(500, ApiResponse<List<Student>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Search students
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<Student>>>> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(ApiResponse<List<Student>>.Fail("Search term is required"));

                var students = await _studentRepo.SearchAsync(term);
                return Ok(ApiResponse<List<Student>>.Success(students, $"Found {students.Count} students"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching students");
                return StatusCode(500, ApiResponse<List<Student>>.Fail("An error occurred while searching"));
            }
        }

        /// <summary>
        /// Upload Excel file and import students
        /// </summary>
        [HttpPost("upload-excel")]
        public async Task<ActionResult<ApiResponse<BulkOperationResult>>> UploadExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(ApiResponse<BulkOperationResult>.Fail("No file uploaded"));

                _logger.LogInformation("Processing Excel file: {FileName}, Size: {Size} bytes", file.FileName, file.Length);

                var uploadedBy = 1; // TODO: Get from authenticated user
                var result = await _excelService.ProcessExcelFileAsync(file, uploadedBy);

                _logger.LogInformation("Processing complete. Success: {Success}, Failed: {Failed}",
                    result.SuccessCount, result.FailedCount);

                if (result.SuccessCount > 0)
                {
                    return Ok(ApiResponse<BulkOperationResult>.Success(
                        result,
                        $"Successfully imported {result.SuccessCount} out of {result.TotalRecords} students"
                    ));
                }
                else
                {
                    return BadRequest(ApiResponse<BulkOperationResult>.Fail(
                        "No students were imported. Check errors.",
                        result.Errors
                    ));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading Excel file");
                return StatusCode(500, ApiResponse<BulkOperationResult>.Fail($"Server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Download Excel template
        /// </summary>
        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var fileBytes = await _excelService.GenerateTemplateAsync();
                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Student_Template.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template");
                return StatusCode(500, "An error occurred while generating the template");
            }
        }

        /// <summary>
        /// Export students to Excel with optional filters
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] string? className = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("Exporting students. Class: {Class}, Status: {Status}, Search: {Search}",
                    className, status, search);

                // Get all students
                var students = await _studentRepo.GetAllAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(className))
                {
                    students = students.Where(s => s.Class.Equals(className, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
                    students = students.Where(s => s.IsActive == isActive).ToList();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    students = students.Where(s =>
                        s.FullName.ToLower().Contains(searchLower) ||
                        s.StudentId.ToLower().Contains(searchLower) ||
                        (s.Email?.ToLower().Contains(searchLower) ?? false) ||
                        (s.PhoneNumber?.Contains(search) ?? false)
                    ).ToList();
                }

                _logger.LogInformation("Exporting {Count} students", students.Count);

                var fileBytes = await _excelService.ExportStudentsToExcelAsync(students);
                var fileName = $"Students_{className}.xlsx";

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting students");
                return StatusCode(500, "An error occurred while exporting students");
            }
        }

        /// <summary>
        /// Delete student
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int studentId)
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(studentId);
                if (student == null)
                    return NotFound(ApiResponse<bool>.Fail($"Student with ID {studentId} not found"));

                var deleted = await _studentRepo.DeleteAsync(studentId);

                if (deleted)
                    return Ok(ApiResponse<bool>.Success(true, "Student deleted successfully"));
                else
                    return StatusCode(500, ApiResponse<bool>.Fail("Failed to delete student"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {Id}", studentId);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while deleting the student"));
            }
        }

        /// <summary>
        /// Get all unique classes (for filter dropdown)
        /// </summary>
        [HttpGet("classes")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetAllClasses()
        {
            try
            {
                var students = await _studentRepo.GetAllAsync();
                var classes = students.Select(s => s.Class).Distinct().OrderBy(c => c).ToList();

                return Ok(ApiResponse<List<string>>.Success(classes, "Classes retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting classes");
                return StatusCode(500, ApiResponse<List<string>>.Fail("An error occurred"));
            }
        }
    }
}
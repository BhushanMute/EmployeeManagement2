using ClosedXML.Excel;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.services
{
    public class ExcelService : IExcelService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IStudentIdGenerator _idGenerator;
        private readonly IFileUploadService _fileService;
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(
            IStudentRepository studentRepo,
            IStudentIdGenerator idGenerator,
            IFileUploadService fileService,
            ILogger<ExcelService> logger)
        {
            _studentRepo = studentRepo;
            _idGenerator = idGenerator;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<BulkOperationResult> ProcessExcelFileAsync(IFormFile file, int uploadedBy)
        {
            var result = new BulkOperationResult();
            var errors = new List<string>();
            var insertedStudents = new List<Student>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    result.Errors.Add("File is empty or null");
                    return result;
                }

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    result.Errors.Add("Invalid file format. Please upload Excel file (.xlsx or .xls)");
                    return result;
                }

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                // ✅ DEBUG: Log all headers
                var headerRow = worksheet.Row(1);
                _logger.LogInformation("=== HEADER ROW ===");
                for (int i = 1; i <= 9; i++)
                {
                    var headerValue = headerRow.Cell(i).GetString();
                    _logger.LogInformation("Column {Col}: '{Value}'", i, headerValue);
                }

                // ✅ Skip header validation for now - just process data
                var allRows = worksheet.RowsUsed().ToList();
                var dataRows = allRows.Skip(1).ToList();
                result.TotalRecords = dataRows.Count;

                _logger.LogInformation("Total data rows to process: {Count}", dataRows.Count);

                foreach (var row in dataRows)
                {
                    try
                    {
                        var rowNumber = row.RowNumber();

                        // ✅ DEBUG: Log raw values from each cell
                        _logger.LogInformation("=== ROW {RowNum} ===", rowNumber);
                        for (int i = 1; i <= 9; i++)
                        {
                            var cellValue = row.Cell(i).GetString();
                            _logger.LogInformation("Cell {Col}: '{Value}'", i, cellValue);
                        }

                        // Extract data
                        var firstName = row.Cell(1).GetString()?.Trim() ?? string.Empty;
                        var lastName = row.Cell(2).GetString()?.Trim() ?? string.Empty;
                        var fullName = row.Cell(3).GetString()?.Trim() ?? string.Empty;
                        var className = row.Cell(4).GetString()?.Trim() ?? string.Empty;
                        var subjects = row.Cell(5).GetString()?.Trim() ?? string.Empty;
                        var ageStr = row.Cell(6).GetString()?.Trim() ?? string.Empty;
                        var joiningDateStr = row.Cell(7).GetString()?.Trim() ?? string.Empty;
                        var batchTime = row.Cell(8).GetString()?.Trim() ?? string.Empty;
                        var passportPhotoData = row.Cell(9).GetString()?.Trim() ?? string.Empty;

                        _logger.LogInformation("Parsed - FirstName: '{FN}', LastName: '{LN}', Class: '{Class}'",
                            firstName, lastName, className);

                        // Validation
                        if (string.IsNullOrWhiteSpace(firstName))
                        {
                            errors.Add($"Row {rowNumber}: First Name is required (found empty)");
                            result.FailedCount++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(lastName))
                        {
                            errors.Add($"Row {rowNumber}: Last Name is required (found empty)");
                            result.FailedCount++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(className))
                        {
                            errors.Add($"Row {rowNumber}: Class is required (found empty)");
                            result.FailedCount++;
                            continue;
                        }

                        // Parse age
                        int? age = null;
                        if (!string.IsNullOrWhiteSpace(ageStr))
                        {
                            if (int.TryParse(ageStr, out var ageValue))
                            {
                                age = ageValue;
                            }
                        }

                        // Parse joining date
                        DateTime joiningDate = DateTime.Now;
                        if (!string.IsNullOrWhiteSpace(joiningDateStr))
                        {
                            DateTime.TryParse(joiningDateStr, out joiningDate);
                        }

                        // Generate student ID
                        var studentId = await _idGenerator.GenerateNextIdAsync();
                        _logger.LogInformation("Generated StudentId: {StudentId}", studentId);

                        // Create student object
                        var student = new Student
                        {
                            StudentId = studentId,
                            FirstName = firstName,
                            LastName = lastName,
                            FullName = string.IsNullOrWhiteSpace(fullName) ? $"{firstName} {lastName}" : fullName,
                            Class = className,
                            Subjects = subjects,
                            Age = age,
                            JoiningDate = joiningDate,
                            BatchTime = batchTime,
                            PassportPhotoPath = null, // Skip photo for now
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = uploadedBy,
                            CreatedDate = DateTime.Now
                        };

                        _logger.LogInformation("Attempting database insert for: {StudentId}", studentId);

                        // Insert into database
                        var newId = await _studentRepo.AddAsync(student);
                        student.Id = newId;

                        _logger.LogInformation("✓ Inserted successfully with ID: {Id}", newId);

                        insertedStudents.Add(student);
                        result.SuccessCount++;
                    }
                    catch (Exception rowEx)
                    {
                        var rowNumber = row.RowNumber();
                        _logger.LogError(rowEx, "ERROR processing row {RowNumber}: {Message}", rowNumber, rowEx.Message);
                        errors.Add($"Row {rowNumber}: {rowEx.Message}");
                        result.FailedCount++;
                    }
                }

                result.Errors = errors;
                result.InsertedStudents = insertedStudents;
                result.Message = $"Processed {result.TotalRecords} records: {result.SuccessCount} succeeded, {result.FailedCount} failed";

                // ✅ Log all errors for debugging
                if (errors.Any())
                {
                    _logger.LogWarning("=== ERRORS ===");
                    foreach (var error in errors)
                    {
                        _logger.LogWarning(error);
                    }
                }

                _logger.LogInformation("Excel processing completed: {SuccessCount} succeeded, {FailedCount} failed",
                    result.SuccessCount, result.FailedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error processing Excel file: {Message}", ex.Message);
                result.Errors.Add($"Error: {ex.Message}");
            }

            return result;
        }
        public async Task<byte[]> ExportStudentsToExcelAsync(List<Student> students)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");

            // ===== HEADERS =====
            var headers = new[]
            {
        "Student ID", "First Name", "Last Name", "Full Name", "Class",
        "Subjects", "Age", "Joining Date", "Batch Time", "Phone",
        "Email", "Status", "Created Date"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // ===== HEADER STYLE =====
            var headerRange = worksheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontSize = 12;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // ===== ADD FILTER =====
            worksheet.Range(1, 1, 1, headers.Length).SetAutoFilter();

            // ===== DATA ROWS =====
            var row = 2;

            foreach (var student in students)
            {
                worksheet.Cell(row, 1).Value = student.StudentId;
                worksheet.Cell(row, 2).Value = student.FirstName;
                worksheet.Cell(row, 3).Value = student.LastName;
                worksheet.Cell(row, 4).Value = student.FullName;
                worksheet.Cell(row, 5).Value = student.Class;
                worksheet.Cell(row, 6).Value = student.Subjects ?? "";
                worksheet.Cell(row, 7).Value = student.Age ?? 0;
                worksheet.Cell(row, 8).Value = student.JoiningDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 9).Value = student.BatchTime ?? "";
                worksheet.Cell(row, 10).Value = student.PhoneNumber ?? "";
                worksheet.Cell(row, 11).Value = student.Email ?? "";
                worksheet.Cell(row, 12).Value = student.IsActive ? "Active" : "Inactive";
                worksheet.Cell(row, 13).Value = student.CreatedDate.ToString("yyyy-MM-dd HH:mm");

                // ===== CENTER ALIGN IMPORTANT COLUMNS =====
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // StudentId
                worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Class
                worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Age
                worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // JoiningDate
                worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // BatchTime
                worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Status

                // ===== STATUS COLOR =====
                var statusCell = worksheet.Cell(row, 12);

                if (student.IsActive)
                {
                    statusCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                }
                else
                {
                    statusCell.Style.Fill.BackgroundColor = XLColor.LightPink;
                }

                // ===== ALTERNATE ROW COLOR =====
                if (row % 2 == 0)
                {
                    worksheet.Range(row, 1, row, headers.Length)
                        .Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                }

                row++;
            }

            // ===== TABLE BORDER =====
            var dataRange = worksheet.Range(1, 1, row - 1, headers.Length);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // ===== SUMMARY =====
            row += 2;

            worksheet.Cell(row, 1).Value = "Summary";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;

            row++;
            worksheet.Cell(row, 1).Value = "Total Students:";
            worksheet.Cell(row, 2).Value = students.Count;

            row++;
            worksheet.Cell(row, 1).Value = "Active:";
            worksheet.Cell(row, 2).Value = students.Count(s => s.IsActive);

            row++;
            worksheet.Cell(row, 1).Value = "Inactive:";
            worksheet.Cell(row, 2).Value = students.Count(s => !s.IsActive);

            row++;
            worksheet.Cell(row, 1).Value = "Export Date:";
            worksheet.Cell(row, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var summaryRange = worksheet.Range(row - 4, 1, row, 2);
            summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            summaryRange.Style.Fill.BackgroundColor = XLColor.LightYellow;

            // ===== AUTO FIT =====
            worksheet.Columns().AdjustToContents();

            // ===== FREEZE HEADER =====
            worksheet.SheetView.FreezeRows(1);

            // ===== SAVE =====
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return await Task.FromResult(stream.ToArray());
        }

        public async Task<byte[]> GenerateTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Student Template");

            // Headers
            worksheet.Cell(1, 1).Value = "First_Name";
            worksheet.Cell(1, 2).Value = "Last_Name";
            worksheet.Cell(1, 3).Value = "Full_Name";
            worksheet.Cell(1, 4).Value = "Class";
            worksheet.Cell(1, 5).Value = "Subjects";
            worksheet.Cell(1, 6).Value = "Age";
            worksheet.Cell(1, 7).Value = "Joining_Date";
            worksheet.Cell(1, 8).Value = "Batch_Time";
            worksheet.Cell(1, 9).Value = "Passport_Photo";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Green;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Sample data
            worksheet.Cell(2, 1).Value = "John";
            worksheet.Cell(2, 2).Value = "Doe";
            worksheet.Cell(2, 3).Value = "John Doe";
            worksheet.Cell(2, 4).Value = "Class 1";
            worksheet.Cell(2, 5).Value = "Math, Science, English";
            worksheet.Cell(2, 6).Value = 10;
            worksheet.Cell(2, 7).Value = "2024-01-15";
            worksheet.Cell(2, 8).Value = "Morning";
            worksheet.Cell(2, 9).Value = "Leave blank or provide base64/path";

            // Auto-fit
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }

        private bool ValidateHeaders(IXLRow headerRow)
        {
            var expectedHeaders = new[]
            {
                "First_Name", "Last_Name", "Full_Name", "Class", "Subjects",
                "Age", "Joining_Date", "Batch_Time", "Passport_Photo"
            };

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                var cellValue = headerRow.Cell(i + 1).GetString().Trim();
                if (!cellValue.Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Header mismatch at column {Column}: Expected '{Expected}', Got '{Actual}'",
                        i + 1, expectedHeaders[i], cellValue);
                    return false;
                }
            }

            return true;
        }

        private bool IsBase64String(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();
            return (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", System.Text.RegularExpressions.RegexOptions.None);
        }
        // ✅ Helper method to safely get cell values
        private string GetCellValue(IXLRow row, int columnNumber)
        {
            try
            {
                var cell = row.Cell(columnNumber);
                if (cell == null || cell.IsEmpty())
                    return string.Empty;

                return cell.GetValue<string>().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}

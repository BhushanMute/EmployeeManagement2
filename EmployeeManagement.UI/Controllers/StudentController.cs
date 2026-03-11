using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IHttpClientFactory factory, ILogger<StudentController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        // GET: Student/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("api/Student");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<StudentViewModel>>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var students = apiResponse?.Data ?? new List<StudentViewModel>();
                    return View(students.OrderBy(s => s.Class).ThenBy(s => s.FullName).ToList());
                }
                else
                {
                    TempData["Error"] = "Failed to load students";
                    return View(new List<StudentViewModel>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students");
                TempData["Error"] = "An error occurred while loading students";
                return View(new List<StudentViewModel>());
            }
        }

        // GET: Student/UploadExcel
        public IActionResult UploadExcel()
        {
            return View();
        }

        // POST: Student/UploadExcel
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to upload";
                    return View();
                }

                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await _client.PostAsync("api/Student/upload-excel", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<BulkOperationResult>>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse?.Data != null)
                    {
                        TempData["Success"] = apiResponse.Message;
                        TempData["UploadResult"] = JsonSerializer.Serialize(apiResponse.Data);
                        return RedirectToAction("UploadedRecords");
                    }
                }

                // Try to get error message
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    TempData["Error"] = errorResponse?.Message ?? "Failed to upload Excel file";
                }
                catch
                {
                    TempData["Error"] = "Failed to upload Excel file";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading Excel file");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        // GET: Student/UploadedRecords
        public IActionResult UploadedRecords()
        {
            try
            {
                var uploadResultJson = TempData["UploadResult"] as string;

                if (string.IsNullOrEmpty(uploadResultJson))
                {
                    return RedirectToAction("Index");
                }

                var uploadResult = JsonSerializer.Deserialize<BulkOperationResult>(uploadResultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.SuccessCount = uploadResult?.SuccessCount ?? 0;
                ViewBag.FailedCount = uploadResult?.FailedCount ?? 0;
                ViewBag.TotalRecords = uploadResult?.TotalRecords ?? 0;
                ViewBag.Errors = uploadResult?.Errors ?? new List<string>();

                var students = uploadResult?.InsertedStudents?.Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    FullName = s.FullName,
                    Class = s.Class,
                    Subjects = s.Subjects,
                    Age = s.Age,
                    JoiningDate = s.JoiningDate,
                    BatchTime = s.BatchTime,
                    PassportPhotoPath = s.PassportPhotoPath,
                    PhoneNumber = s.PhoneNumber,
                    Email = s.Email
                }).ToList() ?? new List<StudentViewModel>();

                return View(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying uploaded records");
                return RedirectToAction("Index");
            }
        }

        // GET: Student/DownloadTemplate
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var response = await _client.GetAsync("api/Student/download-template");

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Student_Template.xlsx");
                }

                TempData["Error"] = "Failed to download template";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading template");
                TempData["Error"] = "An error occurred while downloading the template";
                return RedirectToAction("Index");
            }
        }

        // GET: Student/ExportToExcel

        //public async Task<IActionResult> ExportToExcel(string? className = null, string? status = null, string? search = null)
        //{
        //    try
        //    {
        //        // Build query string for filtered export
        //        var queryParams = new List<string>();

        //        if (!string.IsNullOrEmpty(className))
        //            queryParams.Add($"className={Uri.EscapeDataString(className)}");

        //        if (!string.IsNullOrEmpty(status))
        //            queryParams.Add($"status={Uri.EscapeDataString(status)}");

        //        if (!string.IsNullOrEmpty(search))
        //            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        //        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        //        var url = $"api/Student/export{queryString}";

        //        _logger.LogInformation("Exporting students with URL: {Url}", url);

        //        var response = await _client.GetAsync(url);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var fileBytes = await response.Content.ReadAsByteArrayAsync();

        //            var classPart = string.IsNullOrEmpty(className) ? "All" : className;
        //            var fileName = $"Student_{classPart}.xlsx";
        //            return File(fileBytes,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                fileName);
        //        }

        //        TempData["Error"] = "Failed to export students";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error exporting students");
        //        TempData["Error"] = "An error occurred while exporting students";
        //        return RedirectToAction("Index");
        //    }
        //}

        public async Task<IActionResult> ExportToExcel(string? className = null, string? status = null, string? search = null)
        {
            try
            {
                // Build query string for filtered export
                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(className))
                    queryParams.Add($"className={Uri.EscapeDataString(className)}");

                if (!string.IsNullOrWhiteSpace(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                if (!string.IsNullOrWhiteSpace(search))
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");

                // If no filters -> export ALL records
                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                var url = $"api/Student/export{queryString}";

                _logger.LogInformation("Exporting students using URL: {Url}", url);

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to export students";
                    return RedirectToAction("Index");
                }

                var fileBytes = await response.Content.ReadAsByteArrayAsync();

                // Build file name
                string classPart = string.IsNullOrEmpty(className) ? "All" : className;
                string fileName = $"Student_{classPart}_{DateTime.Now:yyyyMMdd}.xlsx";

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting students");
                TempData["Error"] = "An error occurred while exporting students";
                return RedirectToAction("Index");
            }
        }

        // POST: Student/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(string studentId)
        {
            try
            {
                //var response = await _client.DeleteAsync($"api/Student/Delete/{id}");
               var r =   studentId.Substring(4,4);
                var response = await _client.DeleteAsync($"api/Student/Delete/{r}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Student deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete student";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {Id}", studentId);
                TempData["Error"] = "An error occurred while deleting the student";
            }

            return RedirectToAction("Index");
        }

        // GET: Student/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _client.GetAsync($"api/Student/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<StudentViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse?.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }

                TempData["Error"] = "Student not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student details");
                TempData["Error"] = "An error occurred";
                return RedirectToAction("Index");
            }
        }
    }

    [HttpPost]
        public async Task<IActionResult> UploadStudentPhoto(IFormFile file, int studentId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "No file selected" });
                }

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size must be less than 5MB" });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return Json(new { success = false, message = "Only JPG, PNG, and GIF files are allowed" });
                }

                // Create uploads folder if not exists
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "students");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"student_{studentId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Delete old photo if exists
                var student = await _studentService.GetStudentByIdAsync(studentId);
                if (student != null && !string.IsNullOrEmpty(student.PassportPhotoPath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, student.PassportPhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update database
                var imageUrl = $"/uploads/students/{fileName}";
                await _studentService.UpdateStudentPhotoAsync(studentId, imageUrl);

                return Json(new { success = true, imageUrl = imageUrl, message = "Photo uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading photo: " + ex.Message });
            }
        }

        // POST: Student/RemoveStudentPhoto
        [HttpPost]
        public async Task<IActionResult> RemoveStudentPhoto(int studentId)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(studentId);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found" });
                }

                // Delete file if exists
                if (!string.IsNullOrEmpty(student.PassportPhotoPath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, student.PassportPhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Update database
                await _studentService.UpdateStudentPhotoAsync(studentId, null);

                return Json(new { success = true, message = "Photo removed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error removing photo: " + ex.Message });
            }
        }
        // Helper classes (if not already defined elsewhere)

    }
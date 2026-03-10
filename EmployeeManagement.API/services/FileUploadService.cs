namespace EmployeeManagement.API.services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _studentPhotosPath;
        private readonly long _maxFileSizeBytes;
        private readonly string[] _allowedExtensions;

        public FileUploadService(IConfiguration config, ILogger<FileUploadService> logger)
        {
            _config = config;
            _logger = logger;

            _studentPhotosPath = config["FileUpload:StudentPhotosPath"] ?? "wwwroot/uploads/students";
            _maxFileSizeBytes = config.GetValue<int>("FileUpload:MaxFileSizeMB", 10) * 1024 * 1024;
            _allowedExtensions = config.GetSection("FileUpload:AllowedImageExtensions").Get<string[]>()
                ?? new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Create directory if not exists
            if (!Directory.Exists(_studentPhotosPath))
            {
                Directory.CreateDirectory(_studentPhotosPath);
            }
        }

        public async Task<string> SaveStudentPhotoAsync(IFormFile file, string studentId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");

                if (!IsValidImageFile(file))
                    throw new ArgumentException("Invalid file type");

                if (file.Length > _maxFileSizeBytes)
                    throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSizeBytes / 1024 / 1024}MB");

                var extension = Path.GetExtension(file.FileName).ToLower();
                var fileName = $"{studentId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_studentPhotosPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Saved photo for student {StudentId}: {FileName}", studentId, fileName);

                // Return relative path
                return $"/uploads/students/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving student photo for {StudentId}", studentId);
                throw;
            }
        }

        public async Task<string> SaveBase64ImageAsync(string base64String, string studentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64String))
                    throw new ArgumentException("Base64 string is empty");

                // Remove data:image prefix if exists
                var base64Data = base64String.Contains(",")
                    ? base64String.Split(',')[1]
                    : base64String;

                var imageBytes = Convert.FromBase64String(base64Data);

                // Determine extension from base64 prefix
                var extension = ".jpg";
                if (base64String.StartsWith("data:image/png"))
                    extension = ".png";
                else if (base64String.StartsWith("data:image/gif"))
                    extension = ".gif";

                var fileName = $"{studentId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_studentPhotosPath, fileName);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                _logger.LogInformation("Saved base64 image for student {StudentId}", studentId);

                return $"/uploads/students/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving base64 image for {StudentId}", studentId);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                var fullPath = Path.Combine("wwwroot", filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Deleted file: {FilePath}", filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLower();
            return _allowedExtensions.Contains(extension);
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).ToLower();
        }
    }
}

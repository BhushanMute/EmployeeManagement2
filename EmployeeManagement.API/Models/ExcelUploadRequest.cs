namespace EmployeeManagement.API.Models
{
    public class ExcelUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public bool OverwriteExisting { get; set; } = false;
        public bool ValidateOnly { get; set; } = false;
    }
}

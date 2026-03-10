namespace EmployeeManagement.API.services
{
    public interface IFileUploadService
    {
        Task<string> SaveStudentPhotoAsync(IFormFile file, string studentId);
        Task<string> SaveBase64ImageAsync(string base64String, string studentId);
        Task<bool> DeleteFileAsync(string filePath);
        bool IsValidImageFile(IFormFile file);
        string GetFileExtension(string fileName);
    }
}

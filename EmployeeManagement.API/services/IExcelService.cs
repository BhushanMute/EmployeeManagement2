using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.services
{
    public interface IExcelService
    {
        Task<BulkOperationResult> ProcessExcelFileAsync(IFormFile file, int uploadedBy);
        Task<byte[]> ExportStudentsToExcelAsync(List<Student> students);
        Task<byte[]> GenerateTemplateAsync();
    }
}

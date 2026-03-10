using EmployeeManagement.API.Models;

namespace EmployeeManagement.UI.Models
{
    public class BulkOperationResult
    {
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<Student> InsertedStudents { get; set; } = new();
    }
}

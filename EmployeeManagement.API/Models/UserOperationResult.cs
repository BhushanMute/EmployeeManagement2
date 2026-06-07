namespace EmployeeManagement.API.Models
{
    public class UserOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int? NewId { get; set; }
    }
}

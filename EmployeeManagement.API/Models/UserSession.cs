namespace EmployeeManagement.API.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public string SessionToken { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? IpAddress { get; set; }
        public bool IsActive { get; set; }
    }
}

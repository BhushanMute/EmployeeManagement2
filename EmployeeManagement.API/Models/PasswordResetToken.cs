namespace EmployeeManagement.API.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // From joined User table
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public bool IsValid => !IsUsed && ExpiryDate > DateTime.UtcNow;
    }
}

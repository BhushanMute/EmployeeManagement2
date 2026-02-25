namespace EmployeeManagement.API.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}

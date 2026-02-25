namespace EmployeeManagement.UI.Models
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }
}

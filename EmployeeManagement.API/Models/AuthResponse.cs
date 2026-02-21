namespace EmployeeManagement.API.Models
{
    public class AuthResponse
    {
        public string? Token { get; set; }  
        public string Message { get; set; } = string.Empty;
        public UserModel User { get; set; }
        public string RefreshToken { get; set; }
 
    }
}

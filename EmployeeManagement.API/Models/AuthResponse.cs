namespace EmployeeManagement.API.Models
{
    public class AuthResponse
    {
        public string? Token { get; set; }  
        public string Message { get; set; } = string.Empty; 
    }
}

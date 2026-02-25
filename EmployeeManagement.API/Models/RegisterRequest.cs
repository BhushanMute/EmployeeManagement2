using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public int RoleId { get; set; } = 4; // Default: Employee
    }
}
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public int RoleId { get; set; } = 4;
    }
}
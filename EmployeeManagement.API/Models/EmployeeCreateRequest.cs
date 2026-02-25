using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class EmployeeCreateRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? JoiningDate { get; set; }
    }
}

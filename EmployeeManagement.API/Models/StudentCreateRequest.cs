using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class StudentCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Class { get; set; } = string.Empty;

        public string? Subjects { get; set; }

        public int? Age { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string? BatchTime { get; set; }

        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? ParentName { get; set; }

        public string? ParentPhone { get; set; }

        [EmailAddress]
        public string? ParentEmail { get; set; }

        // Base64 encoded image or file path
        public string? PassportPhotoBase64 { get; set; }
    }
}

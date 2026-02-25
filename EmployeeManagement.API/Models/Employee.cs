using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.API.Models
{
    public class Employee
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Email { get; set; }
        //public int DepartmentId { get; set; }
        //public string DepartmentName { get; set; }
        //public decimal Salary { get; set; }
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string? ProfileImagePath { get; set; }

        public string? Role { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}

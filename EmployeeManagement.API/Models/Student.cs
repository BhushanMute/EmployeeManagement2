namespace EmployeeManagement.API.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string? Subjects { get; set; }
        public int? Age { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? BatchTime { get; set; }
        public string? BatchCode { get; set; }
        public string? PassportPhotoPath { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Audit
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}

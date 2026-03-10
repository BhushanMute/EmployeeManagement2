namespace EmployeeManagement.UI.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string? Subjects { get; set; }
        public int? Age { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? BatchTime { get; set; }
        public string? PassportPhotoPath { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter employee name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        public int? DepartmentId { get; set; }  // user selects only this

        public string DepartmentName { get; set; }  // optional, set in API if needed

        [Required(ErrorMessage = "Please enter salary")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        // For dropdown binding in view
        public List<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    }
}
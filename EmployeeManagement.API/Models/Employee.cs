using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }      // for posting to API
        public string DepartmentName { get; set; } // for display
        public decimal Salary { get; set; }
    }
}

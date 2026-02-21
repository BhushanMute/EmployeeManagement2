using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public decimal Salary { get; set; }
    }
}

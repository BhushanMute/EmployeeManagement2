using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Department { get; set; }

        [Range(1000, 1000000)]
        public decimal Salary { get; set; }
    }
}

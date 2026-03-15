namespace EmployeeManagement.API.Models
{
    public class EmployeeSalaryItem
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public string? Role { get; set; }
        public decimal MonthlySalary { get; set; }
        public decimal DailySalary { get; set; }
        public int WorkingDays { get; set; }
        public decimal LeaveDays { get; set; }
        public decimal UnpaidLeaveDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal NetSalary { get; set; }
        public decimal DeductionAmount { get; set; }
        public DateTime? JoiningDate { get; set; }
    }
}

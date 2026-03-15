namespace EmployeeManagement.API.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DefaultDays { get; set; }
        public int MaxDays { get; set; }
        public bool IsCarryForward { get; set; }
        public int MaxCarryForward { get; set; }
        public bool IsPaid { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

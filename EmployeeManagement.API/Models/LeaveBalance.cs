namespace EmployeeManagement.API.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalPending { get; set; }
        public decimal CarryForward { get; set; }
        public decimal TotalAvailable { get; set; }  // Computed
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation
        public string? LeaveTypeName { get; set; }
        public string? LeaveTypeCode { get; set; }
        public bool IsPaid { get; set; }
        public string? EmployeeName { get; set; }
    }
}

namespace EmployeeManagement.API.Models
{
    public class LeaveApproval
    {
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public int ApproverLevel { get; set; }
        public int ApproverId { get; set; }
        public string ApproverRole { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? Comments { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ApproveReimbursementRequest
    {
        [Required]
        public int ReimbursementId { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public decimal? ApprovedAmount { get; set; }
        public string? Remarks { get; set; }
    }
}

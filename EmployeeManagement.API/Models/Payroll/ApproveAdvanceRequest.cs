using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ApproveAdvanceRequest
    {
        [Required]
        public int AdvanceId { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public decimal? ApprovedAmount { get; set; }
        public string? Remarks { get; set; }
    }
}

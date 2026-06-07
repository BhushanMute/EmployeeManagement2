namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollProcessingDetail
    {
        public int Id { get; set; }
        public int PayrollProcessingId { get; set; }
        public int ComponentId { get; set; }

        // Component Info (denormalized)
        public string ComponentCode { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty;

        // Calculation Details
        public string CalculationType { get; set; } = string.Empty;
        public string? CalculationBase { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? BaseAmount { get; set; }

        // Final Calculated Amount
        public decimal Amount { get; set; }

        // Attendance Impact
        public bool IsAttendanceBased { get; set; }
        public bool AdjustedForLOP { get; set; }
        public decimal? OriginalAmount { get; set; }

        // Display
        public int DisplayOrder { get; set; }

        // Metadata
        public string? Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation Property
        public virtual PayrollProcessing? PayrollProcessing { get; set; }
    }
}

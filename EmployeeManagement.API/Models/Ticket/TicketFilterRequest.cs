namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketFilterRequest
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? TicketType { get; set; }
        public int? AssignedTo { get; set; }
        public int? CreatedBy { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

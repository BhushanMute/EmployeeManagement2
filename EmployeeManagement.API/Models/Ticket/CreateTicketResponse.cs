namespace EmployeeManagement.API.Models.Ticket
{
    public class CreateTicketResponse
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

namespace EmployeeManagement.API.Models.Ticket
{
    public class MyTicketsRequest
    {
        public int UserId { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string? Status { get; set; }
    }
}

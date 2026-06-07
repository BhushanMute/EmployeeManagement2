namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketEmailRecipient
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string NotificationType { get; set; } = ""; // Creator, Assignee, Watcher, QA Team
    }
}

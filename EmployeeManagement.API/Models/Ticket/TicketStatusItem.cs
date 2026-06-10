namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketStatusItem : TicketMasterItem
    {
        public bool IsDefault { get; set; }
        public bool IsFinalStatus { get; set; }
    }
}

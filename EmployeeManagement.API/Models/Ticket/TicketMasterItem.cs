namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketMasterItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string? ColorCode { get; set; }
        public int SortOrder { get; set; }
    }
}

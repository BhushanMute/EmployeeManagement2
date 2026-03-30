namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketReport
    {
        public TicketDashboard Stats { get; set; } = new();
        public List<PriorityDistribution> ByPriority { get; set; } = new();
        public List<TypeDistribution> ByType { get; set; } = new();
        public List<DeveloperWorkload> DeveloperWorkloads { get; set; } = new();
        public DateTime ReportGeneratedAt { get; set; } = DateTime.Now;
    }
}

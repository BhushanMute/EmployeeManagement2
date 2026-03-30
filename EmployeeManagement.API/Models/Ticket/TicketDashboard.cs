namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketDashboard
    {
        public int TotalTickets { get; set; }
        public int NewTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int BlockedTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int ReopenedTickets { get; set; }
        public int CriticalTickets { get; set; }
        public int HighPriorityTickets { get; set; }
        public int OverdueTickets { get; set; }

        // Computed
        public int OpenTickets => TotalTickets - ClosedTickets;
        public double ClosedPercentage => TotalTickets > 0
            ? Math.Round((double)ClosedTickets / TotalTickets * 100, 1)
            : 0;
        public double ResolvedPercentage => TotalTickets > 0
            ? Math.Round((double)(ResolvedTickets + ClosedTickets) / TotalTickets * 100, 1)
            : 0;
        public TicketDashboard Stats { get; set; } = new();
        public List<PriorityDistribution> PriorityDistribution { get; set; } = new();
        public List<TypeDistribution> TypeDistribution { get; set; } = new();
        public List<RecentTicketActivity> RecentActivities { get; set; } = new();
    }

}

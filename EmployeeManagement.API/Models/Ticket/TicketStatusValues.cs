namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketStatusValues
    {
        public const string New = "New";
        public const string Assigned = "Assigned";
        public const string InProgress = "In Progress";
        public const string Blocked = "Blocked";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";
        public const string Reopened = "Reopened";

        public static readonly string[] AllStatuses = new[]
        {
            New, Assigned, InProgress, Blocked, Resolved, Closed, Reopened
        };
    }
}

namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketPriorityValues
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";
        public const string Critical = "Critical";

        public static readonly string[] AllPriorities = new[]
        {
            Low, Medium, High, Critical
        };
    }
}

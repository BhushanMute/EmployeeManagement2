namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketTypeValues
    {
        public const string Bug = "Bug";
        public const string FeatureRequest = "Feature Request";
        public const string Improvement = "Improvement";
        public const string Task = "Task";

        public static readonly string[] AllTypes = new[]
        {
            Bug, FeatureRequest, Improvement, Task
        };
    }
}

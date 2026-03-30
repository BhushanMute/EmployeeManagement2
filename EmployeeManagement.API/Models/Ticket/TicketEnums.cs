namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketEnums
    {
        public enum TicketStatus
        {
            New,
            Assigned,
            InProgress,
            Blocked,
            Resolved,
            Closed,
            Reopened
        }
        public enum TicketPriority
        {
            Low,
            Medium,
            High,
            Critical
        }
        public enum TicketType
        {
            Bug,
            FeatureRequest,
            Improvement,
            Task
        }
        public enum TicketUserRole
        {
            QA,
            Developer,
            Admin,
            Lead
        }
        public enum TicketChangeType
        {
            Created,
            Updated,
            StatusChanged,
            Assigned,
            Reassigned,
            CommentAdded,
            AttachmentAdded,
            AttachmentDeleted,
            Deleted
        }
    }
}

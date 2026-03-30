namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static TicketOperationResult SuccessResult(string message = "Operation completed successfully")
        {
            return new TicketOperationResult { Success = true, Message = message };
        }

        public static TicketOperationResult FailureResult(string message = "Operation failed")
        {
            return new TicketOperationResult { Success = false, Message = message };
        }
    }
}

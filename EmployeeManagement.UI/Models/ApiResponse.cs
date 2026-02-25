using System.Text.Json.Serialization;


namespace EmployeeManagement.UI.Models
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }


        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Status = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Status = false,
                Message = message,
                Errors = errors
            };
        }
    }
     
}
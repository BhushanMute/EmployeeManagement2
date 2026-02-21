namespace EmployeeManagement.API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Check if token is active (not revoked and not expired)
        /// </summary>
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiryDate;

        /// <summary>
        /// Check if token is expired
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    }
}

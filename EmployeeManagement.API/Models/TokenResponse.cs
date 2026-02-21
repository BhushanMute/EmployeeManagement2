namespace EmployeeManagement.API.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string Message { get; set; }

        // Backward compatibility property
        public DateTime Expiration 
        { 
            get => AccessTokenExpiration;
            set => AccessTokenExpiration = value;
        }
    }
}

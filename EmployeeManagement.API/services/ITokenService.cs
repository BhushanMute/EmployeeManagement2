namespace EmployeeManagement.API.services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        bool IsTokenExpired(string token);
        Task<string> RefreshTokenAsync();
        Task SetRefreshTokenAsync(string refreshToken);
    }
}


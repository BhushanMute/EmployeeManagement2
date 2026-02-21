using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<int> SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token, string replacedByToken = null);
        Task RevokeAllUserTokensAsync(int userId);
    }
}

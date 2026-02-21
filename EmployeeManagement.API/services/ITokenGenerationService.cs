using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.services
{
    public interface ITokenGenerationService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiration();
        DateTime GetRefreshTokenExpiration();
    }
}

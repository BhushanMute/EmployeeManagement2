using EmployeeManagement.API.Models;
using System.Security.Claims;

namespace EmployeeManagement.API.Repositories
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, List<Role> roles, List<Permission> permissions);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        DateTime GetAccessTokenExpiry();
        DateTime GetRefreshTokenExpiry();
    }
}

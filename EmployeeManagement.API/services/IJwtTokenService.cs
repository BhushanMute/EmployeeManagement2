using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.services
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserModel user, List<string> roles);
    }
}

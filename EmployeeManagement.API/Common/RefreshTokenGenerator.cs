namespace EmployeeManagement.API.Common
{
    public class RefreshTokenGenerator
    {
        public static string Generate()
        {
            return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        }
    }
}

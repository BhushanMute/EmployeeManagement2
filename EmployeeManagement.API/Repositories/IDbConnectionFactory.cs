using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

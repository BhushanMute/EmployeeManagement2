using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
 

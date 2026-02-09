using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Common
{
    public class DbHelper
    {
        private readonly string _connectionString;
 
        // ✅ THIS constructor MUST exist
        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            var con = new SqlConnection(_connectionString);
            con.Open();
            return con;
        }
    }
}

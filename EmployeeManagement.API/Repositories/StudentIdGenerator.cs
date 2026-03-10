using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class StudentIdGenerator : IStudentIdGenerator
    {
        private readonly string _connectionString;
        private readonly ILogger<StudentIdGenerator> _logger;

        public StudentIdGenerator(IConfiguration config, ILogger<StudentIdGenerator> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found");
            _logger = logger;
        }

        public async Task<string> GenerateNextIdAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GenerateNextStudentId", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                var outputParam = new SqlParameter("@NewStudentId", SqlDbType.NVarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                var newId = outputParam.Value?.ToString() ?? "CSM_0001";
                _logger.LogInformation("Generated new Student ID: {StudentId}", newId);

                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating student ID");
                throw;
            }
        }
    }
}

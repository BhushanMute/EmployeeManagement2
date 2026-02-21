using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        //public async Task<List<Employee>> GetAll()
        //{
        //    var employees = new List<Employee>();

        //    using var conn = GetConnection();
        //    await conn.OpenAsync();

        //    using var cmd = new SqlCommand("sp_GetAllEmployees", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    using var reader = await cmd.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //    {
        //        employees.Add(new Employee
        //        {
        //            Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //            Name = reader.GetString(reader.GetOrdinal("Name")),
        //            Email = reader.GetString(reader.GetOrdinal("Email")),

        //            DepartmentName = reader["DepartmentName"].ToString(),
        //            Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
        //        });
        //    }

        //    return employees;
        //}

        public async Task<List<Employee>> GetAll()
        {
            var employees = new List<Employee>();

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    DepartmentName = reader["DepartmentName"].ToString(),
                    Salary = reader.GetDecimal("Salary")
                });
            }

            return employees;
        }

        public async Task<Employee?> GetById(int id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetEmployeeById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentName = reader["DepartmentName"].ToString(),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
            };
        }
            
        public async Task Add(Employee emp)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sp_AddEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
                cmd.Parameters.AddWithValue("@Salary", emp.Salary);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {

                throw new Exception("Email already exists. Please use different email.");

            }

        }

        
        public async Task Update(Employee emp)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_UpdateEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", emp.Id);
            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@Email", emp.Email);
            cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
            cmd.Parameters.AddWithValue("@Salary", emp.Salary);

            await cmd.ExecuteNonQueryAsync();
        }
 
        public async Task Delete(int id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_DeleteEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
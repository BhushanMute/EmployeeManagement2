using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class AuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found.");
        }

        public async Task Register(RegisterRequest model)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_RegisterUser", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Username", model.Username);
            cmd.Parameters.AddWithValue(
                "@PasswordHash",
                BCrypt.Net.BCrypt.HashPassword(model.Password)
            );

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
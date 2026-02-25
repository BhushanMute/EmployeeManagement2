using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbHelper _dbHelper;

        public RoleRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = new List<Role>();

            using var connection = _dbHelper.GetConnection();
            using var command = new SqlCommand("SELECT Id, RoleName, Description, IsActive FROM Roles WHERE IsActive = 1", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    Id = reader.GetInt32(0),
                    RoleName = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsActive = reader.GetBoolean(3)
                });
            }

            return roles;
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            using var connection = _dbHelper.GetConnection();
            using var command = new SqlCommand("SELECT Id, RoleName, Description, IsActive FROM Roles WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Role
                {
                    Id = reader.GetInt32(0),
                    RoleName = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsActive = reader.GetBoolean(3)
                };
            }

            return null;
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            var roles = new List<Role>();

            using var connection = _dbHelper.GetConnection();
            using var command = new SqlCommand("sp_GetUserRoles", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null : reader.GetString(reader.GetOrdinal("Description"))
                });
            }

            return roles;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy)
        {
            using var connection = _dbHelper.GetConnection();
            using var command = new SqlCommand("sp_AssignRoleToUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);
            command.Parameters.AddWithValue("@AssignedBy", assignedBy);

            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            using var connection = _dbHelper.GetConnection();
            using var command = new SqlCommand("sp_RemoveRoleFromUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return true;
        }
    }
}

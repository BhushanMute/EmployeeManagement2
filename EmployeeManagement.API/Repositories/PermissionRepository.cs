using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
     
        public class PermissionRepository : IPermissionRepository
        {
            private readonly DbHelper _dbHelper;

            public PermissionRepository(DbHelper dbHelper)
            {
                _dbHelper = dbHelper;
            }

            public async Task<List<Permission>> GetAllPermissionsAsync()
            {
                var permissions = new List<Permission>();

                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(
                    "SELECT Id, PermissionName, Description, Module FROM Permissions WHERE IsActive = 1",
                    connection);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    permissions.Add(new Permission
                    {
                        Id = reader.GetInt32(0),
                        PermissionName = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Module = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }

                return permissions;
            }

            public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
            {
                var permissions = new List<Permission>();

                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand("sp_GetUserPermissions", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    permissions.Add(new Permission
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                        PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                            ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Module = reader.IsDBNull(reader.GetOrdinal("Module"))
                            ? null : reader.GetString(reader.GetOrdinal("Module"))
                    });
                }

                return permissions;
            }

            public async Task<List<Permission>> GetRolePermissionsAsync(int roleId)
            {
                var permissions = new List<Permission>();

                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(@"
                SELECT p.Id, p.PermissionName, p.Description, p.Module
                FROM Permissions p
                INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
                WHERE rp.RoleId = @RoleId AND rp.IsActive = 1 AND p.IsActive = 1",
                    connection);
                command.Parameters.AddWithValue("@RoleId", roleId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    permissions.Add(new Permission
                    {
                        Id = reader.GetInt32(0),
                        PermissionName = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Module = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }

                return permissions;
            }

            public async Task<bool> CheckUserPermissionAsync(int userId, string permissionName)
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand("sp_CheckUserPermission", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PermissionName", permissionName);

                var hasPermissionParam = new SqlParameter("@HasPermission", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(hasPermissionParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return (bool)hasPermissionParam.Value;
            }
        }
     
}

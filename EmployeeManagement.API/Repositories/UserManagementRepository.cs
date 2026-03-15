using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserManagementRepository> _logger;

        public UserManagementRepository(IConfiguration config, ILogger<UserManagementRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found.");
            _logger = logger;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public async Task<List<PendingUser>> GetPendingUsers()
        {
            var users = new List<PendingUser>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetPendingUsers", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapPendingUser(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending users");
                throw;
            }

            return users;
        }

        public async Task<List<PendingUser>> GetAllUsersWithRoles()
        {
            var users = new List<PendingUser>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllUsersWithRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapPendingUser(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users with roles");
                throw;
            }

            return users;
        }

        public async Task ApproveUserAndAssignRole(int userId, int roleId, int? departmentId, int approvedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_ApproveUserAndAssignRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user: {UserId}", userId);
                throw;
            }
        }

        public async Task RejectUser(int userId, int rejectedBy, string? reason)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_RejectUserRegistration", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
                cmd.Parameters.AddWithValue("@RejectionReason", (object?)reason ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<RoleInfo>> GetAllRoles()
        {
            var roles = new List<RoleInfo>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new RoleInfo
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                throw;
            }

            return roles;
        }

        private PendingUser MapPendingUser(SqlDataReader reader)
        {
            return new PendingUser
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader["Username"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                FullName = reader["FullName"]?.ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                RegistrationStatus = reader["RegistrationStatus"]?.ToString() ?? "Pending",
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                EmployeeId = reader.IsDBNull(reader.GetOrdinal("EmployeeId")) ? null : reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                EmployeeName = reader["EmployeeName"]?.ToString(),
                DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentName = reader["DepartmentName"]?.ToString(),
                AssignedRoles = reader["AssignedRoles"]?.ToString()
            };
        }
    }
}
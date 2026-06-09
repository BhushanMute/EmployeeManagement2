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

        public async Task<UserListResponse> GetAllUsersAsync(string? search, string? role, string? status, int pageNumber, int pageSize)
        {
            var response = new UserListResponse { PageNumber = pageNumber, PageSize = pageSize };

            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_GetAllUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchTerm", (object?)search ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleFilter", (object?)role ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StatusFilter", (object?)status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            // First: total count
            if (await reader.ReadAsync())
                response.TotalRecords = reader.GetInt32(0);

            // Second: users
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    response.Users.Add(new UserListItem
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader["Username"]?.ToString() ?? "",
                        FullName = reader["FullName"]?.ToString() ?? "",
                        Email = reader["Email"]?.ToString() ?? "",
                        PhoneNumber = reader["PhoneNumber"]?.ToString(),
                        ProfilePicture = reader["ProfilePicture"]?.ToString(),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginDate")),
                        Roles = reader["Roles"]?.ToString(),
                        RoleIds = reader["RoleIds"]?.ToString()
                    });
                }
            }

            return response;
        }

        public async Task<UserDetailResponse?> GetUserByIdAsync(int userId)
        {
            UserDetailResponse? user = null;

            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_GetUserById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                user = new UserDetailResponse
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Username = reader["Username"]?.ToString() ?? "",
                    FullName = reader["FullName"]?.ToString() ?? "",
                    Email = reader["Email"]?.ToString() ?? "",
                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                    ProfilePicture = reader["ProfilePicture"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("IsEmailVerified")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginDate")),
                    EmployeeCode = reader["EmployeeCode"]?.ToString(),
                    DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                    DepartmentName = reader["DepartmentName"]?.ToString(),
                    DesignationId = reader.IsDBNull(reader.GetOrdinal("DesignationId")) ? null : reader.GetInt32(reader.GetOrdinal("DesignationId")),
                    DesignationName = reader["DesignationName"]?.ToString(),
                    JoiningDate = reader.IsDBNull(reader.GetOrdinal("JoiningDate")) ? null : reader.GetDateTime(reader.GetOrdinal("JoiningDate"))
                };
            }

            // Roles
            if (user != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    user.Roles.Add(new UserRoleItem
                    {
                        UserRoleId = reader.GetInt32(reader.GetOrdinal("UserRoleId")),
                        RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                        RoleName = reader["RoleName"]?.ToString() ?? "",
                        RoleDescription = reader["RoleDescription"]?.ToString(),
                        AssignedDate = reader.GetDateTime(reader.GetOrdinal("AssignedDate")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
            }

            return user;
        }

        public async Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, int createdBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_CreateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Username", request.Username);
            cmd.Parameters.AddWithValue("@FullName", request.FullName);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)request.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordHash", request.PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", "BCryptEmbeddedSalt");
            cmd.Parameters.AddWithValue("@RoleIds", request.RoleIds);
            cmd.Parameters.AddWithValue("@DepartmentId", (object?)request.DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DesignationId", (object?)request.DesignationId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeCode", (object?)request.EmployeeCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            var newIdParam = new SqlParameter("@NewUserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(newIdParam);

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = Convert.ToInt32(reader["Success"]) == 1,
                    Message = reader["Message"]?.ToString() ?? "",
                    NewId = reader["UserId"] == DBNull.Value ? null : Convert.ToInt32(reader["UserId"])
                };
            }

            return new UserOperationResult
            {
                Success = false,
                Message = "No response from database"
            };
        }
        public async Task<UserOperationResult> UpdateUserAsync(UpdateUserRequest request, int updatedBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_UpdateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", request.UserId);
            cmd.Parameters.AddWithValue("@FullName", request.FullName);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)request.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", request.IsActive);
            cmd.Parameters.AddWithValue("@RoleIds", request.RoleIds);
            cmd.Parameters.AddWithValue("@DepartmentId", (object?)request.DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DesignationId", (object?)request.DesignationId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeCode", (object?)request.EmployeeCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "No response" };
        }

        public async Task<UserOperationResult> DeleteUserAsync(int userId, int deletedBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_DeleteUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<UserOperationResult> ToggleStatusAsync(int userId, bool isActive, int updatedBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_ToggleUserStatus", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<UserOperationResult> ResetPasswordAsync(int userId, string passwordHash, int resetBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_ResetUserPassword", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@NewPasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@ResetBy", resetBy);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<List<RoleWithCountResponse>> GetAllRolesAsync()
        {
            var roles = new List<RoleWithCountResponse>();

            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_GetAllRolesWithCount", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new RoleWithCountResponse
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"]?.ToString() ?? "",
                    RoleDescription = reader["RoleDescription"] == DBNull.Value
                        ? null
                        : reader["RoleDescription"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedDate = reader["CreatedDate"] == DBNull.Value
                        ? DateTime.Now
                        : Convert.ToDateTime(reader["CreatedDate"]),
                    UserCount = Convert.ToInt32(reader["UserCount"])
                });
            }

            return roles;
        }

        public async Task<UserOperationResult> CreateRoleAsync(CreateRoleRequest request, int createdBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_CreateRole", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleName", request.RoleName);
            cmd.Parameters.AddWithValue("@RoleDescription", (object?)request.RoleDescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            var newIdParam = new SqlParameter("@NewRoleId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(newIdParam);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1),
                    NewId = newIdParam.Value as int?
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<UserOperationResult> UpdateRoleAsync(UpdateRoleRequest request, int updatedBy)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_UpdateRole", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleId", request.RoleId);
            cmd.Parameters.AddWithValue("@RoleName", request.RoleName);
            cmd.Parameters.AddWithValue("@RoleDescription", (object?)request.RoleDescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", request.IsActive);
            cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<UserOperationResult> DeleteRoleAsync(int roleId)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_DeleteRole", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleId", roleId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserOperationResult
                {
                    Success = reader.GetInt32(0) == 1,
                    Message = reader.GetString(1)
                };
            }
            return new UserOperationResult { Success = false, Message = "Failed" };
        }

        public async Task<UserDropdownData> GetDropdownDataAsync()
        {
            var data = new UserDropdownData();

            using var conn = GetConnection();
            using var cmd = new SqlCommand("sp_GetUserManagementDropdowns", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            // 1. Roles result set
            while (await reader.ReadAsync())
            {
                data.Roles.Add(new RoleWithCountResponse
                {
                    RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    RoleName = reader["RoleName"]?.ToString() ?? "",
                    RoleDescription = reader["RoleDescription"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserCount = reader.GetInt32(reader.GetOrdinal("UserCount"))
                });
            }

            // 2. Departments result set
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Departments.Add(new DropdownItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader["Name"]?.ToString() ?? ""
                    });
                }
            }

            // 3. Designations result set
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Designations.Add(new DropdownItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader["Name"]?.ToString() ?? "",
                        DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId"))
                            ? null
                            : reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                    });
                }
            }

            return data;
        }
    }
}
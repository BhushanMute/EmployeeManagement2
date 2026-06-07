using Dapper;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<LeaveRepository> _logger;

        public LeaveRepository(IConfiguration config, ILogger<LeaveRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
            _logger = logger;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region Leave Types

        public async Task<List<LeaveType>> GetAllLeaveTypes()
        {
            var leaveTypes = new List<LeaveType>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllLeaveTypes", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    leaveTypes.Add(MapLeaveType(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all leave types");
                throw;
            }

            return leaveTypes;
        }

        public async Task<LeaveType?> GetLeaveTypeById(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetLeaveTypeById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapLeaveType(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave type by id: {Id}", id);
                throw;
            }

            return null;
        }

        #endregion

        #region Leave Requests

        public async Task<int> ApplyLeave(LeaveRequest request)
        {
            try
            {
                _logger.LogInformation("🚀 ApplyLeave called for Employee: {EmpId}, AttachmentPath: {Path}",
                    request.EmployeeId,
                    request.AttachmentPath ?? "NULL");

                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_ApplyLeave", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // ✅ ALL 12 input parameters (matching SP)
                cmd.Parameters.AddWithValue("@EmployeeId", request.EmployeeId);
                cmd.Parameters.AddWithValue("@LeaveTypeId", request.LeaveTypeId);
                cmd.Parameters.AddWithValue("@StartDate", request.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", request.EndDate);
                cmd.Parameters.AddWithValue("@TotalDays", request.TotalDays);
                cmd.Parameters.AddWithValue("@Reason", request.Reason ?? string.Empty);
                cmd.Parameters.AddWithValue("@IsHalfDay", request.IsHalfDay);
                cmd.Parameters.AddWithValue("@HalfDayType",
                    (object?)request.HalfDayType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmergencyContact",
                    (object?)request.EmergencyContact ?? DBNull.Value);

                // ✅ CRITICAL: AttachmentPath
                cmd.Parameters.AddWithValue("@AttachmentPath",
                    (object?)request.AttachmentPath ?? DBNull.Value);

                // ✅ ADD: Status parameter (was missing!)
                cmd.Parameters.AddWithValue("@Status", request.Status ?? "Pending");

                cmd.Parameters.AddWithValue("@CreatedBy",
                    (object?)request.CreatedBy ?? DBNull.Value);

                // ✅ FIX: Output parameter name must match SP (@NewLeaveRequestId, not @NewId)
                var newIdParam = new SqlParameter("@NewLeaveRequestId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(newIdParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                var newId = (int)newIdParam.Value;

                _logger.LogInformation("✅ Leave created: ID={Id}, AttachmentPath={Path}",
                    newId, request.AttachmentPath ?? "NULL");

                return newId;
            }
            catch (SqlException ex) when (ex.Message.Contains("overlapping") || ex.Message.Contains("already have"))
            {
                _logger.LogWarning("Overlapping leave request for employee: {EmployeeId}", request.EmployeeId);
                throw new Exception("You already have a leave request for the selected dates.");
            }
            catch (SqlException ex) when (ex.Message.Contains("Insufficient"))
            {
                _logger.LogWarning("Insufficient balance for employee: {EmployeeId}", request.EmployeeId);
                throw new Exception("Insufficient leave balance.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave for employee: {EmployeeId}", request.EmployeeId);
                throw;
            }
        }

        public async Task<LeaveRequest?> GetLeaveRequestById(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetLeaveRequestById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapLeaveRequest(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave request by id: {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<List<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId, int? year = null)
        {
            var requests = new List<LeaveRequest>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeLeaveHistory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    requests.Add(MapLeaveRequest(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave history for employee: {EmployeeId}", employeeId);
                throw;
            }

            return requests;
        }

        public async Task<List<LeaveRequest>> GetPendingLeaveRequests(int? approverId = null, int? departmentId = null)
        {
            var requests = new List<LeaveRequest>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetPendingLeaveRequests", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApproverId", (object?)approverId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    requests.Add(MapLeaveRequest(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending leave requests");
                throw;
            }

            return requests;
        }

        public async Task<PagedResult<LeaveRequest>> GetAllLeaveRequests(
            string? status, int? departmentId, int? leaveTypeId,
            DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var result = new PagedResult<LeaveRequest>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllLeaveRequests", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LeaveTypeId", (object?)leaveTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", (object?)startDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", (object?)endDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(totalRecordsParam);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Items.Add(MapLeaveRequest(reader));
                }

                await reader.CloseAsync();
                result.TotalRecords = (int)totalRecordsParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all leave requests");
                throw;
            }

            return result;
        }

        #endregion

        #region Leave Actions

        public async Task ApproveLeave(int leaveRequestId, int approvedBy, string? remarks)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_ApproveLeave", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LeaveRequestId", leaveRequestId);
                cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request: {Id}", leaveRequestId);
                throw;
            }
        }

        public async Task RejectLeave(int leaveRequestId, int rejectedBy, string? remarks)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_RejectLeave", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LeaveRequestId", leaveRequestId);
                cmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
                cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave request: {Id}", leaveRequestId);
                throw;
            }
        }

        public async Task CancelLeave(int leaveRequestId, int cancelledBy, string? cancelReason)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_CancelLeave", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LeaveRequestId", leaveRequestId);
                cmd.Parameters.AddWithValue("@CancelledBy", cancelledBy);
                cmd.Parameters.AddWithValue("@CancelReason", (object?)cancelReason ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request: {Id}", leaveRequestId);
                throw;
            }
        }

        #endregion

        #region Leave Balance

        public async Task<List<LeaveBalance>> GetLeaveBalance(int employeeId, int? year = null)
        {
            var balances = new List<LeaveBalance>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetLeaveBalance", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    balances.Add(MapLeaveBalance(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave balance for employee: {EmployeeId}", employeeId);
                throw;
            }

            return balances;
        }

        public async Task AllocateLeaveBalance(int employeeId, int leaveTypeId, int year,
            decimal totalAllocated, decimal carryForward, int? createdBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AllocateLeaveBalance", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@LeaveTypeId", leaveTypeId);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@TotalAllocated", totalAllocated);
                cmd.Parameters.AddWithValue("@CarryForward", carryForward);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)createdBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating leave balance");
                throw;
            }
        }

        public async Task AllocateDefaultLeaveForAllEmployees(int year, int? createdBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AllocateDefaultLeaveForAllEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)createdBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating default leave for year: {Year}", year);
                throw;
            }
        }

        #endregion

        #region Holidays

        public async Task<List<Holiday>> GetHolidays(int? year = null)
        {
            var holidays = new List<Holiday>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetHolidays", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    holidays.Add(new Holiday
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Date = reader.GetDateTime("Date"),
                        Day = reader["Day"]?.ToString() ?? string.Empty,
                        Type = reader["Type"]?.ToString() ?? "Public",
                        Description = reader.GetNullableString("Description"),
                        Year = reader.GetInt32("Year"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting holidays");
                throw;
            }

            return holidays;
        }

        #region Holidays CRUD

         

        public async Task<Holiday?> GetHolidayById(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetHolidayById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapHoliday(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting holiday by id: {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<int> AddHoliday(Holiday holiday)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AddHoliday", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", holiday.Name);
                cmd.Parameters.AddWithValue("@Date", holiday.Date);
                cmd.Parameters.AddWithValue("@Type", holiday.Type);
                cmd.Parameters.AddWithValue("@Description", (object?)holiday.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)holiday.CreatedBy ?? DBNull.Value);

                var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(newIdParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)newIdParam.Value;
            }
            catch (SqlException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning("Duplicate holiday date: {Date}", holiday.Date);
                throw new Exception("A holiday already exists on this date.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding holiday");
                throw;
            }
        }

        public async Task UpdateHoliday(Holiday holiday)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_UpdateHoliday", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", holiday.Id);
                cmd.Parameters.AddWithValue("@Name", holiday.Name);
                cmd.Parameters.AddWithValue("@Date", holiday.Date);
                cmd.Parameters.AddWithValue("@Type", holiday.Type);
                cmd.Parameters.AddWithValue("@Description", (object?)holiday.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", holiday.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)holiday.UpdatedBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Message.Contains("already exists"))
            {
                throw new Exception("Another holiday already exists on this date.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating holiday: {Id}", holiday.Id);
                throw;
            }
        }

        public async Task DeleteHoliday(int id, int? deletedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_DeleteHoliday", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@DeletedBy", (object?)deletedBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting holiday: {Id}", id);
                throw;
            }
        }

        public async Task<HolidayStats> GetHolidaysCount(int? year = null)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetHolidaysCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new HolidayStats
                    {
                        TotalHolidays = reader.GetInt32(reader.GetOrdinal("TotalHolidays")),
                        PublicHolidays = reader.GetInt32(reader.GetOrdinal("PublicHolidays")),
                        OptionalHolidays = reader.GetInt32(reader.GetOrdinal("OptionalHolidays")),
                        RestrictedHolidays = reader.GetInt32(reader.GetOrdinal("RestrictedHolidays")),
                        UpcomingHolidays = reader.GetInt32(reader.GetOrdinal("UpcomingHolidays")),
                        PastHolidays = reader.GetInt32(reader.GetOrdinal("PastHolidays"))
                    };
                }

                return new HolidayStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting holidays count");
                throw;
            }
        }

        #endregion

        #region Holiday Mapper

        private Holiday MapHoliday(SqlDataReader reader)
        {
            return new Holiday
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Date = reader.GetDateTime("Date"),
                Day = reader["Day"]?.ToString() ?? string.Empty,
                Type = reader["Type"]?.ToString() ?? "Public",
                Description = reader.GetNullableString("Description"),
                Year = reader.GetInt32("Year"),
                IsActive = reader.GetBoolean("IsActive"),
                IsDeleted = reader.GetBoolean("IsDeleted"),
                CreatedBy = reader.GetNullableInt32("CreatedBy"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedBy = reader.GetNullableInt32("UpdatedBy"),
                UpdatedDate = reader.GetNullableDateTime("UpdatedDate")
            };
        }

        #endregion

        #endregion

        #region Reports

        public async Task<List<MonthlyLeaveReportItem>> GetMonthlyLeaveReport(int month, int year, int? departmentId = null)
        {
            var report = new List<MonthlyLeaveReportItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetMonthlyLeaveReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    report.Add(new MonthlyLeaveReportItem
                    {
                        EmployeeId = reader.GetInt32("EmployeeId"),
                        EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                        DepartmentName = reader.GetNullableString("DepartmentName"),
                        LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                        TotalRequests = reader.GetInt32("TotalRequests"),
                        TotalDays = reader.GetDecimal("TotalDays"),
                        ApprovedDays = reader.GetDecimal("ApprovedDays"),
                        RejectedDays = reader.GetDecimal("RejectedDays"),
                        PendingDays = reader.GetDecimal("PendingDays"),
                        CancelledDays = reader.GetDecimal("CancelledDays")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly leave report");
                throw;
            }

            return report;
        }

        #region Reports

        public async Task<List<EmployeeLeaveReportItem>> GetEmployeeLeaveReport(int? year, int? departmentId, int? employeeId)
        {
            var report = new List<EmployeeLeaveReportItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeLeaveReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeId", (object?)employeeId ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    report.Add(new EmployeeLeaveReportItem
                    {
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                        EmployeeEmail = reader["EmployeeEmail"]?.ToString(),
                        DepartmentName = reader["DepartmentName"]?.ToString(),
                        LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                        LeaveTypeCode = reader["LeaveTypeCode"]?.ToString() ?? string.Empty,
                        TotalAllocated = reader.GetDecimal(reader.GetOrdinal("TotalAllocated")),
                        TotalUsed = reader.GetDecimal(reader.GetOrdinal("TotalUsed")),
                        TotalPending = reader.GetDecimal(reader.GetOrdinal("TotalPending")),
                        CarryForward = reader.GetDecimal(reader.GetOrdinal("CarryForward")),
                        TotalAvailable = reader.GetDecimal(reader.GetOrdinal("TotalAvailable")),
                        ApprovedCount = reader.GetInt32(reader.GetOrdinal("ApprovedCount")),
                        RejectedCount = reader.GetInt32(reader.GetOrdinal("RejectedCount")),
                        PendingCount = reader.GetInt32(reader.GetOrdinal("PendingCount"))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee leave report");
                throw;
            }

            return report;
        }

        public async Task<List<DepartmentLeaveReportItem>> GetDepartmentLeaveReport(int? year)
        {
            var report = new List<DepartmentLeaveReportItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetDepartmentLeaveReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    report.Add(new DepartmentLeaveReportItem
                    {
                        DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                        TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees")),
                        TotalAllocated = reader.GetDecimal(reader.GetOrdinal("TotalAllocated")),
                        TotalUsed = reader.GetDecimal(reader.GetOrdinal("TotalUsed")),
                        TotalPending = reader.GetDecimal(reader.GetOrdinal("TotalPending")),
                        TotalAvailable = reader.GetDecimal(reader.GetOrdinal("TotalAvailable")),
                        ApprovedRequests = reader.GetInt32(reader.GetOrdinal("ApprovedRequests")),
                        PendingRequests = reader.GetInt32(reader.GetOrdinal("PendingRequests")),
                        RejectedRequests = reader.GetInt32(reader.GetOrdinal("RejectedRequests"))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department leave report");
                throw;
            }

            return report;
        }

        public async Task<List<LeaveCalendarItem>> GetLeaveCalendarData(int month, int year, int? departmentId)
        {
            var items = new List<LeaveCalendarItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetLeaveCalendarData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(new LeaveCalendarItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                        DepartmentName = reader["DepartmentName"]?.ToString(),
                        LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                        LeaveTypeCode = reader["LeaveTypeCode"]?.ToString() ?? string.Empty,
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        TotalDays = reader.GetDecimal(reader.GetOrdinal("TotalDays")),
                        Status = reader["Status"]?.ToString() ?? string.Empty,
                        IsHalfDay = reader.GetBoolean(reader.GetOrdinal("IsHalfDay")),
                        HalfDayType = reader.GetNullableString("HalfDayType"),
                        Reason = reader.GetNullableString("Reason")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave calendar data");
                throw;
            }

            return items;
        }

        public async Task<LeaveDashboardStats> GetLeaveDashboardStats(int? year)
        {
            var stats = new LeaveDashboardStats();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetLeaveDashboardStats", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // First result set: Overall stats
                if (await reader.ReadAsync())
                {
                    stats.TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests"));
                    stats.ApprovedRequests = reader.GetInt32(reader.GetOrdinal("ApprovedRequests"));
                    stats.RejectedRequests = reader.GetInt32(reader.GetOrdinal("RejectedRequests"));
                    stats.PendingRequests = reader.GetInt32(reader.GetOrdinal("PendingRequests"));
                    stats.CancelledRequests = reader.GetInt32(reader.GetOrdinal("CancelledRequests"));
                    stats.TotalApprovedDays = reader.GetDecimal(reader.GetOrdinal("TotalApprovedDays"));
                    stats.EmployeesWithLeave = reader.GetInt32(reader.GetOrdinal("EmployeesWithLeave"));
                    stats.TotalActiveEmployees = reader.GetInt32(reader.GetOrdinal("TotalActiveEmployees"));
                }

                // Second result set: Monthly breakdown
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.MonthlyData.Add(new MonthlyBreakdown
                        {
                            MonthNumber = reader.GetInt32(reader.GetOrdinal("MonthNumber")),
                            MonthName = reader["MonthName"]?.ToString() ?? string.Empty,
                            TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests")),
                            Approved = reader.GetInt32(reader.GetOrdinal("Approved")),
                            Rejected = reader.GetInt32(reader.GetOrdinal("Rejected")),
                            Pending = reader.GetInt32(reader.GetOrdinal("Pending")),
                            ApprovedDays = reader.GetDecimal(reader.GetOrdinal("ApprovedDays"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                throw;
            }

            return stats;
        }

        #endregion
        #region Leave Allocation

        public async Task<int> AllocateFixedLeaveForAllEmployees(int year, decimal leavesPerType, int? createdBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AllocateFixedLeaveForAllEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@LeavesPerType", leavesPerType);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)createdBy ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(reader.GetOrdinal("RecordsInserted"));
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating fixed leave for all employees");
                throw;
            }
        }

        public async Task AllocateLeaveForSingleEmployee(int employeeId, int year, decimal leavesPerType, int? createdBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AllocateLeaveForSingleEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@LeavesPerType", leavesPerType);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)createdBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating leave for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<List<LeaveBalance>> GetAllEmployeeBalances(int year)
        {
            var balances = new List<LeaveBalance>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllEmployeeBalances", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    balances.Add(MapLeaveBalance(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employee balances for year: {Year}", year);
                throw;
            }

            return balances;
        }

        #endregion
        #region Admin Dashboard

        public async Task<AdminLeaveDashboard> GetAdminLeaveDashboard(int? year = null)
        {
            var dashboard = new AdminLeaveDashboard();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAdminLeaveDashboard", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // Result Set 1: Overall Stats
                if (await reader.ReadAsync())
                {
                    dashboard.TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees"));
                    dashboard.TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests"));
                    dashboard.PendingRequests = reader.GetInt32(reader.GetOrdinal("PendingRequests"));
                    dashboard.ApprovedRequests = reader.GetInt32(reader.GetOrdinal("ApprovedRequests"));
                    dashboard.RejectedRequests = reader.GetInt32(reader.GetOrdinal("RejectedRequests"));
                    dashboard.CancelledRequests = reader.GetInt32(reader.GetOrdinal("CancelledRequests"));
                    dashboard.TotalApprovedDays = reader.GetDecimal(reader.GetOrdinal("TotalApprovedDays"));
                    dashboard.EmployeesOnLeave = reader.GetInt32(reader.GetOrdinal("EmployeesOnLeave"));
                    dashboard.OnLeaveToday = reader.GetInt32(reader.GetOrdinal("OnLeaveToday"));
                    dashboard.TotalHolidays = reader.GetInt32(reader.GetOrdinal("TotalHolidays"));
                    dashboard.UpcomingHolidays = reader.GetInt32(reader.GetOrdinal("UpcomingHolidays"));
                }

                // Result Set 2: Monthly Breakdown
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.MonthlyData.Add(new MonthlyBreakdown
                        {
                            MonthNumber = reader.GetInt32(reader.GetOrdinal("MonthNumber")),
                            MonthName = reader["MonthName"]?.ToString() ?? string.Empty,
                            TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests")),
                            Approved = reader.GetInt32(reader.GetOrdinal("Approved")),
                            Rejected = reader.GetInt32(reader.GetOrdinal("Rejected")),
                            Pending = reader.GetInt32(reader.GetOrdinal("Pending")),
                            ApprovedDays = reader.GetDecimal(reader.GetOrdinal("ApprovedDays"))
                        });
                    }
                }

                // Result Set 3: Leave Type Breakdown
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.LeaveTypeData.Add(new LeaveTypeBreakdown
                        {
                            LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                            LeaveTypeCode = reader["LeaveTypeCode"]?.ToString() ?? string.Empty,
                            TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests")),
                            ApprovedDays = reader.GetDecimal(reader.GetOrdinal("ApprovedDays")),
                            PendingDays = reader.GetDecimal(reader.GetOrdinal("PendingDays"))
                        });
                    }
                }

                // Result Set 4: Department Breakdown
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.DepartmentData.Add(new DepartmentBreakdown
                        {
                            DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                            TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees")),
                            TotalRequests = reader.GetInt32(reader.GetOrdinal("TotalRequests")),
                            ApprovedDays = reader.GetDecimal(reader.GetOrdinal("ApprovedDays")),
                            PendingCount = reader.GetInt32(reader.GetOrdinal("PendingCount"))
                        });
                    }
                }

                // Result Set 5: Recent Requests
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.RecentRequests.Add(new RecentLeaveItem
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                            LeaveTypeCode = reader["LeaveTypeCode"]?.ToString() ?? string.Empty,
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            TotalDays = reader.GetDecimal(reader.GetOrdinal("TotalDays")),
                            Status = reader["Status"]?.ToString() ?? string.Empty,
                            AppliedDate = reader.GetDateTime(reader.GetOrdinal("AppliedDate")),
                            Reason = reader["Reason"]?.ToString()
                        });
                    }
                }

                // Result Set 6: On Leave Today
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.OnLeaveTodayList.Add(new TodayLeaveItem
                        {
                            EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            LeaveTypeName = reader["LeaveTypeName"]?.ToString() ?? string.Empty,
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            TotalDays = reader.GetDecimal(reader.GetOrdinal("TotalDays"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin leave dashboard");
                throw;
            }

            return dashboard;
        }
        public async Task<int> CreateLeaveRequestAsync(LeaveRequest request, int createdBy)
        {
            try
            {
                using var conn = GetConnection();

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                var parameters = new DynamicParameters();

                // ✅ Input parameters - MUST match SP exactly (12 inputs)
                parameters.Add("@EmployeeId", request.EmployeeId, DbType.Int32);
                parameters.Add("@LeaveTypeId", request.LeaveTypeId, DbType.Int32);
                parameters.Add("@StartDate", request.StartDate, DbType.Date);
                parameters.Add("@EndDate", request.EndDate, DbType.Date);
                parameters.Add("@TotalDays", request.TotalDays, DbType.Decimal);
                parameters.Add("@Reason", request.Reason ?? string.Empty, DbType.String, size: 1000);
                parameters.Add("@IsHalfDay", request.IsHalfDay, DbType.Boolean);
                parameters.Add("@HalfDayType", (object?)request.HalfDayType ?? DBNull.Value, DbType.String, size: 20);
                parameters.Add("@EmergencyContact", (object?)request.EmergencyContact ?? DBNull.Value, DbType.String, size: 100);
                parameters.Add("@AttachmentPath", (object?)request.AttachmentPath ?? DBNull.Value, DbType.String, size: 500);
                parameters.Add("@Status", request.Status ?? "Pending", DbType.String, size: 20);
                parameters.Add("@CreatedBy", createdBy, DbType.Int32);

                // ✅ Output parameter (1)
                parameters.Add("@NewLeaveRequestId",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                // Total = 12 input + 1 output = 13 parameters ✅

                // ✅ Execute the stored procedure
                await conn.ExecuteAsync(
                    "sp_ApplyLeave",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // ✅ Get output parameter value
                var newId = parameters.Get<int>("@NewLeaveRequestId");

                _logger.LogInformation("✅ Leave request created with ID: {Id} for Employee: {EmpId}",
                    newId, request.EmployeeId);

                return newId;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx,
                    "❌ SQL Error creating leave for Employee {EmpId}. Error: {Error}",
                    request.EmployeeId, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error creating leave request for Employee {EmpId}",
                    request.EmployeeId);
                throw;
            }
        } 
        #endregion
        #endregion

        #region Mapper Methods

        private LeaveType MapLeaveType(SqlDataReader reader)
        {
            return new LeaveType
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Description = reader.GetNullableString("Description"),
                DefaultDays = reader.GetInt32("DefaultDays"),
                MaxDays = reader.GetInt32("MaxDays"),
                IsCarryForward = reader.GetBoolean("IsCarryForward"),
                MaxCarryForward = reader.GetInt32("MaxCarryForward"),
                IsPaid = reader.GetBoolean("IsPaid"),
                IsActive = reader.GetBoolean("IsActive"),
                IsDeleted = reader.GetBoolean("IsDeleted"),
                CreatedBy = reader.GetNullableInt32("CreatedBy"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedBy = reader.GetNullableInt32("UpdatedBy"),
                UpdatedDate = reader.GetNullableDateTime("UpdatedDate")
            };
        }

        private LeaveRequest MapLeaveRequest(SqlDataReader reader)
        {
            return new LeaveRequest
            {
                Id = reader.GetInt32("Id"),
                EmployeeId = reader.GetInt32("EmployeeId"),
                LeaveTypeId = reader.GetInt32("LeaveTypeId"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
                TotalDays = reader.GetDecimal("TotalDays"),
                Reason = reader["Reason"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Pending",
                IsHalfDay = reader.GetBoolean("IsHalfDay"),
                HalfDayType = reader.GetNullableString("HalfDayType"),
                AttachmentPath = reader.GetNullableString("AttachmentPath"),
                EmergencyContact = reader.GetNullableString("EmergencyContact"),
                Remarks = reader.GetNullableString("Remarks"),
                AppliedDate = reader.GetDateTime("AppliedDate"),
                ApprovedBy = reader.GetNullableInt32("ApprovedBy"),
                ApprovedDate = reader.GetNullableDateTime("ApprovedDate"),
                RejectedBy = reader.GetNullableInt32("RejectedBy"),
                RejectedDate = reader.GetNullableDateTime("RejectedDate"),
                CancelledDate = reader.GetNullableDateTime("CancelledDate"),
                CancelReason = reader.GetNullableString("CancelReason"),
                IsActive = reader.GetBoolean("IsActive"),
                IsDeleted = reader.GetBoolean("IsDeleted"),
                CreatedBy = reader.GetNullableInt32("CreatedBy"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedBy = reader.GetNullableInt32("UpdatedBy"),
                UpdatedDate = reader.GetNullableDateTime("UpdatedDate"),
                EmployeeName = reader.GetNullableString("EmployeeName"),
                EmployeeEmail = reader.GetNullableString("EmployeeEmail"),
                DepartmentId = reader.GetNullableInt32("DepartmentId"),
                DepartmentName = reader.GetNullableString("DepartmentName"),
                LeaveTypeName = reader.GetNullableString("LeaveTypeName"),
                LeaveTypeCode = reader.GetNullableString("LeaveTypeCode")
            };
        }

        private LeaveBalance MapLeaveBalance(SqlDataReader reader)
        {
            return new LeaveBalance
            {
                Id = reader.GetInt32("Id"),
                EmployeeId = reader.GetInt32("EmployeeId"),
                LeaveTypeId = reader.GetInt32("LeaveTypeId"),
                Year = reader.GetInt32("Year"),
                TotalAllocated = reader.GetDecimal("TotalAllocated"),
                TotalUsed = reader.GetDecimal("TotalUsed"),
                TotalPending = reader.GetDecimal("TotalPending"),
                CarryForward = reader.GetDecimal("CarryForward"),
                TotalAvailable = reader.GetDecimal("TotalAvailable"),
                LeaveTypeName = reader.GetNullableString("LeaveTypeName"),
                LeaveTypeCode = reader.GetNullableString("LeaveTypeCode"),
                IsPaid = reader.GetBoolean("IsPaid"),
                EmployeeName = reader.GetNullableString("EmployeeName")
            };
        }

        #endregion
    }
}
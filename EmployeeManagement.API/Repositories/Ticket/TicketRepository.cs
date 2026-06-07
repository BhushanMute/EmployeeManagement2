using EmployeeManagement.API.Models.Ticket;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmployeeManagement.API.Repositories.Ticket
{
    /// <summary>
    /// Ticket Repository Implementation using ADO.NET with Stored Procedures
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(IConfiguration config, ILogger<TicketRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found.");
            _logger = logger;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        #region TICKET CRUD OPERATIONS

        /// <summary>
        /// Create a new ticket
        /// </summary>
        public async Task<CreateTicketResponse> CreateTicketAsync(CreateTicketRequest request, int createdBy)
        {
            var response = new CreateTicketResponse();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_CreateTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.AddWithValue("@Title", request.Title);
                cmd.Parameters.AddWithValue("@Description", request.Description);
                cmd.Parameters.AddWithValue("@TicketType", request.TicketType);
                cmd.Parameters.AddWithValue("@Priority", request.Priority);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@AssignedTo", (object?)request.AssignedTo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DueDate", (object?)request.DueDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StepsToReproduce", (object?)request.StepsToReproduce ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectedResult", (object?)request.ExpectedResult ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActualResult", (object?)request.ActualResult ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Environment", (object?)request.Environment ?? DBNull.Value);

                // Output parameters
                var ticketIdParam = new SqlParameter("@TicketId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var ticketNumberParam = new SqlParameter("@TicketNumber", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(ticketIdParam);
                cmd.Parameters.Add(ticketNumberParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                response.TicketId = (int)ticketIdParam.Value;
                response.TicketNumber = ticketNumberParam.Value?.ToString() ?? string.Empty;
                response.Message = "Ticket created successfully";

                _logger.LogInformation("Ticket created: {TicketNumber} by User {UserId}", response.TicketNumber, createdBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket");
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get ticket by ID with full details
        /// </summary>
        public async Task<Models.Ticket.Ticket?> GetTicketByIdAsync(int ticketId)
        {
            Models.Ticket.Ticket? ticket = null;

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    ticket = MapTicketFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket by ID: {TicketId}", ticketId);
                throw;
            }

            return ticket;
        }

        /// <summary>
        /// Get all tickets with filters and pagination
        /// </summary>
        public async Task<TicketListResponse> GetTicketsAsync(TicketFilterRequest filter)
        {
            var response = new TicketListResponse
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTickets", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Status", (object?)filter.Status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Priority", (object?)filter.Priority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TicketType", (object?)filter.TicketType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AssignedTo", (object?)filter.AssignedTo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)filter.CreatedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SearchTerm", (object?)filter.SearchTerm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PageNumber", filter.PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", filter.PageSize);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // First result set: Tickets
                while (await reader.ReadAsync())
                {
                    response.Tickets.Add(MapTicketListItemFromReader(reader));
                }

                // Second result set: Total count
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    response.TotalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tickets with filter");
                throw;
            }

            return response;
        }

        /// <summary>
        /// Update ticket details
        /// </summary>
        public async Task<TicketOperationResult> UpdateTicketAsync(UpdateTicketRequest request, int updatedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_UpdateTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", request.TicketId);
                cmd.Parameters.AddWithValue("@Title", request.Title);
                cmd.Parameters.AddWithValue("@Description", request.Description);
                cmd.Parameters.AddWithValue("@TicketType", request.TicketType);
                cmd.Parameters.AddWithValue("@Priority", request.Priority);
                cmd.Parameters.AddWithValue("@DueDate", (object?)request.DueDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StepsToReproduce", (object?)request.StepsToReproduce ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectedResult", (object?)request.ExpectedResult ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActualResult", (object?)request.ActualResult ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Environment", (object?)request.Environment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TicketOperationResult
                    {
                        Success = reader.GetInt32(reader.GetOrdinal("Success")) == 1,
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };
                }

                return TicketOperationResult.FailureResult("No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket: {TicketId}", request.TicketId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        /// <summary>
        /// Delete ticket (soft delete)
        /// </summary>
        public async Task<TicketOperationResult> DeleteTicketAsync(int ticketId, int deletedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_DeleteTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TicketOperationResult
                    {
                        Success = reader.GetInt32(reader.GetOrdinal("Success")) == 1,
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };
                }

                return TicketOperationResult.FailureResult("No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket: {TicketId}", ticketId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        #endregion

        #region STATUS OPERATIONS

        /// <summary>
        /// Update ticket status
        /// </summary>
        public async Task<TicketOperationResult> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, int changedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_UpdateTicketStatus", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", request.TicketId);
                cmd.Parameters.AddWithValue("@NewStatus", request.NewStatus);
                cmd.Parameters.AddWithValue("@ChangedBy", changedBy);
                cmd.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var result = new TicketOperationResult
                    {
                        Success = reader.GetInt32(reader.GetOrdinal("Success")) == 1,
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };

                    if (result.Success)
                    {
                        _logger.LogInformation("Ticket {TicketId} status changed to {Status} by User {UserId}",
                            request.TicketId, request.NewStatus, changedBy);
                    }

                    return result;
                }

                return TicketOperationResult.FailureResult("No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket status: {TicketId}", request.TicketId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        #endregion

        #region ASSIGNMENT OPERATIONS

        /// <summary>
        /// Assign or reassign ticket to developer
        /// </summary>
        public async Task<TicketOperationResult> AssignTicketAsync(AssignTicketRequest request, int assignedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AssignTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", request.TicketId);
                cmd.Parameters.AddWithValue("@AssignedTo", request.AssignedTo);
                cmd.Parameters.AddWithValue("@AssignedBy", assignedBy);
                cmd.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var result = new TicketOperationResult
                    {
                        Success = reader.GetInt32(reader.GetOrdinal("Success")) == 1,
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };

                    if (result.Success)
                    {
                        _logger.LogInformation("Ticket {TicketId} assigned to User {AssignedTo} by User {AssignedBy}",
                            request.TicketId, request.AssignedTo, assignedBy);
                    }

                    return result;
                }

                return TicketOperationResult.FailureResult("No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning ticket: {TicketId}", request.TicketId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        #endregion

        #region COMMENT OPERATIONS

        /// <summary>
        /// Add comment to ticket
        /// </summary>
        public async Task<AddCommentResponse> AddCommentAsync(AddCommentRequest request, int userId)
        {
            var response = new AddCommentResponse();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AddTicketComment", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", request.TicketId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Comment", request.Comment);

                var commentIdParam = new SqlParameter("@CommentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(commentIdParam);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var success = reader.GetInt32(reader.GetOrdinal("Success")) == 1;
                    response.Message = reader.GetString(reader.GetOrdinal("Message"));

                    if (success)
                    {
                        response.CommentId = (int)commentIdParam.Value;
                        _logger.LogInformation("Comment added to Ticket {TicketId} by User {UserId}", request.TicketId, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to ticket: {TicketId}", request.TicketId);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get all comments for a ticket
        /// </summary>
        public async Task<List<TicketComment>> GetTicketCommentsAsync(int ticketId)
        {
            var comments = new List<TicketComment>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketComments", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    comments.Add(new TicketComment
                    {
                        CommentId = reader.GetInt32(reader.GetOrdinal("CommentId")),
                        TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                        Comment = reader.GetString(reader.GetOrdinal("Comment")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        UserName = reader["UserName"]?.ToString(),
                        UserEmail = reader["UserEmail"]?.ToString(),
                        ProfilePicture = reader["ProfilePicture"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for ticket: {TicketId}", ticketId);
                throw;
            }

            return comments;
        }

        /// <summary>
        /// Delete comment (soft delete)
        /// </summary>
        public async Task<TicketOperationResult> DeleteCommentAsync(int commentId, int deletedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand(@"
                    UPDATE TicketComments 
                    SET IsDeleted = 1 
                    WHERE CommentId = @CommentId", conn);

                cmd.Parameters.AddWithValue("@CommentId", commentId);

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0
                    ? TicketOperationResult.SuccessResult("Comment deleted successfully")
                    : TicketOperationResult.FailureResult("Comment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment: {CommentId}", commentId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        #endregion

        #region ATTACHMENT OPERATIONS

        /// <summary>
        /// Add attachment to ticket
        /// </summary>
        public async Task<AddAttachmentResponse> AddAttachmentAsync(AddAttachmentRequest request, int uploadedBy)
        {
            var response = new AddAttachmentResponse();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AddTicketAttachment", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TicketId", request.TicketId);
                cmd.Parameters.AddWithValue("@FileName", request.FileName);
                cmd.Parameters.AddWithValue("@FilePath", request.FilePath);
                cmd.Parameters.AddWithValue("@FileSize", request.FileSize);
                cmd.Parameters.AddWithValue("@FileType", (object?)request.FileType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UploadedBy", uploadedBy);

                var attachmentIdParam = new SqlParameter("@AttachmentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(attachmentIdParam);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var success = reader.GetInt32(reader.GetOrdinal("Success")) == 1;
                    response.Message = reader.GetString(reader.GetOrdinal("Message"));

                    if (success)
                    {
                        response.AttachmentId = (int)attachmentIdParam.Value;
                        _logger.LogInformation("Attachment added to Ticket {TicketId}: {FileName}", request.TicketId, request.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment to ticket: {TicketId}", request.TicketId);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get all attachments for a ticket
        /// </summary>
        public async Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId)
        {
            var attachments = new List<TicketAttachment>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketAttachments", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    attachments.Add(new TicketAttachment
                    {
                        AttachmentId = reader.GetInt32(reader.GetOrdinal("AttachmentId")),
                        TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                        FileName = reader.GetString(reader.GetOrdinal("FileName")),
                        FilePath = reader.GetString(reader.GetOrdinal("FilePath")),
                        FileSize = reader.GetInt64(reader.GetOrdinal("FileSize")),
                        FileType = reader["FileType"]?.ToString(),
                        UploadedDate = reader.GetDateTime(reader.GetOrdinal("UploadedDate")),
                        UploadedBy = reader.GetInt32(reader.GetOrdinal("UploadedBy")),
                        UploadedByName = reader["UploadedByName"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attachments for ticket: {TicketId}", ticketId);
                throw;
            }

            return attachments;
        }

        /// <summary>
        /// Get attachment by ID
        /// </summary>
        public async Task<TicketAttachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand(@"
                    SELECT a.*, u.FullName AS UploadedByName
                    FROM TicketAttachments a
                    INNER JOIN Users u ON a.UploadedBy = u.Id
                    WHERE a.AttachmentId = @AttachmentId AND a.IsDeleted = 0", conn);

                cmd.Parameters.AddWithValue("@AttachmentId", attachmentId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TicketAttachment
                    {
                        AttachmentId = reader.GetInt32(reader.GetOrdinal("AttachmentId")),
                        TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                        FileName = reader.GetString(reader.GetOrdinal("FileName")),
                        FilePath = reader.GetString(reader.GetOrdinal("FilePath")),
                        FileSize = reader.GetInt64(reader.GetOrdinal("FileSize")),
                        FileType = reader["FileType"]?.ToString(),
                        UploadedDate = reader.GetDateTime(reader.GetOrdinal("UploadedDate")),
                        UploadedBy = reader.GetInt32(reader.GetOrdinal("UploadedBy")),
                        UploadedByName = reader["UploadedByName"]?.ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attachment: {AttachmentId}", attachmentId);
                throw;
            }
        }

        /// <summary>
        /// Delete attachment (soft delete)
        /// </summary>
        public async Task<TicketOperationResult> DeleteAttachmentAsync(int attachmentId, int deletedBy)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_DeleteTicketAttachment", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AttachmentId", attachmentId);
                cmd.Parameters.AddWithValue("@DeletedBy", deletedBy);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TicketOperationResult
                    {
                        Success = reader.GetInt32(reader.GetOrdinal("Success")) == 1,
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };
                }

                return TicketOperationResult.FailureResult("No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attachment: {AttachmentId}", attachmentId);
                return TicketOperationResult.FailureResult(ex.Message);
            }
        }

        #endregion

        #region HISTORY OPERATIONS

        /// <summary>
        /// Get ticket history (audit trail)
        /// </summary>
        public async Task<List<TicketHistory>> GetTicketHistoryAsync(int ticketId)
        {
            var history = new List<TicketHistory>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketHistory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    history.Add(new TicketHistory
                    {
                        HistoryId = reader.GetInt32(reader.GetOrdinal("HistoryId")),
                        TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                        ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                        OldValue = reader["OldValue"]?.ToString(),
                        NewValue = reader["NewValue"]?.ToString(),
                        Remarks = reader["Remarks"]?.ToString(),
                        ChangeDate = reader.GetDateTime(reader.GetOrdinal("ChangeDate")),
                        ChangedByName = reader["ChangedByName"]?.ToString(),
                        ChangedByEmail = reader["ChangedByEmail"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history for ticket: {TicketId}", ticketId);
                throw;
            }

            return history;
        }

        #endregion

        #region DASHBOARD & REPORTS

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public async Task<TicketDashboard> GetDashboardAsync(int? userId = null, string? userRole = null)
        {
            var dashboard = new TicketDashboard();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketDashboard", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", (object?)userRole ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // ✅ First result set: Stats (Direct properties, NO Stats wrapper)
                if (await reader.ReadAsync())
                {
                    dashboard.TotalTickets = SafeGetInt(reader, "TotalTickets");
                    dashboard.NewTickets = SafeGetInt(reader, "NewTickets");
                    dashboard.AssignedTickets = SafeGetInt(reader, "AssignedTickets");
                    dashboard.InProgressTickets = SafeGetInt(reader, "InProgressTickets");
                    dashboard.BlockedTickets = SafeGetInt(reader, "BlockedTickets");
                    dashboard.ResolvedTickets = SafeGetInt(reader, "ResolvedTickets");
                    dashboard.ClosedTickets = SafeGetInt(reader, "ClosedTickets");
                    dashboard.ReopenedTickets = SafeGetInt(reader, "ReopenedTickets");
                    dashboard.CriticalTickets = SafeGetInt(reader, "CriticalTickets");
                    dashboard.HighPriorityTickets = SafeGetInt(reader, "HighPriorityTickets");
                    dashboard.OverdueTickets = SafeGetInt(reader, "OverdueTickets");
                }

                // ✅ Second: Priority Distribution
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.PriorityDistribution.Add(new PriorityDistribution
                        {
                            Priority = reader["Priority"]?.ToString() ?? "",
                            Count = SafeGetInt(reader, "Count")
                        });
                    }
                }

                // ✅ Third: Type Distribution
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.TypeDistribution.Add(new TypeDistribution
                        {
                            TicketType = reader["TicketType"]?.ToString() ?? "",
                            Count = SafeGetInt(reader, "Count")
                        });
                    }
                }

                // ✅ Fourth: Recent Activities
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dashboard.RecentActivities.Add(new RecentTicketActivity
                        {
                            TicketId = SafeGetInt(reader, "TicketId"),
                            TicketNumber = reader["TicketNumber"]?.ToString() ?? "",
                            Title = reader["Title"]?.ToString() ?? "",
                            Status = reader["Status"]?.ToString() ?? "",
                            Priority = reader["Priority"]?.ToString() ?? "",
                            UpdatedDate = SafeGetDateTime(reader, "UpdatedDate"),
                            CreatedByName = reader["CreatedByName"]?.ToString(),
                            AssignedToName = reader["AssignedToName"]?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard");
                // Don't throw - return empty dashboard
            }

            return dashboard;
        }

        // ✅ Helper methods for safe reading
        private static int SafeGetInt(SqlDataReader reader, string column)
        {
            try
            {
                var ordinal = reader.GetOrdinal(column);
                if (reader.IsDBNull(ordinal)) return 0;

                var value = reader.GetValue(ordinal);
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        private static DateTime SafeGetDateTime(SqlDataReader reader, string column)
        {
            try
            {
                var ordinal = reader.GetOrdinal(column);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Get my tickets (based on role)
        /// </summary>
        public async Task<List<TicketListItem>> GetMyTicketsAsync(MyTicketsRequest request)
        {
            var tickets = new List<TicketListItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetMyTickets", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", request.UserId);
                cmd.Parameters.AddWithValue("@UserRole", request.UserRole);
                cmd.Parameters.AddWithValue("@Status", (object?)request.Status ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tickets.Add(MapTicketListItemFromReader(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my tickets for User: {UserId}", request.UserId);
                throw;
            }

            return tickets;
        }

        /// <summary>
        /// Update overdue tickets
        /// </summary>
        public async Task<int> UpdateOverdueTicketsAsync()
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_UpdateOverdueTickets", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var updatedCount = reader.GetInt32(reader.GetOrdinal("UpdatedCount"));
                    _logger.LogInformation("Updated {Count} overdue tickets", updatedCount);
                    return updatedCount;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating overdue tickets");
                throw;
            }
        }

        #endregion

        #region DROPDOWN DATA

        /// <summary>
        /// Get dropdown data for ticket forms
        /// </summary>
        public async Task<TicketDropdowns> GetDropdownsAsync()
        {
            var dropdowns = new TicketDropdowns
            {
                Statuses = new List<string>(TicketStatusValues.AllStatuses),
                Priorities = new List<string>(TicketPriorityValues.AllPriorities),
                TicketTypes = new List<string>(TicketTypeValues.AllTypes)
            };

            dropdowns.Developers = await GetDevelopersAsync();
            dropdowns.QAUsers = await GetQAUsersAsync();

            return dropdowns;
        }

        /// <summary>
        /// Get developers list for assignment
        /// </summary>
        public async Task<List<UserDropdownItem>> GetDevelopersAsync()
        {
            var developers = new List<UserDropdownItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand(@"
                    SELECT u.Id AS UserId, u.FullName, u.Email, r.RoleName AS Role
                    FROM Users u
                    INNER JOIN UserRoles ur ON u.Id = ur.UserId
                    INNER JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE u.IsActive = 1 AND u.IsDeleted = 0
                      AND r.RoleName IN ('Developer', 'Admin', 'Lead')
                    ORDER BY u.FullName", conn);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    developers.Add(new UserDropdownItem
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader["Role"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting developers list");
                throw;
            }

            return developers;
        }

        /// <summary>
        /// Get QA users list
        /// </summary>
        public async Task<List<UserDropdownItem>> GetQAUsersAsync()
        {
            var qaUsers = new List<UserDropdownItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand(@"
                    SELECT u.Id AS UserId, u.FullName, u.Email, r.RoleName AS Role
                    FROM Users u
                    INNER JOIN UserRoles ur ON u.Id = ur.UserId
                    INNER JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE u.IsActive = 1 AND u.IsDeleted = 0
                      AND r.RoleName IN ('QA', 'Admin')
                    ORDER BY u.FullName", conn);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    qaUsers.Add(new UserDropdownItem
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader["Role"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QA users list");
                throw;
            }

            return qaUsers;
        }

        #endregion

        #region HELPER METHODS

        /// <summary>
        /// Map reader to Ticket object
        /// </summary>
        private Models.Ticket.Ticket MapTicketFromReader(SqlDataReader reader)
        {
            return new Models.Ticket.Ticket
            {
                TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                TicketNumber = reader.GetString(reader.GetOrdinal("TicketNumber")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                TicketType = reader.GetString(reader.GetOrdinal("TicketType")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                ResolvedDate = reader.IsDBNull(reader.GetOrdinal("ResolvedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ResolvedDate")),
                ClosedDate = reader.IsDBNull(reader.GetOrdinal("ClosedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ClosedDate")),
                StepsToReproduce = reader["StepsToReproduce"]?.ToString(),
                ExpectedResult = reader["ExpectedResult"]?.ToString(),
                ActualResult = reader["ActualResult"]?.ToString(),
                Environment = reader["Environment"]?.ToString(),
                IsOverdue = reader.GetBoolean(reader.GetOrdinal("IsOverdue")),
                CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreatedByName = reader["CreatedByName"]?.ToString(),
                CreatedByEmail = reader["CreatedByEmail"]?.ToString(),
                AssignedTo = reader.IsDBNull(reader.GetOrdinal("AssignedTo")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTo")),
                AssignedToName = reader["AssignedToName"]?.ToString(),
                AssignedToEmail = reader["AssignedToEmail"]?.ToString()
            };
        }

        /// <summary>
        /// Map reader to TicketListItem object
        /// </summary>
        private TicketListItem MapTicketListItemFromReader(SqlDataReader reader)
        {
            return new TicketListItem
            {
                TicketId = reader.GetInt32(reader.GetOrdinal("TicketId")),
                TicketNumber = reader.GetString(reader.GetOrdinal("TicketNumber")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                TicketType = reader.GetString(reader.GetOrdinal("TicketType")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                IsOverdue = reader.GetBoolean(reader.GetOrdinal("IsOverdue")),
                CreatedByName = reader["CreatedByName"]?.ToString(),
                AssignedToName = reader["AssignedToName"]?.ToString(),
                CommentCount = reader.GetInt32(reader.GetOrdinal("CommentCount")),
                AttachmentCount = reader.GetInt32(reader.GetOrdinal("AttachmentCount"))
            };
        }
        #region EMAIL RECIPIENT METHODS

        /// <summary>
        /// Get recipients for ticket email notifications
        /// </summary>
      
        #region NEW: DYNAMIC USER METHODS

        /// <summary>
        /// Get users by roles dynamically (comma-separated role names)
        /// </summary>
        public async Task<List<UserDropdownItem>> GetUsersByRolesAsync(string roleNames, int? departmentId = null)
        {
            var users = new List<UserDropdownItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketUsersByRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleNames", (object?)roleNames ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IncludeInactive", false);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new UserDropdownItem
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        FullName = reader["FullName"]?.ToString() ?? "",
                        Email = reader["Email"]?.ToString() ?? "",
                        Role = reader["Roles"]?.ToString(),
                        DepartmentName = reader["DepartmentName"]?.ToString(),
                        DesignationName = reader["DesignationName"]?.ToString(),
                        EmployeeCode = reader["EmployeeCode"]?.ToString(),
                        ProfilePicture = reader["ProfilePicture"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by roles: {Roles}", roleNames);
            }

            return users;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        public async Task<List<RoleItem>> GetAllRolesAsync()
        {
            var roles = new List<RoleItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new RoleItem
                    {
                        RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                        RoleName = reader["RoleName"]?.ToString() ?? "",
                        RoleDescription = reader["RoleDescription"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
            }

            return roles;
        }

        /// <summary>
        /// Get all departments
        /// </summary>
        public async Task<List<DepartmentItem>> GetAllDepartmentsAsync()
        {
            var depts = new List<DepartmentItem>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAllDepartments", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    depts.Add(new DepartmentItem
                    {
                        DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        DepartmentName = reader["DepartmentName"]?.ToString() ?? "",
                        DepartmentCode = reader["DepartmentCode"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting departments");
            }

            return depts;
        }

        #endregion

        #region NEW: EMAIL RECIPIENTS

        /// <summary>
        /// Get recipients for ticket email notifications
        /// </summary>
        public async Task<List<TicketEmailRecipient>> GetEmailRecipientsAsync(int ticketId, string eventType)
        {
            var recipients = new List<TicketEmailRecipient>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetTicketNotificationRecipients", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@EventType", eventType);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    recipients.Add(new TicketEmailRecipient
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        FullName = reader["FullName"]?.ToString() ?? "",
                        Email = reader["Email"]?.ToString() ?? "",
                        NotificationType = reader["NotificationType"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting email recipients for ticket {TicketId}, event {EventType}",
                    ticketId, eventType);
            }

            return recipients;
        }

        #endregion

        #endregion

        #endregion
    }

}
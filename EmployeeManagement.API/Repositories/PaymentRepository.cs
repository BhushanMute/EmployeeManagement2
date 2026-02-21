using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public readonly string _connectionString;

        public PaymentRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Payment> CreatePaymentAsync(PaymentRequest request, string orderId, string transactionId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CreatePayment", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", request.EmployeeId);
            cmd.Parameters.AddWithValue("@Amount", request.Amount);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@TransactionId", transactionId);
            cmd.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentMethod", request.PaymentMethod ?? "Card");
            cmd.Parameters.AddWithValue("@PaymentStatus", "Pending");

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Payment
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    TransactionId = reader["TransactionId"].ToString(),
                    OrderId = reader["OrderId"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                };
            }

            return null;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetPaymentById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapPayment(reader);
            }

            return null;
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(string orderId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetPaymentByOrderId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapPayment(reader);
            }

            return null;
        }

        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetPaymentByTransactionId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransactionId", transactionId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapPayment(reader);
            }

            return null;
        }

        public async Task<List<Payment>> GetPaymentsByEmployeeAsync(int employeeId)
        {
            var payments = new List<Payment>();
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetPaymentsByEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                payments.Add(MapPayment(reader));
            }

            return payments;
        }

        public async Task UpdatePaymentStatusAsync(int id, string status, string transactionId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_UpdatePaymentStatus", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@PaymentStatus", status);
            cmd.Parameters.AddWithValue("@TransactionId", transactionId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            var payments = new List<Payment>();
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetPendingPayments", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                payments.Add(MapPayment(reader));
            }

            return payments;
        }

        public async Task<decimal> GetTotalPaymentsByEmployeeAsync(int employeeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_GetTotalPaymentsByEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        private Payment MapPayment(SqlDataReader reader)
        {
            return new Payment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                TransactionId = reader["TransactionId"].ToString(),
                OrderId = reader["OrderId"].ToString(),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                PaymentMethod = reader["PaymentMethod"].ToString(),
                Currency = reader["Currency"].ToString(),
                Description = reader["Description"]?.ToString(),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CompletedDate = reader["CompletedDate"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("CompletedDate")) : null
            };
        }
    }
} 
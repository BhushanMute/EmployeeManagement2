using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;

namespace EmployeeManagement.API.Services.Interfaces
{
    public interface ISalarySlipService
    {
        Task<ApiResponse<SalarySlipResponse>> GetSalarySlipAsync(int slipId);
        Task<ApiResponse<List<SalarySlipResponse>>> GetEmployeeSalarySlipsAsync(int employeeId, int? year, int? month);
        Task<ApiResponse<bool>> GenerateSalarySlipsAsync(GenerateSalarySlipRequest request, int generatedBy);
        Task<ApiResponse<bool>> SendSalarySlipEmailAsync(SendSalarySlipEmailRequest request);
        Task<ApiResponse<bool>> SendBulkSalarySlipEmailAsync(SendBulkSalarySlipEmailRequest request);
        Task<ApiResponse<bool>> TrackViewAsync(int slipId);
        Task<ApiResponse<bool>> TrackDownloadAsync(int slipId);
        ApiResponse<string> GetReportUrl(int slipId);
    }
}

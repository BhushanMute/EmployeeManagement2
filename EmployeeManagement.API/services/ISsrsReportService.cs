namespace EmployeeManagement.API.services
{
    public interface ISsrsReportService
    {
        Task<byte[]> GenerateSalarySlipPdfAsync(int employeeId, int month, int year, int payrollProcessId);


    }
}

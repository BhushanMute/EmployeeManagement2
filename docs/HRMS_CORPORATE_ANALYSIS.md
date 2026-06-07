# HRMS Corporate Upgrade Analysis

Date: 2026-06-05

This note starts the corporate-level review from the pasted master prompt. It is intentionally practical: first understand the current implementation, then improve it module by module without blindly rewriting the project.

## Current Project Map

- API project: `EmployeeManagement.API`
- MVC UI project: `EmployeeManagement.UI`
- Main data access style: Dapper and ADO.NET with SQL Server stored procedures
- Existing API modules: auth, profile, employee, department, user management, permissions, audit log, leave, student, payment, payroll, salary structure, loan, ticket, reports, error logs
- Existing UI modules: account, employee, user management, audit log, leave, student, payment, payroll, salary structure, salary slip, ticket, reports, diagnostics
- Existing SQL script folder: `EmployeeManagement.API/SQl_Script`

## Immediate Findings

1. API startup was blocked by invalid `appsettings.json`.
   - Fixed malformed JSON around `EmailSettings`.
   - API now builds and runs.

2. API project file included compiled `bin/Debug/net8.0` files as project items.
   - Fixed by removing those generated-file entries from `EmployeeManagement.API.csproj`.
   - Reason: compiled outputs should not be source inputs.

3. UI salary slip flow exists, but API salary slip endpoints are missing.
   - UI calls endpoints like `api/SalarySlip/{id}`, `api/SalarySlip/generate`, and `api/SalarySlip/send-bulk-email`.
   - API has payroll/payslip models and repository methods, but no `SalarySlipController`.
   - This is the next main backend gap.

4. Payroll API controllers existed but several service methods threw `NotImplementedException`.
   - Started fixing this by wiring report/register/dashboard methods in `PayrollService` to the existing repository.

5. Database is not currently created on the checked SQL Server instance.
   - SQL Server is reachable.
   - `EmployeeDb` was not found.
   - Existing scripts can create it, but applying them changes local SQL Server state and should be done deliberately.

6. Solution file issue.
   - `dotnet build EmployeeManagement.slnx` is not supported by the installed SDK/MSBuild on this machine.
   - Building individual projects works.

## Salary Slip / Payroll Status

Existing pieces:

- UI salary slip screens exist.
- API models exist:
  - `SalarySlip`
  - `SalarySlipResponse`
  - `GenerateSalarySlipRequest`
  - `SendSalarySlipEmailRequest`
  - `PayslipData`
  - `SlipComponentResponse`
- Payroll repository already has payslip-related methods:
  - `GetPayslipDataAsync`
  - `LogPayslipGenerationAsync`
- Payroll processing, register, bank transfer, department-wise salary, statutory report models exist.

Missing or incomplete pieces:

- API `SalarySlipController`
- Salary slip service layer
- Salary slip repository contract/table/SP alignment
- Email log table and retry flow
- PDF/SSRS service abstraction
- SSRS settings in appsettings
- Confirmed SQL stored procedures for salary slip generation and email tracking
- End-to-end DB setup on current SQL Server

## Recommended Salary Slip Flow

1. Payroll cycle create hota hai.
2. Salary structure employee ko assign hoti hai.
3. Attendance, leave, overtime, loan/advance data collect hota hai.
4. Payroll process hota hai employee-wise.
5. Payroll approve/lock hota hai.
6. Salary slip records generate hote hain.
7. SSRS report `EmployeeId + Month + Year` ya `SlipId` se render hota hai.
8. PDF download/preview available hota hai.
9. Admin single ya bulk email send karta hai.
10. Email status table me `Pending`, `Sent`, `Failed` track hota hai.
11. Failed emails retry kiye ja sakte hain.

## Proposed Database Objects For Salary Slip

- `SalarySlips`
- `SalarySlipComponents`
- `SalarySlipEmailLogs`
- `SalarySlipDownloadLogs`
- `SalarySlipViewLogs`

Core stored procedures:

- `sp_GenerateSalarySlipsForCycle`
- `sp_GetSalarySlipById`
- `sp_GetEmployeeSalarySlips`
- `sp_GetSalarySlipsByCycle`
- `sp_GetSalarySlipForSSRS`
- `sp_LogSalarySlipEmail`
- `sp_UpdateSalarySlipEmailStatus`
- `sp_TrackSalarySlipView`
- `sp_TrackSalarySlipDownload`

## Role Access Review

Existing:

- Roles, permissions, user roles, role permissions appear in scripts and repository names.
- API uses policies like `Payroll.ViewCycle`, `Payroll.CreateCycle`, etc.

Gaps:

- Need one consistent permission naming convention.
- Need button-level permission exposed to UI.
- Need menu access response based on claims/permissions.
- Need unauthorized page/JSON behavior documented and consistent.

Recommended permission format:

- `Module.Action`
- Examples:
  - `Employee.View`
  - `Employee.Create`
  - `Payroll.ProcessPayroll`
  - `SalarySlip.SendEmail`
  - `Ticket.Assign`

## Ticket Module Review

Existing:

- API and UI ticket controllers exist.
- Repository and models exist.

Needs review:

- Confirm ticket status transitions.
- Confirm QA/developer role permissions.
- Confirm ticket attachment validation.
- Add escalation/audit logic if not present.

## Corporate Architecture Recommendations

Current project is workable, but should be made more consistent:

- Keep controllers thin.
- Put business rules in services.
- Put stored procedure calls only in repositories.
- Add service interfaces for missing modules such as salary slip and SSRS.
- Add common result handling and validation utilities.
- Avoid duplicate service registrations and duplicate `using` directives.
- Do not keep generated `bin`/`obj` files in project files.
- Add DB setup instructions for local development.

## Next Implementation Slices

1. Add API `SalarySlipController` and `ISalarySlipService`.
2. Add salary slip repository methods and align them to SQL stored procedures.
3. Add salary slip email log table and stored procedures.
4. Add SSRS settings and `ISSRSReportService`.
5. Build admin bulk salary slip email flow.
6. Add role/permission endpoint for menu and button permissions.
7. Harden file upload and email configuration.
8. Add dashboard metrics for payroll, tickets, users, departments.

## Teaching Notes

- Controller ka kaam request lena aur response dena hai.
- Service ka kaam business decision lena hai.
- Repository ka kaam database se baat karna hai.
- Stored procedure me SQL transaction and calculation logic rakhenge jahan payroll jaise business-heavy operation ho.
- SSRS report ko direct UI me hardcode nahi karna chahiye; ek service banani chahiye jo report URL/PDF bytes handle kare.

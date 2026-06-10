USE [master]
GO
/****** Object:  Database [EmployeeDb]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE DATABASE [EmployeeDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'EmployeeDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\EmployeeDb.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'EmployeeDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\EmployeeDb_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [EmployeeDb] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EmployeeDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EmployeeDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EmployeeDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EmployeeDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EmployeeDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EmployeeDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [EmployeeDb] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [EmployeeDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EmployeeDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EmployeeDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EmployeeDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EmployeeDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EmployeeDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EmployeeDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EmployeeDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EmployeeDb] SET  ENABLE_BROKER 
GO
ALTER DATABASE [EmployeeDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EmployeeDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EmployeeDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EmployeeDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EmployeeDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EmployeeDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EmployeeDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EmployeeDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [EmployeeDb] SET  MULTI_USER 
GO
ALTER DATABASE [EmployeeDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EmployeeDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EmployeeDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EmployeeDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [EmployeeDb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [EmployeeDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [EmployeeDb] SET QUERY_STORE = OFF
GO
USE [EmployeeDb]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[Class] [nvarchar](50) NOT NULL,
	[Subjects] [nvarchar](500) NULL,
	[Age] [int] NULL,
	[DateOfBirth] [date] NULL,
	[JoiningDate] [date] NOT NULL,
	[BatchTime] [nvarchar](50) NULL,
	[PassportPhotoPath] [nvarchar](500) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[Address] [nvarchar](500) NULL,
	[ParentName] [nvarchar](200) NULL,
	[ParentPhone] [nvarchar](20) NULL,
	[ParentEmail] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
	[SearchField]  AS (lower(((((([StudentId]+' ')+[FullName])+' ')+isnull([Email],''))+' ')+isnull([PhoneNumber],''))),
	[BatchCode] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Students_StudentId] UNIQUE NONCLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attendance]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attendance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttendanceId] [nvarchar](50) NOT NULL,
	[StudentId] [int] NOT NULL,
	[AttendanceDate] [date] NOT NULL,
	[AttendanceTime] [time](7) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CapturedImagePath] [nvarchar](500) NULL,
	[ConfidenceScore] [decimal](5, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[MarkedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[AttendanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Attendance_Student_Date] UNIQUE NONCLUSTERED 
(
	[StudentId] ASC,
	[AttendanceDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_AttendanceSummary]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_AttendanceSummary]
AS
SELECT 
    s.Id AS StudentId,
    s.StudentId AS StudentCode,
    s.FullName,
    s.Class,
    s.BatchTime,
    COUNT(a.Id) AS TotalDays,
    COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS TotalPresent,
    COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS TotalAbsent,
    COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS TotalLate,
    COUNT(CASE WHEN a.Status = 'Unknown' THEN 1 END) AS TotalUnknown,
    CAST(
        CASE 
            WHEN COUNT(a.Id) > 0 
            THEN (COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) * 100.0) / COUNT(a.Id)
            ELSE 0 
        END AS DECIMAL(5,2)
    ) AS AttendancePercentage,
    MAX(a.AttendanceDate) AS LastAttendanceDate
FROM Students s
LEFT JOIN Attendance a ON s.Id = a.StudentId
WHERE s.IsDeleted = 0
GROUP BY s.Id, s.StudentId, s.FullName, s.Class, s.BatchTime;
GO
/****** Object:  View [dbo].[vw_DailyAttendanceReport]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_DailyAttendanceReport]
AS
SELECT 
    a.AttendanceId,
    a.AttendanceDate,
    a.AttendanceTime,
    s.StudentId,
    s.FullName,
    s.Class,
    s.BatchTime,
    a.Status,
    a.ConfidenceScore,
    a.CapturedImagePath
FROM Attendance a
INNER JOIN Students s ON a.StudentId = s.Id
WHERE s.IsDeleted = 0;
GO
/****** Object:  View [dbo].[vw_ClasswiseAttendance]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_ClasswiseAttendance]
AS
SELECT 
    s.Class,
    COUNT(DISTINCT s.Id) AS TotalStudents,
    COUNT(a.Id) AS TotalAttendanceRecords,
    COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS TotalPresent,
    COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS TotalAbsent,
    CAST(
        CASE 
            WHEN COUNT(a.Id) > 0 
            THEN (COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) * 100.0) / COUNT(a.Id)
            ELSE 0 
        END AS DECIMAL(5,2)
    ) AS ClassAttendancePercentage
FROM Students s
LEFT JOIN Attendance a ON s.Id = a.StudentId
WHERE s.IsDeleted = 0
GROUP BY s.Class;
GO
/****** Object:  Table [dbo].[AdvanceTypes]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvanceTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdvanceTypeName] [nvarchar](100) NOT NULL,
	[AdvanceTypeCode] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[MaxAmount] [decimal](18, 2) NOT NULL,
	[MaxPercentageOfSalary] [decimal](5, 2) NULL,
	[MaxRecoveryMonths] [int] NOT NULL,
	[MinServiceMonths] [int] NOT NULL,
	[RequiresApproval] [bit] NOT NULL,
	[RecoveryStartFrom] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[AdvanceTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLogs]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Action] [nvarchar](100) NOT NULL,
	[EntityName] [nvarchar](100) NULL,
	[EntityId] [int] NULL,
	[OldValues] [nvarchar](max) NULL,
	[NewValues] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](500) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[TableName] [nvarchar](100) NULL,
	[RecordId] [int] NULL,
	[Username] [nvarchar](100) NULL,
	[Timestamp] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankTransferBatches]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankTransferBatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchNumber] [nvarchar](50) NOT NULL,
	[PayrollCycleId] [int] NOT NULL,
	[TotalEmployees] [int] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[FileGeneratedDate] [datetime] NULL,
	[FileName] [nvarchar](200) NULL,
	[FilePath] [nvarchar](500) NULL,
	[FileFormat] [nvarchar](20) NOT NULL,
	[CompanyBankAccountId] [int] NULL,
	[DebitAccountNumber] [nvarchar](50) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ProcessedDate] [datetime] NULL,
	[ProcessedBy] [int] NULL,
	[BankReferenceNo] [nvarchar](100) NULL,
	[UTRNumbers] [nvarchar](max) NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[BatchNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classes]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Capacity] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ClassName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyMaster]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](200) NOT NULL,
	[Address] [nvarchar](500) NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[LogoPath] [nvarchar](500) NULL,
	[PAN] [nvarchar](20) NULL,
	[CIN] [nvarchar](30) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Designations]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Designations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DesignationName] [nvarchar](100) NOT NULL,
	[DepartmentId] [int] NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeAdvances]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAdvances](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdvanceNumber] [nvarchar](50) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[AdvanceTypeId] [int] NOT NULL,
	[RequestedAmount] [decimal](18, 2) NOT NULL,
	[ApprovedAmount] [decimal](18, 2) NULL,
	[RecoveryMonths] [int] NOT NULL,
	[MonthlyRecoveryAmount] [decimal](18, 2) NOT NULL,
	[RequestDate] [date] NOT NULL,
	[Reason] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[RejectedBy] [int] NULL,
	[RejectedDate] [datetime] NULL,
	[RejectionReason] [nvarchar](500) NULL,
	[DisbursementDate] [date] NULL,
	[DisbursedAmount] [decimal](18, 2) NOT NULL,
	[TotalRecovered] [decimal](18, 2) NOT NULL,
	[OutstandingAmount] [decimal](18, 2) NOT NULL,
	[RecoveryStartDate] [date] NULL,
	[RecoveryEndDate] [date] NULL,
	[IsFullyRecovered] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[AdvanceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeAttendance]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAttendance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[AttendanceDate] [date] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[InTime] [time](7) NULL,
	[OutTime] [time](7) NULL,
	[OvertimeHours] [decimal](5, 2) NULL,
	[IsHoliday] [bit] NULL,
	[Remarks] [nvarchar](250) NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeBankDetails]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeBankDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[AccountHolderName] [nvarchar](200) NOT NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[BankName] [nvarchar](200) NOT NULL,
	[BranchName] [nvarchar](200) NULL,
	[IFSCCode] [nvarchar](20) NOT NULL,
	[BankCode] [nvarchar](20) NULL,
	[BranchCode] [nvarchar](20) NULL,
	[AccountType] [nvarchar](20) NOT NULL,
	[IsPrimaryAccount] [bit] NOT NULL,
	[IsVerified] [bit] NOT NULL,
	[VerifiedBy] [int] NULL,
	[VerifiedDate] [datetime] NULL,
	[VerificationMethod] [nvarchar](50) NULL,
	[ChequeAttachmentPath] [nvarchar](500) NULL,
	[EffectiveFrom] [date] NOT NULL,
	[EffectiveTo] [date] NULL,
	[IsActive] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeLoans]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeLoans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoanNumber] [nvarchar](50) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[LoanTypeId] [int] NOT NULL,
	[LoanAmount] [decimal](18, 2) NOT NULL,
	[InterestRate] [decimal](5, 2) NOT NULL,
	[TenureMonths] [int] NOT NULL,
	[EMIAmount] [decimal](18, 2) NOT NULL,
	[TotalRepayableAmount] [decimal](18, 2) NOT NULL,
	[ApplicationDate] [date] NOT NULL,
	[RequestedAmount] [decimal](18, 2) NOT NULL,
	[ApprovedAmount] [decimal](18, 2) NULL,
	[Purpose] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[ApprovalRemarks] [nvarchar](500) NULL,
	[RejectedBy] [int] NULL,
	[RejectedDate] [datetime] NULL,
	[RejectionReason] [nvarchar](500) NULL,
	[DisbursementDate] [date] NULL,
	[DisbursementMode] [nvarchar](50) NULL,
	[DisbursementReferenceNo] [nvarchar](100) NULL,
	[DisbursedBy] [int] NULL,
	[FirstEMIDate] [date] NULL,
	[LastEMIDate] [date] NULL,
	[TotalEMIsPaid] [int] NOT NULL,
	[TotalAmountPaid] [decimal](18, 2) NOT NULL,
	[PrincipalPaid] [decimal](18, 2) NOT NULL,
	[InterestPaid] [decimal](18, 2) NOT NULL,
	[OutstandingPrincipal] [decimal](18, 2) NOT NULL,
	[OutstandingInterest] [decimal](18, 2) NOT NULL,
	[OutstandingAmount] [decimal](18, 2) NOT NULL,
	[IsFullyPaid] [bit] NOT NULL,
	[ClosureDate] [date] NULL,
	[ClosureType] [nvarchar](50) NULL,
	[ClosedBy] [int] NULL,
	[GuarantorEmployeeId] [int] NULL,
	[GuarantorName] [nvarchar](200) NULL,
	[GuarantorRelation] [nvarchar](100) NULL,
	[AttachmentPath] [nvarchar](500) NULL,
	[Remarks] [nvarchar](1000) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ProcessingFee] [decimal](18, 2) NULL,
	[LastEMIPaidDate] [datetime] NULL,
	[PrepaymentAmount] [decimal](18, 2) NULL,
	[LastPrepaymentDate] [datetime] NULL,
	[ClosureRemarks] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[LoanNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeReimbursements]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeReimbursements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimNumber] [nvarchar](50) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[ReimbursementTypeId] [int] NOT NULL,
	[ClaimDate] [date] NOT NULL,
	[ClaimAmount] [decimal](18, 2) NOT NULL,
	[ApprovedAmount] [decimal](18, 2) NULL,
	[ExpenseDate] [date] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[BillNumber] [nvarchar](100) NULL,
	[VendorName] [nvarchar](200) NULL,
	[AttachmentPath] [nvarchar](500) NULL,
	[BillAttachmentPath] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[ApprovalRemarks] [nvarchar](500) NULL,
	[RejectedBy] [int] NULL,
	[RejectedDate] [datetime] NULL,
	[RejectionReason] [nvarchar](500) NULL,
	[PaymentStatus] [nvarchar](20) NOT NULL,
	[PaymentDate] [date] NULL,
	[PayrollCycleId] [int] NULL,
	[PaymentMode] [nvarchar](50) NULL,
	[PaymentReferenceNo] [nvarchar](100) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ClaimNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[Salary] [decimal](18, 2) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Address] [nvarchar](500) NULL,
	[DateOfBirth] [date] NULL,
	[JoiningDate] [date] NULL,
	[ProfileImagePath] [nvarchar](500) NULL,
	[Role] [nvarchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
	[EmployeeCode] [nvarchar](50) NULL,
	[Designation] [nvarchar](100) NULL,
	[PAN] [nvarchar](20) NULL,
	[UAN] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Employees_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeSalaryComponents]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeSalaryComponents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeSalaryStructureId] [int] NOT NULL,
	[ComponentId] [int] NOT NULL,
	[CalculationType] [nvarchar](20) NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[Percentage] [decimal](5, 2) NULL,
	[CalculationBase] [nvarchar](50) NULL,
	[FormulaExpression] [nvarchar](500) NULL,
	[ComponentType] [nvarchar](20) NOT NULL,
	[MonthlyAmount] [decimal](18, 2) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsMandatory] [bit] NOT NULL,
	[EffectiveFrom] [date] NOT NULL,
	[EffectiveTo] [date] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[AnnualAmount] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeSalaryStructure]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeSalaryStructure](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NULL,
	[CTC] [decimal](18, 2) NULL,
	[GrossSalary] [decimal](18, 2) NULL,
	[NetSalary] [decimal](18, 2) NULL,
	[EffectiveFrom] [date] NULL,
	[IsCurrentStructure] [bit] NULL,
	[RevisionNumber] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[EffectiveTo] [date] NULL,
	[TemplateId] [int] NULL,
	[RevisionReason] [nvarchar](500) NULL,
	[PreviousStructureId] [int] NULL,
	[CreatedBy] [int] NULL,
	[BasicSalary] [decimal](18, 2) NULL,
	[TotalEarnings] [decimal](18, 2) NULL,
	[TotalDeductions] [decimal](18, 2) NULL,
	[EmployerContributions] [decimal](18, 2) NULL,
	[IsActive] [bit] NULL,
	[Status] [nvarchar](50) NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[ApprovedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeTaxDeclarations]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeTaxDeclarations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[FinancialYear] [nvarchar](10) NOT NULL,
	[SelectedTaxRegime] [nvarchar](20) NOT NULL,
	[RegimeSelectionDate] [date] NULL,
	[LIC] [decimal](18, 2) NOT NULL,
	[PPF] [decimal](18, 2) NOT NULL,
	[ELSS] [decimal](18, 2) NOT NULL,
	[HomeLoanPrincipal] [decimal](18, 2) NOT NULL,
	[ChildrenTuitionFees] [decimal](18, 2) NOT NULL,
	[NSC] [decimal](18, 2) NOT NULL,
	[FD_5Year] [decimal](18, 2) NOT NULL,
	[Other80C] [decimal](18, 2) NOT NULL,
	[Total80C] [decimal](18, 2) NOT NULL,
	[HealthInsurance_Self] [decimal](18, 2) NOT NULL,
	[HealthInsurance_Parents] [decimal](18, 2) NOT NULL,
	[PreventiveHealthCheckup] [decimal](18, 2) NOT NULL,
	[Total80D] [decimal](18, 2) NOT NULL,
	[EducationLoanInterest] [decimal](18, 2) NOT NULL,
	[HomeLoanInterest] [decimal](18, 2) NOT NULL,
	[HRA_Received] [decimal](18, 2) NOT NULL,
	[Rent_Paid] [decimal](18, 2) NOT NULL,
	[LandlordPAN] [nvarchar](20) NULL,
	[IsMetroCity] [bit] NOT NULL,
	[Section80G_Donation] [decimal](18, 2) NOT NULL,
	[Section80TTA_SavingsInterest] [decimal](18, 2) NOT NULL,
	[StandardDeduction] [decimal](18, 2) NOT NULL,
	[TotalDeductions] [decimal](18, 2) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[SubmittedDate] [date] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[RejectionReason] [nvarchar](500) NULL,
	[ProofSubmitted] [bit] NOT NULL,
	[ProofSubmissionDate] [date] NULL,
	[ProofAttachmentPath] [nvarchar](500) NULL,
	[IsLocked] [bit] NOT NULL,
	[LockedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TaxDeclaration_Employee_FY] UNIQUE NONCLUSTERED 
(
	[EmployeeId] ASC,
	[FinancialYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExcelUploadHistory]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExcelUploadHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](500) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[TotalRecords] [int] NOT NULL,
	[SuccessCount] [int] NOT NULL,
	[FailedCount] [int] NOT NULL,
	[ErrorDetails] [nvarchar](max) NULL,
	[UploadedBy] [int] NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FullAndFinalSettlement]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FullAndFinalSettlement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SettlementNumber] [nvarchar](50) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[SeparationType] [nvarchar](50) NOT NULL,
	[ResignationDate] [date] NULL,
	[LastWorkingDate] [date] NOT NULL,
	[NoticePeriodDays] [int] NOT NULL,
	[NoticePeriodServed] [int] NOT NULL,
	[NoticePeriodShortfall] [int] NOT NULL,
	[LastMonthSalary] [decimal](18, 2) NOT NULL,
	[WorkingDaysInLastMonth] [int] NOT NULL,
	[ProRataSalary] [decimal](18, 2) NOT NULL,
	[UnusedLeaveBalance] [decimal](5, 2) NOT NULL,
	[LeaveEncashmentAmount] [decimal](18, 2) NOT NULL,
	[TotalServiceYears] [decimal](5, 2) NOT NULL,
	[IsEligibleForGratuity] [bit] NOT NULL,
	[GratuityAmount] [decimal](18, 2) NOT NULL,
	[NoticePeriodRecovery] [decimal](18, 2) NOT NULL,
	[ProRataBonus] [decimal](18, 2) NOT NULL,
	[LoanOutstanding] [decimal](18, 2) NOT NULL,
	[AdvanceOutstanding] [decimal](18, 2) NOT NULL,
	[OtherRecoveries] [decimal](18, 2) NOT NULL,
	[AssetRecovery] [decimal](18, 2) NOT NULL,
	[TotalEarnings] [decimal](18, 2) NOT NULL,
	[TotalDeductions] [decimal](18, 2) NOT NULL,
	[NetSettlementAmount] [decimal](18, 2) NOT NULL,
	[PaymentStatus] [nvarchar](20) NOT NULL,
	[PaymentDate] [date] NULL,
	[PaymentMode] [nvarchar](50) NULL,
	[PaymentReferenceNo] [nvarchar](100) NULL,
	[IsClearanceCompleted] [bit] NOT NULL,
	[ClearanceDate] [date] NULL,
	[SettlementLetterPath] [nvarchar](500) NULL,
	[RelievingLetterPath] [nvarchar](500) NULL,
	[ExperienceLetterPath] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[Remarks] [nvarchar](1000) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SettlementNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Holidays]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Holidays](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Date] [date] NOT NULL,
	[Day] [nvarchar](20) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Year] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LeaveApprovals]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveApprovals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeaveRequestId] [int] NOT NULL,
	[ApproverLevel] [int] NOT NULL,
	[ApproverId] [int] NOT NULL,
	[ApproverRole] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[Comments] [nvarchar](500) NULL,
	[ActionDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LeaveBalances]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveBalances](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[LeaveTypeId] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[TotalAllocated] [decimal](5, 1) NOT NULL,
	[TotalUsed] [decimal](5, 1) NOT NULL,
	[TotalPending] [decimal](5, 1) NOT NULL,
	[CarryForward] [decimal](5, 1) NOT NULL,
	[TotalAvailable]  AS ((([TotalAllocated]+[CarryForward])-[TotalUsed])-[TotalPending]),
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_LeaveBalance] UNIQUE NONCLUSTERED 
(
	[EmployeeId] ASC,
	[LeaveTypeId] ASC,
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LeaveRequests]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[LeaveTypeId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[TotalDays] [decimal](5, 1) NOT NULL,
	[Reason] [nvarchar](1000) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[IsHalfDay] [bit] NOT NULL,
	[HalfDayType] [nvarchar](20) NULL,
	[AttachmentPath] [nvarchar](500) NULL,
	[EmergencyContact] [nvarchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[AppliedDate] [datetime] NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[RejectedBy] [int] NULL,
	[RejectedDate] [datetime] NULL,
	[CancelledDate] [datetime] NULL,
	[CancelReason] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LeaveTypes]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[DefaultDays] [int] NOT NULL,
	[MaxDays] [int] NOT NULL,
	[IsCarryForward] [bit] NOT NULL,
	[MaxCarryForward] [int] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoanEMISchedule]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoanEMISchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoanId] [int] NOT NULL,
	[EMINumber] [int] NOT NULL,
	[EMIDueDate] [date] NOT NULL,
	[EMIAmount] [decimal](18, 2) NOT NULL,
	[PrincipalAmount] [decimal](18, 2) NOT NULL,
	[InterestAmount] [decimal](18, 2) NOT NULL,
	[OpeningBalance] [decimal](18, 2) NOT NULL,
	[ClosingBalance] [decimal](18, 2) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[PaymentDate] [date] NULL,
	[AmountPaid] [decimal](18, 2) NOT NULL,
	[PayrollCycleId] [int] NULL,
	[IsLatePayment] [bit] NOT NULL,
	[LateFee] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_LoanEMI] UNIQUE NONCLUSTERED 
(
	[LoanId] ASC,
	[EMINumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoanTypes]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoanTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoanTypeName] [nvarchar](100) NOT NULL,
	[LoanTypeCode] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[MinAmount] [decimal](18, 2) NOT NULL,
	[MaxAmount] [decimal](18, 2) NOT NULL,
	[MaxTenureMonths] [int] NOT NULL,
	[InterestRate] [decimal](5, 2) NOT NULL,
	[MinServiceMonths] [int] NOT NULL,
	[RequiresGuarantor] [bit] NOT NULL,
	[RequiresDocuments] [bit] NOT NULL,
	[EMICalculationType] [nvarchar](20) NOT NULL,
	[MaxEMIPercentageOfSalary] [decimal](5, 2) NULL,
	[DeductionStartFrom] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[MinTenureMonths] [int] NULL,
	[RequiresCollateral] [bit] NULL,
	[MaxLoanMultiplier] [decimal](5, 2) NULL,
	[ProcessingFeePercent] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[LoanTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasswordResetTokens]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [nvarchar](500) NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedDate] [datetime2](7) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayrollArrears]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayrollArrears](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[RevisionEffectiveDate] [date] NOT NULL,
	[RevisionApprovedDate] [date] NOT NULL,
	[OldSalaryStructureId] [int] NOT NULL,
	[NewSalaryStructureId] [int] NOT NULL,
	[ArrearsPeriodFrom] [date] NOT NULL,
	[ArrearsPeriodTo] [date] NOT NULL,
	[TotalMonths] [int] NOT NULL,
	[OldGrossSalary] [decimal](18, 2) NOT NULL,
	[NewGrossSalary] [decimal](18, 2) NOT NULL,
	[DifferencePerMonth] [decimal](18, 2) NOT NULL,
	[TotalArrearsAmount] [decimal](18, 2) NOT NULL,
	[ComponentWiseArrears] [nvarchar](max) NULL,
	[PaymentStatus] [nvarchar](20) NOT NULL,
	[PayrollCycleId] [int] NULL,
	[PaymentDate] [date] NULL,
	[CalculatedBy] [int] NULL,
	[CalculatedDate] [datetime] NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[Remarks] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayrollCycle]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayrollCycle](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CycleName] [nvarchar](100) NOT NULL,
	[CycleCode] [nvarchar](20) NOT NULL,
	[PeriodType] [nvarchar](20) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[ProcessingDate] [date] NULL,
	[SalaryCreditDate] [date] NULL,
	[FinancialYear] [nvarchar](10) NOT NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[LockedBy] [int] NULL,
	[LockedDate] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[ApprovalRemarks] [nvarchar](500) NULL,
	[TotalEmployees] [int] NOT NULL,
	[ProcessedEmployees] [int] NOT NULL,
	[TotalGrossSalary] [decimal](18, 2) NOT NULL,
	[TotalDeductions] [decimal](18, 2) NOT NULL,
	[TotalNetSalary] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CycleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayrollEmailQueue]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayrollEmailQueue](
	[QueueId] [bigint] IDENTITY(1,1) NOT NULL,
	[PayrollProcessId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[EmailAddress] [varchar](255) NOT NULL,
	[EmployeeName] [varchar](100) NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[PdfFilePath] [varchar](500) NULL,
	[Status] [varchar](20) NULL,
	[RetryCount] [int] NULL,
	[ErrorMessage] [varchar](max) NULL,
	[SentDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CycleId] [int] NULL,
	[SalarySlipId] [int] NULL,
	[EmployeeEmail] [nvarchar](200) NULL,
	[Subject] [nvarchar](255) NULL,
	[BodyHtml] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[QueueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayrollProcessing]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayrollProcessing](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PayrollCycleId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[EmployeeSalaryStructureId] [int] NOT NULL,
	[BasicSalary] [decimal](18, 2) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[TotalEarnings] [decimal](18, 2) NOT NULL,
	[TotalDeductions] [decimal](18, 2) NOT NULL,
	[NetSalary] [decimal](18, 2) NOT NULL,
	[CTC] [decimal](18, 2) NOT NULL,
	[TotalWorkingDays] [int] NOT NULL,
	[PresentDays] [decimal](5, 2) NOT NULL,
	[AbsentDays] [decimal](5, 2) NOT NULL,
	[PaidLeaveDays] [decimal](5, 2) NOT NULL,
	[UnpaidLeaveDays] [decimal](5, 2) NOT NULL,
	[WeeklyOffDays] [int] NOT NULL,
	[HolidayDays] [int] NOT NULL,
	[LOPDays] [decimal](5, 2) NOT NULL,
	[LOPAmount] [decimal](18, 2) NOT NULL,
	[OvertimeHours] [decimal](5, 2) NOT NULL,
	[OvertimeAmount] [decimal](18, 2) NOT NULL,
	[ArrearsAmount] [decimal](18, 2) NOT NULL,
	[AdjustmentAmount] [decimal](18, 2) NOT NULL,
	[BonusAmount] [decimal](18, 2) NOT NULL,
	[TotalReimbursements] [decimal](18, 2) NOT NULL,
	[PFEmployee] [decimal](18, 2) NOT NULL,
	[PFEmployer] [decimal](18, 2) NOT NULL,
	[ESIEmployee] [decimal](18, 2) NOT NULL,
	[ESIEmployer] [decimal](18, 2) NOT NULL,
	[ProfessionalTax] [decimal](18, 2) NOT NULL,
	[TDS] [decimal](18, 2) NOT NULL,
	[LoanEMI] [decimal](18, 2) NOT NULL,
	[AdvanceRecovery] [decimal](18, 2) NOT NULL,
	[PaymentMode] [nvarchar](20) NULL,
	[PaymentStatus] [nvarchar](20) NOT NULL,
	[PaymentDate] [date] NULL,
	[PaymentReferenceNo] [nvarchar](100) NULL,
	[BankAccountNumber] [nvarchar](50) NULL,
	[BankName] [nvarchar](100) NULL,
	[BankIFSC] [nvarchar](20) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[IsOnHold] [bit] NOT NULL,
	[HoldReason] [nvarchar](500) NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[CalculatedDate] [datetime] NULL,
	[LastRecalculatedDate] [datetime] NULL,
	[Remarks] [nvarchar](1000) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_PayrollProcessing_Employee_Cycle] UNIQUE NONCLUSTERED 
(
	[PayrollCycleId] ASC,
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayrollProcessingDetails]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayrollProcessingDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PayrollProcessingId] [int] NOT NULL,
	[ComponentId] [int] NOT NULL,
	[ComponentCode] [nvarchar](20) NOT NULL,
	[ComponentName] [nvarchar](100) NOT NULL,
	[ComponentType] [nvarchar](20) NOT NULL,
	[CalculationType] [nvarchar](20) NOT NULL,
	[CalculationBase] [nvarchar](50) NULL,
	[Percentage] [decimal](5, 2) NULL,
	[BaseAmount] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[IsAttendanceBased] [bit] NOT NULL,
	[AdjustedForLOP] [bit] NOT NULL,
	[OriginalAmount] [decimal](18, 2) NULL,
	[DisplayOrder] [int] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PermissionName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Module] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PermissionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProfessionalTaxSlabs]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfessionalTaxSlabs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateCode] [nvarchar](10) NOT NULL,
	[StateName] [nvarchar](100) NOT NULL,
	[SlabNumber] [int] NOT NULL,
	[MinSalary] [decimal](18, 2) NOT NULL,
	[MaxSalary] [decimal](18, 2) NOT NULL,
	[PTAmount] [decimal](18, 2) NOT NULL,
	[ApplicableMonth] [int] NULL,
	[EffectiveFrom] [date] NOT NULL,
	[EffectiveTo] [date] NULL,
	[FinancialYear] [nvarchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [nvarchar](500) NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[IpAddress] [nvarchar](50) NULL,
	[IsRevoked] [bit] NOT NULL,
	[RevokedDate] [datetime2](7) NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReimbursementTypes]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReimbursementTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReimbursementName] [nvarchar](100) NOT NULL,
	[ReimbursementCode] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[MaxAmountPerClaim] [decimal](18, 2) NULL,
	[MaxAmountPerMonth] [decimal](18, 2) NULL,
	[MaxAmountPerYear] [decimal](18, 2) NULL,
	[RequiresBill] [bit] NOT NULL,
	[RequiresApproval] [bit] NOT NULL,
	[ApprovalLevels] [int] NOT NULL,
	[IsTaxable] [bit] NOT NULL,
	[TaxExemptionLimit] [decimal](18, 2) NULL,
	[IncludeInSalarySlip] [bit] NOT NULL,
	[PaymentMode] [nvarchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReimbursementCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[PermissionId] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RolePermission] UNIQUE NONCLUSTERED 
(
	[RoleId] ASC,
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[RoleDescription] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalaryComponentMaster]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalaryComponentMaster](
	[ComponentId] [int] IDENTITY(1,1) NOT NULL,
	[ComponentCode] [varchar](50) NOT NULL,
	[ComponentName] [varchar](100) NOT NULL,
	[ComponentType] [varchar](20) NOT NULL,
	[CalculationType] [varchar](20) NOT NULL,
	[DefaultValue] [decimal](18, 2) NULL,
	[PercentageValue] [decimal](5, 2) NULL,
	[FormulaText] [varchar](500) NULL,
	[IsTaxApplicable] [bit] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ComponentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalaryComponents]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalaryComponents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ComponentCode] [nvarchar](20) NOT NULL,
	[ComponentName] [nvarchar](100) NOT NULL,
	[ComponentType] [nvarchar](20) NOT NULL,
	[Category] [nvarchar](50) NULL,
	[CalculationType] [nvarchar](20) NOT NULL,
	[CalculationBase] [nvarchar](50) NULL,
	[DefaultPercentage] [decimal](5, 2) NULL,
	[DefaultAmount] [decimal](18, 2) NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsStatutory] [bit] NOT NULL,
	[IsTaxable] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[FormulaExpression] [nvarchar](500) NULL,
	[MinAmount] [decimal](18, 2) NULL,
	[MaxAmount] [decimal](18, 2) NULL,
	[Description] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ComponentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalarySlipEmailLog]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalarySlipEmailLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SalarySlipId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[RecipientEmail] [nvarchar](255) NULL,
	[Status] [nvarchar](20) NULL,
	[SentDate] [datetime] NULL,
	[FailureReason] [nvarchar](500) NULL,
	[TriggeredBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalarySlips]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalarySlips](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SlipNumber] [nvarchar](50) NOT NULL,
	[PayrollProcessingId] [int] NOT NULL,
	[PayrollCycleId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[PayPeriodStart] [date] NOT NULL,
	[PayPeriodEnd] [date] NOT NULL,
	[PaymentDate] [date] NULL,
	[BasicSalary] [decimal](18, 2) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[TotalEarnings] [decimal](18, 2) NOT NULL,
	[TotalDeductions] [decimal](18, 2) NOT NULL,
	[NetSalary] [decimal](18, 2) NOT NULL,
	[TotalWorkingDays] [int] NOT NULL,
	[PresentDays] [decimal](5, 2) NOT NULL,
	[PaidLeaveDays] [decimal](5, 2) NOT NULL,
	[LOPDays] [decimal](5, 2) NOT NULL,
	[SSRSReportPath] [nvarchar](500) NULL,
	[SSRSReportParameters] [nvarchar](max) NULL,
	[PDFGeneratedDate] [datetime] NULL,
	[PDFFilePath] [nvarchar](500) NULL,
	[PDFFileSize] [bigint] NULL,
	[PDFGenerationStatus] [nvarchar](20) NULL,
	[PDFGenerationError] [nvarchar](1000) NULL,
	[IsDigitallySigned] [bit] NOT NULL,
	[SignedBy] [int] NULL,
	[SignedDate] [datetime] NULL,
	[DigitalSignaturePath] [nvarchar](500) NULL,
	[EmailSentDate] [datetime] NULL,
	[EmailStatus] [nvarchar](20) NULL,
	[EmailSentTo] [nvarchar](500) NULL,
	[EmailFailureReason] [nvarchar](500) NULL,
	[ViewedByEmployee] [bit] NOT NULL,
	[FirstViewedDate] [datetime] NULL,
	[ViewCount] [int] NOT NULL,
	[LastViewedDate] [datetime] NULL,
	[DownloadCount] [int] NOT NULL,
	[LastDownloadedDate] [datetime] NULL,
	[IsPasswordProtected] [bit] NOT NULL,
	[PasswordHash] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[GeneratedBy] [int] NULL,
	[GeneratedDate] [datetime] NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SlipNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalaryTemplateComponents]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalaryTemplateComponents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[ComponentId] [int] NOT NULL,
	[CalculationType] [nvarchar](20) NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[Percentage] [decimal](5, 2) NULL,
	[CalculationBase] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[FixedAmount] [decimal](18, 2) NULL,
	[MonthlyAmount] [decimal](18, 2) NULL,
	[AnnualAmount] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TemplateComponent] UNIQUE NONCLUSTERED 
(
	[TemplateId] ASC,
	[ComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalaryTemplates]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalaryTemplates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [nvarchar](100) NOT NULL,
	[TemplateCode] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[DepartmentId] [int] NULL,
	[DesignationId] [int] NULL,
	[TotalCTC] [decimal](18, 2) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[NetSalary] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedBy] [int] NULL,
	[DeletedDate] [datetime] NULL,
	[GradeLevel] [nvarchar](50) NULL,
	[TotalEarnings] [decimal](18, 2) NULL,
	[TotalDeductions] [decimal](18, 2) NULL,
	[EmployerContributions] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TemplateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SSRSReportConfigurations]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SSRSReportConfigurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportName] [nvarchar](100) NOT NULL,
	[ReportCode] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[SSRSServerUrl] [nvarchar](500) NOT NULL,
	[ReportPath] [nvarchar](500) NOT NULL,
	[ReportFileName] [nvarchar](200) NULL,
	[AuthenticationType] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](100) NULL,
	[PasswordEncrypted] [nvarchar](500) NULL,
	[DefaultParameters] [nvarchar](max) NULL,
	[DefaultExportFormat] [nvarchar](20) NOT NULL,
	[SupportedFormats] [nvarchar](200) NULL,
	[HeaderTemplate] [nvarchar](max) NULL,
	[FooterTemplate] [nvarchar](max) NULL,
	[LogoPath] [nvarchar](500) NULL,
	[WatermarkText] [nvarchar](100) NULL,
	[EmailSubjectTemplate] [nvarchar](500) NULL,
	[EmailBodyTemplate] [nvarchar](max) NULL,
	[AllowedRoles] [nvarchar](500) NULL,
	[IsPublic] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReportCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReportName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatutorySettings]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatutorySettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SettingType] [nvarchar](50) NOT NULL,
	[StateName] [nvarchar](100) NULL,
	[StateCode] [nvarchar](10) NULL,
	[EffectiveFrom] [date] NOT NULL,
	[EffectiveTo] [date] NULL,
	[FinancialYear] [nvarchar](10) NOT NULL,
	[PFEmployeePercentage] [decimal](5, 2) NULL,
	[PFEmployerPercentage] [decimal](5, 2) NULL,
	[PFWageLimit] [decimal](18, 2) NULL,
	[PFAdminCharges] [decimal](5, 2) NULL,
	[PFEDLICharges] [decimal](5, 2) NULL,
	[EPSPercentage] [decimal](5, 2) NULL,
	[ESIEmployeePercentage] [decimal](5, 2) NULL,
	[ESIEmployerPercentage] [decimal](5, 2) NULL,
	[ESIWageLimit] [decimal](18, 2) NULL,
	[PTSlabType] [nvarchar](50) NULL,
	[PTMinSalary] [decimal](18, 2) NULL,
	[PTMaxSalary] [decimal](18, 2) NULL,
	[PTAmount] [decimal](18, 2) NULL,
	[PTCalculationFormula] [nvarchar](500) NULL,
	[TDSSection] [nvarchar](50) NULL,
	[TDSPercentage] [decimal](5, 2) NULL,
	[TDSThresholdLimit] [decimal](18, 2) NULL,
	[GratuityYearsRequired] [decimal](5, 2) NULL,
	[GratuityFormula] [nvarchar](500) NULL,
	[GratuityMaxLimit] [decimal](18, 2) NULL,
	[BonusMinSalary] [decimal](18, 2) NULL,
	[BonusPercentage] [decimal](5, 2) NULL,
	[ConfigurationJson] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subjects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](100) NOT NULL,
	[SubjectCode] [nvarchar](20) NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SubjectName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TDSSlabs]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TDSSlabs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaxRegime] [nvarchar](20) NOT NULL,
	[FinancialYear] [nvarchar](10) NOT NULL,
	[SlabNumber] [int] NOT NULL,
	[MinIncome] [decimal](18, 2) NOT NULL,
	[MaxIncome] [decimal](18, 2) NOT NULL,
	[TaxPercentage] [decimal](5, 2) NOT NULL,
	[StandardDeduction] [decimal](18, 2) NULL,
	[BasicExemptionLimit] [decimal](18, 2) NULL,
	[CessPercentage] [decimal](5, 2) NOT NULL,
	[SurchargeApplicable] [bit] NOT NULL,
	[ApplicableForAge] [nvarchar](50) NULL,
	[ApplicableForGender] [nvarchar](20) NULL,
	[EffectiveFrom] [date] NOT NULL,
	[EffectiveTo] [date] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketAttachments]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketAttachments](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FilePath] [nvarchar](500) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[FileType] [nvarchar](100) NULL,
	[UploadedBy] [int] NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketComments]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketComments](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Comment] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsInternal] [bit] NOT NULL,
	[ParentCommentId] [int] NULL,
	[IsSystemGenerated] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketHistory]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketHistory](
	[HistoryId] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[ChangedBy] [int] NOT NULL,
	[ChangeType] [nvarchar](100) NOT NULL,
	[OldValue] [nvarchar](500) NULL,
	[NewValue] [nvarchar](500) NULL,
	[ChangeDate] [datetime] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[HistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tickets]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tickets](
	[TicketId] [int] IDENTITY(1,1) NOT NULL,
	[TicketNumber] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TicketType] [nvarchar](50) NOT NULL,
	[Priority] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[AssignedTo] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[DueDate] [datetime] NULL,
	[ResolvedDate] [datetime] NULL,
	[ClosedDate] [datetime] NULL,
	[StepsToReproduce] [nvarchar](max) NULL,
	[ExpectedResult] [nvarchar](max) NULL,
	[ActualResult] [nvarchar](max) NULL,
	[Environment] [nvarchar](200) NULL,
	[IsOverdue] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TicketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TicketNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketWatchers]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketWatchers](
	[WatcherId] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AddedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[WatcherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TicketId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[AssignedDate] [datetime] NULL,
	[AssignedBy] [int] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserRoles] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[PasswordSalt] [nvarchar](500) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[EmailConfirmed] [bit] NULL,
	[LastLoginDate] [datetime] NULL,
	[FailedLoginAttempts] [int] NULL,
	[LockoutEndDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[PasswordChangedDate] [datetime] NULL,
	[FullName] [nvarchar](100) NULL,
	[LastLoginIp] [nvarchar](50) NULL,
	[DateOfBirth] [date] NULL,
	[Address] [nvarchar](500) NULL,
	[ProfilePicture] [nvarchar](500) NULL,
	[RegistrationStatus] [nvarchar](20) NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[RejectionReason] [nvarchar](500) NULL,
	[PasswordResetToken] [nvarchar](500) NULL,
	[PasswordResetTokenExpiry] [datetime] NULL,
	[UserId] [int] NULL,
	[IsEmailVerified] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSettings]    Script Date: 10-06-2026 07:40:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Theme] [nvarchar](20) NULL,
	[Language] [nvarchar](10) NULL,
	[EmailNotifications] [bit] NULL,
	[TwoFactorEnabled] [bit] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserSettings_UserId] UNIQUE NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Attendance_Date]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_Attendance_Date] ON [dbo].[Attendance]
(
	[AttendanceDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Attendance_Status]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_Attendance_Status] ON [dbo].[Attendance]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Attendance_StudentId]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_Attendance_StudentId] ON [dbo].[Attendance]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_Action]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs]
(
	[Action] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_CreatedDate]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedDate] ON [dbo].[AuditLogs]
(
	[CreatedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditLogs_EntityName_EntityId]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityName_EntityId] ON [dbo].[AuditLogs]
(
	[EntityName] ASC,
	[EntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuditLogs_UserId]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BankTransfer_Payroll]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_BankTransfer_Payroll] ON [dbo].[BankTransferBatches]
(
	[PayrollCycleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BankTransfer_Status]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_BankTransfer_Status] ON [dbo].[BankTransferBatches]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Classes_ClassName]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_Classes_ClassName] ON [dbo].[Classes]
(
	[ClassName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmployeeAdvances_Employee]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmployeeAdvances_Employee] ON [dbo].[EmployeeAdvances]
(
	[EmployeeId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmployeeAdvances_Status]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmployeeAdvances_Status] ON [dbo].[EmployeeAdvances]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmpAtt_EmpId_Date]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmpAtt_EmpId_Date] ON [dbo].[EmployeeAttendance]
(
	[EmployeeId] ASC,
	[AttendanceDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_BankDetails_AccountNo]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_BankDetails_AccountNo] ON [dbo].[EmployeeBankDetails]
(
	[AccountNumber] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BankDetails_Employee]    Script Date: 10-06-2026 07:40:34 AM ******/
CREATE NONCLUSTERED INDEX [IX_BankDetails_Employee] ON [dbo].[EmployeeBankDetails]
(
	[EmployeeId] ASC,
	[IsPrimaryAccount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmployeeLoans_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmployeeLoans_Employee] ON [dbo].[EmployeeLoans]
(
	[EmployeeId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmployeeLoans_Number]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmployeeLoans_Number] ON [dbo].[EmployeeLoans]
(
	[LoanNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmployeeLoans_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmployeeLoans_Status] ON [dbo].[EmployeeLoans]
(
	[Status] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reimbursements_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Reimbursements_Employee] ON [dbo].[EmployeeReimbursements]
(
	[EmployeeId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reimbursements_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Reimbursements_Status] ON [dbo].[EmployeeReimbursements]
(
	[Status] ASC,
	[PaymentStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Employees_DepartmentId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees]
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Employees_DepartmentId_Active]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId_Active] ON [dbo].[Employees]
(
	[DepartmentId] ASC,
	[IsActive] ASC
)
INCLUDE([Name],[Email],[Salary]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Employees_Email]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_Email] ON [dbo].[Employees]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Employees_IsActive]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_IsActive] ON [dbo].[Employees]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Employees_IsDeleted]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_IsDeleted] ON [dbo].[Employees]
(
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Employees_Name]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_Name] ON [dbo].[Employees]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmpSalaryComp_Component]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmpSalaryComp_Component] ON [dbo].[EmployeeSalaryComponents]
(
	[ComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmpSalaryComp_EffectiveDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmpSalaryComp_EffectiveDate] ON [dbo].[EmployeeSalaryComponents]
(
	[EffectiveFrom] ASC,
	[EffectiveTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmpSalaryComp_Structure]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmpSalaryComp_Structure] ON [dbo].[EmployeeSalaryComponents]
(
	[EmployeeSalaryStructureId] ASC,
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TaxDeclaration_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_TaxDeclaration_Employee] ON [dbo].[EmployeeTaxDeclarations]
(
	[EmployeeId] ASC,
	[FinancialYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TaxDeclaration_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_TaxDeclaration_Status] ON [dbo].[EmployeeTaxDeclarations]
(
	[Status] ASC,
	[FinancialYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExcelUpload_Date]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExcelUpload_Date] ON [dbo].[ExcelUploadHistory]
(
	[UploadedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FnF_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_FnF_Employee] ON [dbo].[FullAndFinalSettlement]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_FnF_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_FnF_Status] ON [dbo].[FullAndFinalSettlement]
(
	[Status] ASC,
	[PaymentStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Holidays_Date]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Holidays_Date] ON [dbo].[Holidays]
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Holidays_Year]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Holidays_Year] ON [dbo].[Holidays]
(
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Holidays_Year_Active]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Holidays_Year_Active] ON [dbo].[Holidays]
(
	[Year] ASC,
	[IsActive] ASC
)
INCLUDE([Name],[Date],[Day],[Type]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveApprovals_LeaveRequestId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveApprovals_LeaveRequestId] ON [dbo].[LeaveApprovals]
(
	[LeaveRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveBalances_Employee_Year]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveBalances_Employee_Year] ON [dbo].[LeaveBalances]
(
	[EmployeeId] ASC,
	[Year] ASC
)
INCLUDE([LeaveTypeId],[TotalAllocated],[TotalUsed],[TotalPending],[CarryForward]) 
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveBalances_EmployeeYear]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveBalances_EmployeeYear] ON [dbo].[LeaveBalances]
(
	[EmployeeId] ASC,
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveRequests_AppliedDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_AppliedDate] ON [dbo].[LeaveRequests]
(
	[AppliedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveRequests_Dates]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_Dates] ON [dbo].[LeaveRequests]
(
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LeaveRequests_Dates_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_Dates_Status] ON [dbo].[LeaveRequests]
(
	[StartDate] ASC,
	[EndDate] ASC,
	[Status] ASC
)
INCLUDE([EmployeeId],[LeaveTypeId],[TotalDays]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveRequests_EmployeeId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_EmployeeId] ON [dbo].[LeaveRequests]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveRequests_EmployeeId_Year]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_EmployeeId_Year] ON [dbo].[LeaveRequests]
(
	[EmployeeId] ASC,
	[StartDate] ASC
)
INCLUDE([LeaveTypeId],[EndDate],[TotalDays],[Status],[Reason],[AppliedDate]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LeaveRequests_LeaveTypeId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_LeaveTypeId] ON [dbo].[LeaveRequests]
(
	[LeaveTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LeaveRequests_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_Status] ON [dbo].[LeaveRequests]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LeaveRequests_Status_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LeaveRequests_Status_Employee] ON [dbo].[LeaveRequests]
(
	[Status] ASC,
	[EmployeeId] ASC
)
INCLUDE([LeaveTypeId],[StartDate],[EndDate],[TotalDays],[AppliedDate]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LoanEMI_DueDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LoanEMI_DueDate] ON [dbo].[LoanEMISchedule]
(
	[EMIDueDate] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LoanEMI_Loan]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_LoanEMI_Loan] ON [dbo].[LoanEMISchedule]
(
	[LoanId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PasswordResetTokens_Token]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PasswordResetTokens_Token] ON [dbo].[PasswordResetTokens]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PasswordResetTokens_UserId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PasswordResetTokens_UserId] ON [dbo].[PasswordResetTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Arrears_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Arrears_Employee] ON [dbo].[PayrollArrears]
(
	[EmployeeId] ASC,
	[PaymentStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollCycle_Code]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollCycle_Code] ON [dbo].[PayrollCycle]
(
	[CycleCode] ASC
)
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollCycle_Period]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollCycle_Period] ON [dbo].[PayrollCycle]
(
	[StartDate] ASC,
	[EndDate] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PayrollCycle_YearMonth]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollCycle_YearMonth] ON [dbo].[PayrollCycle]
(
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollEmailQueue_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollEmailQueue_Status] ON [dbo].[PayrollEmailQueue]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollProcessing_Cycle]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollProcessing_Cycle] ON [dbo].[PayrollProcessing]
(
	[PayrollCycleId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PayrollProcessing_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollProcessing_Employee] ON [dbo].[PayrollProcessing]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollProcessing_Payment]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollProcessing_Payment] ON [dbo].[PayrollProcessing]
(
	[PaymentStatus] ASC,
	[PaymentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PayrollDetails_Component]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollDetails_Component] ON [dbo].[PayrollProcessingDetails]
(
	[ComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PayrollDetails_Processing]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PayrollDetails_Processing] ON [dbo].[PayrollProcessingDetails]
(
	[PayrollProcessingId] ASC,
	[ComponentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PTSlab_Salary]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PTSlab_Salary] ON [dbo].[ProfessionalTaxSlabs]
(
	[StateCode] ASC,
	[MinSalary] ASC,
	[MaxSalary] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PTSlab_State]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_PTSlab_State] ON [dbo].[ProfessionalTaxSlabs]
(
	[StateCode] ASC,
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_ExpiryDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_ExpiryDate] ON [dbo].[RefreshTokens]
(
	[ExpiryDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RefreshTokens_Token]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_Token] ON [dbo].[RefreshTokens]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_UserId_IsRevoked_IsUsed]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId_IsRevoked_IsUsed] ON [dbo].[RefreshTokens]
(
	[UserId] ASC,
	[IsRevoked] ASC,
	[IsUsed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalaryComponents_Code]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalaryComponents_Code] ON [dbo].[SalaryComponents]
(
	[ComponentCode] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalaryComponents_Type]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalaryComponents_Type] ON [dbo].[SalaryComponents]
(
	[ComponentType] ASC,
	[IsActive] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SalarySlips_Cycle]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalarySlips_Cycle] ON [dbo].[SalarySlips]
(
	[PayrollCycleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalarySlips_EmailStatus]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalarySlips_EmailStatus] ON [dbo].[SalarySlips]
(
	[EmailStatus] ASC
)
WHERE ([EmailStatus]='Pending')
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SalarySlips_Employee]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalarySlips_Employee] ON [dbo].[SalarySlips]
(
	[EmployeeId] ASC,
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SalarySlips_Period]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalarySlips_Period] ON [dbo].[SalarySlips]
(
	[Year] ASC,
	[Month] ASC,
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalarySlips_SlipNumber]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalarySlips_SlipNumber] ON [dbo].[SalarySlips]
(
	[SlipNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalaryTemplates_Code]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalaryTemplates_Code] ON [dbo].[SalaryTemplates]
(
	[TemplateCode] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Statutory_EffectiveDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Statutory_EffectiveDate] ON [dbo].[StatutorySettings]
(
	[EffectiveFrom] ASC,
	[EffectiveTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Statutory_State]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Statutory_State] ON [dbo].[StatutorySettings]
(
	[StateCode] ASC,
	[SettingType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Statutory_Type]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Statutory_Type] ON [dbo].[StatutorySettings]
(
	[SettingType] ASC,
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Students_Class]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Students_Class] ON [dbo].[Students]
(
	[Class] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Students_FullName]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Students_FullName] ON [dbo].[Students]
(
	[FullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Students_IsActive]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Students_IsActive] ON [dbo].[Students]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Students_IsDeleted]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Students_IsDeleted] ON [dbo].[Students]
(
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [IX_Students_Search]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Students_Search] ON [dbo].[Students]
(
	[SearchField] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Subjects_SubjectName]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Subjects_SubjectName] ON [dbo].[Subjects]
(
	[SubjectName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TicketAttachments_TicketId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_TicketAttachments_TicketId] ON [dbo].[TicketAttachments]
(
	[TicketId] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TicketComments_TicketId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_TicketComments_TicketId] ON [dbo].[TicketComments]
(
	[TicketId] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TicketHistory_TicketId]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_TicketHistory_TicketId] ON [dbo].[TicketHistory]
(
	[TicketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tickets_AssignedTo]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tickets_AssignedTo] ON [dbo].[Tickets]
(
	[AssignedTo] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tickets_CreatedBy]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tickets_CreatedBy] ON [dbo].[Tickets]
(
	[CreatedBy] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tickets_DueDate]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tickets_DueDate] ON [dbo].[Tickets]
(
	[DueDate] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tickets_Priority]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tickets_Priority] ON [dbo].[Tickets]
(
	[Priority] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tickets_Status]    Script Date: 10-06-2026 07:40:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tickets_Status] ON [dbo].[Tickets]
(
	[Status] ASC
)
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ((3)) FOR [MaxRecoveryMonths]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ((3)) FOR [MinServiceMonths]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ((1)) FOR [RequiresApproval]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ('NextMonth') FOR [RecoveryStartFrom]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AdvanceTypes] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Attendance] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AuditLogs] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AuditLogs] ADD  DEFAULT (getutcdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[BankTransferBatches] ADD  DEFAULT ((0)) FOR [TotalEmployees]
GO
ALTER TABLE [dbo].[BankTransferBatches] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[BankTransferBatches] ADD  DEFAULT ('Excel') FOR [FileFormat]
GO
ALTER TABLE [dbo].[BankTransferBatches] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[BankTransferBatches] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CompanyMaster] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[CompanyMaster] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Departments] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Departments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Designations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Designations] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((1)) FOR [RecoveryMonths]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT (getdate()) FOR [RequestDate]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((0)) FOR [DisbursedAmount]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((0)) FOR [TotalRecovered]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((0)) FOR [OutstandingAmount]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((0)) FOR [IsFullyRecovered]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeAdvances] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmployeeAttendance] ADD  DEFAULT ((0)) FOR [OvertimeHours]
GO
ALTER TABLE [dbo].[EmployeeAttendance] ADD  DEFAULT ((0)) FOR [IsHoliday]
GO
ALTER TABLE [dbo].[EmployeeAttendance] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT ('Savings') FOR [AccountType]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT ((1)) FOR [IsPrimaryAccount]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT (getdate()) FOR [EffectiveFrom]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeBankDetails] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [InterestRate]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT (getdate()) FOR [ApplicationDate]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [TotalEMIsPaid]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [TotalAmountPaid]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [PrincipalPaid]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [InterestPaid]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [OutstandingPrincipal]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [OutstandingInterest]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [OutstandingAmount]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [IsFullyPaid]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmployeeLoans] ADD  DEFAULT ((0)) FOR [PrepaymentAmount]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT (getdate()) FOR [ClaimDate]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT ('Pending') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeReimbursements] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((0)) FOR [Salary]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT ((0)) FOR [MonthlyAmount]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT ((0)) FOR [IsMandatory]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmployeeSalaryStructure] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeSalaryStructure] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ('New') FOR [SelectedTaxRegime]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [LIC]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [PPF]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [ELSS]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [HomeLoanPrincipal]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [ChildrenTuitionFees]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [NSC]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [FD_5Year]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Other80C]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Total80C]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [HealthInsurance_Self]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [HealthInsurance_Parents]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [PreventiveHealthCheckup]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Total80D]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [EducationLoanInterest]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [HomeLoanInterest]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [HRA_Received]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Rent_Paid]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [IsMetroCity]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Section80G_Donation]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [Section80TTA_SavingsInterest]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((50000)) FOR [StandardDeduction]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [TotalDeductions]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [ProofSubmitted]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ExcelUploadHistory] ADD  DEFAULT ((0)) FOR [SuccessCount]
GO
ALTER TABLE [dbo].[ExcelUploadHistory] ADD  DEFAULT ((0)) FOR [FailedCount]
GO
ALTER TABLE [dbo].[ExcelUploadHistory] ADD  DEFAULT (getdate()) FOR [UploadedDate]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [NoticePeriodDays]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [NoticePeriodServed]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [NoticePeriodShortfall]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [LastMonthSalary]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [WorkingDaysInLastMonth]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [ProRataSalary]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [UnusedLeaveBalance]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [LeaveEncashmentAmount]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [TotalServiceYears]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [IsEligibleForGratuity]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [GratuityAmount]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [NoticePeriodRecovery]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [ProRataBonus]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [LoanOutstanding]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [AdvanceOutstanding]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [OtherRecoveries]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [AssetRecovery]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [TotalEarnings]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [TotalDeductions]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [NetSettlementAmount]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ('Pending') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ((0)) FOR [IsClearanceCompleted]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Holidays] ADD  DEFAULT ('Public') FOR [Type]
GO
ALTER TABLE [dbo].[Holidays] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Holidays] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Holidays] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LeaveApprovals] ADD  DEFAULT ((1)) FOR [ApproverLevel]
GO
ALTER TABLE [dbo].[LeaveApprovals] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[LeaveApprovals] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT ((0)) FOR [TotalAllocated]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT ((0)) FOR [TotalUsed]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT ((0)) FOR [TotalPending]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT ((0)) FOR [CarryForward]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LeaveBalances] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT ((0)) FOR [IsHalfDay]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT (getdate()) FOR [AppliedDate]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LeaveRequests] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((0)) FOR [DefaultDays]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((0)) FOR [MaxDays]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((0)) FOR [IsCarryForward]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((0)) FOR [MaxCarryForward]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((1)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LeaveTypes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LoanEMISchedule] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[LoanEMISchedule] ADD  DEFAULT ((0)) FOR [AmountPaid]
GO
ALTER TABLE [dbo].[LoanEMISchedule] ADD  DEFAULT ((0)) FOR [IsLatePayment]
GO
ALTER TABLE [dbo].[LoanEMISchedule] ADD  DEFAULT ((0)) FOR [LateFee]
GO
ALTER TABLE [dbo].[LoanEMISchedule] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [MinAmount]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [InterestRate]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((6)) FOR [MinServiceMonths]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [RequiresGuarantor]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [RequiresDocuments]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ('ReducingBalance') FOR [EMICalculationType]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ('NextMonth') FOR [DeductionStartFrom]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [RequiresCollateral]
GO
ALTER TABLE [dbo].[LoanTypes] ADD  DEFAULT ((0)) FOR [ProcessingFeePercent]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[PayrollArrears] ADD  DEFAULT ('Pending') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[PayrollArrears] ADD  DEFAULT (getdate()) FOR [CalculatedDate]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [TotalEmployees]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [ProcessedEmployees]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [TotalGrossSalary]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [TotalDeductions]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((0)) FOR [TotalNetSalary]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PayrollCycle] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PayrollEmailQueue] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[PayrollEmailQueue] ADD  DEFAULT ((0)) FOR [RetryCount]
GO
ALTER TABLE [dbo].[PayrollEmailQueue] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [BasicSalary]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [GrossSalary]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [TotalEarnings]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [TotalDeductions]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [NetSalary]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [CTC]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [TotalWorkingDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [PresentDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [AbsentDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [PaidLeaveDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [UnpaidLeaveDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [WeeklyOffDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [HolidayDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [LOPDays]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [LOPAmount]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [OvertimeHours]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [OvertimeAmount]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [ArrearsAmount]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [AdjustmentAmount]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [BonusAmount]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [TotalReimbursements]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [PFEmployee]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [PFEmployer]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [ESIEmployee]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [ESIEmployer]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [ProfessionalTax]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [TDS]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [LoanEMI]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [AdvanceRecovery]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ('Pending') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT ((0)) FOR [IsOnHold]
GO
ALTER TABLE [dbo].[PayrollProcessing] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] ADD  DEFAULT ((0)) FOR [IsAttendanceBased]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] ADD  DEFAULT ((0)) FOR [AdjustedForLOP]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Permissions] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Permissions] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProfessionalTaxSlabs] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ProfessionalTaxSlabs] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RefreshTokens] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RefreshTokens] ADD  DEFAULT ((0)) FOR [IsRevoked]
GO
ALTER TABLE [dbo].[RefreshTokens] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((1)) FOR [RequiresBill]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((1)) FOR [RequiresApproval]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((1)) FOR [ApprovalLevels]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((0)) FOR [IsTaxable]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((1)) FOR [IncludeInSalarySlip]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ReimbursementTypes] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryComponentMaster] ADD  DEFAULT ((0)) FOR [IsTaxApplicable]
GO
ALTER TABLE [dbo].[SalaryComponentMaster] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalaryComponentMaster] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT ((0)) FOR [IsStatutory]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT ((1)) FOR [IsTaxable]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryComponents] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [IsDigitallySigned]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [ViewedByEmployee]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [ViewCount]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [DownloadCount]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [IsPasswordProtected]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[SalarySlips] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryTemplateComponents] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[SalaryTemplateComponents] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalaryTemplateComponents] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT ((0)) FOR [TotalCTC]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT ((0)) FOR [GrossSalary]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT ((0)) FOR [NetSalary]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalaryTemplates] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT ('Windows') FOR [AuthenticationType]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT ('PDF') FOR [DefaultExportFormat]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT ((0)) FOR [IsPublic]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[StatutorySettings] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[StatutorySettings] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[StatutorySettings] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT (getdate()) FOR [JoiningDate]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[TDSSlabs] ADD  DEFAULT ((4.00)) FOR [CessPercentage]
GO
ALTER TABLE [dbo].[TDSSlabs] ADD  DEFAULT ((0)) FOR [SurchargeApplicable]
GO
ALTER TABLE [dbo].[TDSSlabs] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TDSSlabs] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[TDSSlabs] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[TicketAttachments] ADD  DEFAULT (getdate()) FOR [UploadedDate]
GO
ALTER TABLE [dbo].[TicketAttachments] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TicketComments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[TicketComments] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TicketComments] ADD  DEFAULT ((0)) FOR [IsInternal]
GO
ALTER TABLE [dbo].[TicketComments] ADD  DEFAULT ((0)) FOR [IsSystemGenerated]
GO
ALTER TABLE [dbo].[TicketHistory] ADD  DEFAULT (getdate()) FOR [ChangeDate]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT ('New') FOR [Status]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT ((0)) FOR [IsOverdue]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TicketWatchers] ADD  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[UserRoles] ADD  DEFAULT (getdate()) FOR [AssignedDate]
GO
ALTER TABLE [dbo].[UserRoles] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [EmailConfirmed]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [FailedLoginAttempts]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('Pending') FOR [RegistrationStatus]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsEmailVerified]
GO
ALTER TABLE [dbo].[UserSettings] ADD  DEFAULT ('light') FOR [Theme]
GO
ALTER TABLE [dbo].[UserSettings] ADD  DEFAULT ('en') FOR [Language]
GO
ALTER TABLE [dbo].[UserSettings] ADD  DEFAULT ((1)) FOR [EmailNotifications]
GO
ALTER TABLE [dbo].[UserSettings] ADD  DEFAULT ((0)) FOR [TwoFactorEnabled]
GO
ALTER TABLE [dbo].[UserSettings] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Student] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[Attendance] CHECK CONSTRAINT [FK_Attendance_Student]
GO
ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Users]
GO
ALTER TABLE [dbo].[BankTransferBatches]  WITH CHECK ADD  CONSTRAINT [FK_BankTransfer_Payroll] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[BankTransferBatches] CHECK CONSTRAINT [FK_BankTransfer_Payroll]
GO
ALTER TABLE [dbo].[Designations]  WITH CHECK ADD  CONSTRAINT [FK_Designations_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[Designations] CHECK CONSTRAINT [FK_Designations_Departments]
GO
ALTER TABLE [dbo].[EmployeeAdvances]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAdvances_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeAdvances] CHECK CONSTRAINT [FK_EmployeeAdvances_Employee]
GO
ALTER TABLE [dbo].[EmployeeAdvances]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAdvances_Type] FOREIGN KEY([AdvanceTypeId])
REFERENCES [dbo].[AdvanceTypes] ([Id])
GO
ALTER TABLE [dbo].[EmployeeAdvances] CHECK CONSTRAINT [FK_EmployeeAdvances_Type]
GO
ALTER TABLE [dbo].[EmployeeAttendance]  WITH CHECK ADD  CONSTRAINT [FK_EmpAtt_Emp] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeAttendance] CHECK CONSTRAINT [FK_EmpAtt_Emp]
GO
ALTER TABLE [dbo].[EmployeeBankDetails]  WITH CHECK ADD  CONSTRAINT [FK_BankDetails_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeBankDetails] CHECK CONSTRAINT [FK_BankDetails_Employee]
GO
ALTER TABLE [dbo].[EmployeeLoans]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLoans_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeLoans] CHECK CONSTRAINT [FK_EmployeeLoans_Employee]
GO
ALTER TABLE [dbo].[EmployeeLoans]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLoans_Guarantor] FOREIGN KEY([GuarantorEmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeLoans] CHECK CONSTRAINT [FK_EmployeeLoans_Guarantor]
GO
ALTER TABLE [dbo].[EmployeeLoans]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeLoans_LoanType] FOREIGN KEY([LoanTypeId])
REFERENCES [dbo].[LoanTypes] ([Id])
GO
ALTER TABLE [dbo].[EmployeeLoans] CHECK CONSTRAINT [FK_EmployeeLoans_LoanType]
GO
ALTER TABLE [dbo].[EmployeeReimbursements]  WITH CHECK ADD  CONSTRAINT [FK_Reimbursements_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeReimbursements] CHECK CONSTRAINT [FK_Reimbursements_Employee]
GO
ALTER TABLE [dbo].[EmployeeReimbursements]  WITH CHECK ADD  CONSTRAINT [FK_Reimbursements_Payroll] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[EmployeeReimbursements] CHECK CONSTRAINT [FK_Reimbursements_Payroll]
GO
ALTER TABLE [dbo].[EmployeeReimbursements]  WITH CHECK ADD  CONSTRAINT [FK_Reimbursements_Type] FOREIGN KEY([ReimbursementTypeId])
REFERENCES [dbo].[ReimbursementTypes] ([Id])
GO
ALTER TABLE [dbo].[EmployeeReimbursements] CHECK CONSTRAINT [FK_Reimbursements_Type]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents]  WITH CHECK ADD  CONSTRAINT [FK_EmpSalaryComp_Component] FOREIGN KEY([ComponentId])
REFERENCES [dbo].[SalaryComponents] ([Id])
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] CHECK CONSTRAINT [FK_EmpSalaryComp_Component]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents]  WITH CHECK ADD  CONSTRAINT [FK_EmpSalaryComp_Structure] FOREIGN KEY([EmployeeSalaryStructureId])
REFERENCES [dbo].[EmployeeSalaryStructure] ([Id])
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] CHECK CONSTRAINT [FK_EmpSalaryComp_Structure]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations]  WITH CHECK ADD  CONSTRAINT [FK_TaxDeclaration_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] CHECK CONSTRAINT [FK_TaxDeclaration_Employee]
GO
ALTER TABLE [dbo].[ExcelUploadHistory]  WITH CHECK ADD  CONSTRAINT [FK_ExcelUpload_User] FOREIGN KEY([UploadedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ExcelUploadHistory] CHECK CONSTRAINT [FK_ExcelUpload_User]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement]  WITH CHECK ADD  CONSTRAINT [FK_FnF_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] CHECK CONSTRAINT [FK_FnF_Employee]
GO
ALTER TABLE [dbo].[LeaveApprovals]  WITH CHECK ADD  CONSTRAINT [FK_LeaveApprovals_LeaveRequests] FOREIGN KEY([LeaveRequestId])
REFERENCES [dbo].[LeaveRequests] ([Id])
GO
ALTER TABLE [dbo].[LeaveApprovals] CHECK CONSTRAINT [FK_LeaveApprovals_LeaveRequests]
GO
ALTER TABLE [dbo].[LeaveBalances]  WITH CHECK ADD  CONSTRAINT [FK_LeaveBalances_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[LeaveBalances] CHECK CONSTRAINT [FK_LeaveBalances_Employees]
GO
ALTER TABLE [dbo].[LeaveBalances]  WITH CHECK ADD  CONSTRAINT [FK_LeaveBalances_LeaveTypes] FOREIGN KEY([LeaveTypeId])
REFERENCES [dbo].[LeaveTypes] ([Id])
GO
ALTER TABLE [dbo].[LeaveBalances] CHECK CONSTRAINT [FK_LeaveBalances_LeaveTypes]
GO
ALTER TABLE [dbo].[LeaveRequests]  WITH CHECK ADD  CONSTRAINT [FK_LeaveRequests_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[LeaveRequests] CHECK CONSTRAINT [FK_LeaveRequests_Employees]
GO
ALTER TABLE [dbo].[LeaveRequests]  WITH CHECK ADD  CONSTRAINT [FK_LeaveRequests_LeaveTypes] FOREIGN KEY([LeaveTypeId])
REFERENCES [dbo].[LeaveTypes] ([Id])
GO
ALTER TABLE [dbo].[LeaveRequests] CHECK CONSTRAINT [FK_LeaveRequests_LeaveTypes]
GO
ALTER TABLE [dbo].[LoanEMISchedule]  WITH CHECK ADD  CONSTRAINT [FK_LoanEMI_Loan] FOREIGN KEY([LoanId])
REFERENCES [dbo].[EmployeeLoans] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LoanEMISchedule] CHECK CONSTRAINT [FK_LoanEMI_Loan]
GO
ALTER TABLE [dbo].[LoanEMISchedule]  WITH CHECK ADD  CONSTRAINT [FK_LoanEMI_Payroll] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[LoanEMISchedule] CHECK CONSTRAINT [FK_LoanEMI_Payroll]
GO
ALTER TABLE [dbo].[PasswordResetTokens]  WITH CHECK ADD  CONSTRAINT [FK_PasswordResetTokens_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PasswordResetTokens] CHECK CONSTRAINT [FK_PasswordResetTokens_Users]
GO
ALTER TABLE [dbo].[PayrollArrears]  WITH CHECK ADD  CONSTRAINT [FK_Arrears_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[PayrollArrears] CHECK CONSTRAINT [FK_Arrears_Employee]
GO
ALTER TABLE [dbo].[PayrollArrears]  WITH CHECK ADD  CONSTRAINT [FK_Arrears_NewStructure] FOREIGN KEY([NewSalaryStructureId])
REFERENCES [dbo].[EmployeeSalaryStructure] ([Id])
GO
ALTER TABLE [dbo].[PayrollArrears] CHECK CONSTRAINT [FK_Arrears_NewStructure]
GO
ALTER TABLE [dbo].[PayrollArrears]  WITH CHECK ADD  CONSTRAINT [FK_Arrears_OldStructure] FOREIGN KEY([OldSalaryStructureId])
REFERENCES [dbo].[EmployeeSalaryStructure] ([Id])
GO
ALTER TABLE [dbo].[PayrollArrears] CHECK CONSTRAINT [FK_Arrears_OldStructure]
GO
ALTER TABLE [dbo].[PayrollArrears]  WITH CHECK ADD  CONSTRAINT [FK_Arrears_Payroll] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[PayrollArrears] CHECK CONSTRAINT [FK_Arrears_Payroll]
GO
ALTER TABLE [dbo].[PayrollProcessing]  WITH CHECK ADD  CONSTRAINT [FK_PayrollProcessing_Cycle] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[PayrollProcessing] CHECK CONSTRAINT [FK_PayrollProcessing_Cycle]
GO
ALTER TABLE [dbo].[PayrollProcessing]  WITH CHECK ADD  CONSTRAINT [FK_PayrollProcessing_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[PayrollProcessing] CHECK CONSTRAINT [FK_PayrollProcessing_Employee]
GO
ALTER TABLE [dbo].[PayrollProcessing]  WITH CHECK ADD  CONSTRAINT [FK_PayrollProcessing_Structure] FOREIGN KEY([EmployeeSalaryStructureId])
REFERENCES [dbo].[EmployeeSalaryStructure] ([Id])
GO
ALTER TABLE [dbo].[PayrollProcessing] CHECK CONSTRAINT [FK_PayrollProcessing_Structure]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails]  WITH CHECK ADD  CONSTRAINT [FK_PayrollDetails_Component] FOREIGN KEY([ComponentId])
REFERENCES [dbo].[SalaryComponents] ([Id])
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] CHECK CONSTRAINT [FK_PayrollDetails_Component]
GO
ALTER TABLE [dbo].[PayrollProcessingDetails]  WITH CHECK ADD  CONSTRAINT [FK_PayrollDetails_Processing] FOREIGN KEY([PayrollProcessingId])
REFERENCES [dbo].[PayrollProcessing] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PayrollProcessingDetails] CHECK CONSTRAINT [FK_PayrollDetails_Processing]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_Users]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permissions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Roles]
GO
ALTER TABLE [dbo].[SalarySlips]  WITH CHECK ADD  CONSTRAINT [FK_SalarySlips_Cycle] FOREIGN KEY([PayrollCycleId])
REFERENCES [dbo].[PayrollCycle] ([Id])
GO
ALTER TABLE [dbo].[SalarySlips] CHECK CONSTRAINT [FK_SalarySlips_Cycle]
GO
ALTER TABLE [dbo].[SalarySlips]  WITH CHECK ADD  CONSTRAINT [FK_SalarySlips_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[SalarySlips] CHECK CONSTRAINT [FK_SalarySlips_Employee]
GO
ALTER TABLE [dbo].[SalarySlips]  WITH CHECK ADD  CONSTRAINT [FK_SalarySlips_Processing] FOREIGN KEY([PayrollProcessingId])
REFERENCES [dbo].[PayrollProcessing] ([Id])
GO
ALTER TABLE [dbo].[SalarySlips] CHECK CONSTRAINT [FK_SalarySlips_Processing]
GO
ALTER TABLE [dbo].[SalaryTemplateComponents]  WITH CHECK ADD  CONSTRAINT [FK_TemplateComponents_Component] FOREIGN KEY([ComponentId])
REFERENCES [dbo].[SalaryComponents] ([Id])
GO
ALTER TABLE [dbo].[SalaryTemplateComponents] CHECK CONSTRAINT [FK_TemplateComponents_Component]
GO
ALTER TABLE [dbo].[SalaryTemplateComponents]  WITH CHECK ADD  CONSTRAINT [FK_TemplateComponents_Template] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[SalaryTemplates] ([Id])
GO
ALTER TABLE [dbo].[SalaryTemplateComponents] CHECK CONSTRAINT [FK_TemplateComponents_Template]
GO
ALTER TABLE [dbo].[SalaryTemplates]  WITH CHECK ADD  CONSTRAINT [FK_SalaryTemplates_Department] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[SalaryTemplates] CHECK CONSTRAINT [FK_SalaryTemplates_Department]
GO
ALTER TABLE [dbo].[TicketAttachments]  WITH CHECK ADD  CONSTRAINT [FK_TicketAttachments_Ticket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Tickets] ([TicketId])
GO
ALTER TABLE [dbo].[TicketAttachments] CHECK CONSTRAINT [FK_TicketAttachments_Ticket]
GO
ALTER TABLE [dbo].[TicketAttachments]  WITH CHECK ADD  CONSTRAINT [FK_TicketAttachments_User] FOREIGN KEY([UploadedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[TicketAttachments] CHECK CONSTRAINT [FK_TicketAttachments_User]
GO
ALTER TABLE [dbo].[TicketComments]  WITH CHECK ADD  CONSTRAINT [FK_TicketComments_Ticket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Tickets] ([TicketId])
GO
ALTER TABLE [dbo].[TicketComments] CHECK CONSTRAINT [FK_TicketComments_Ticket]
GO
ALTER TABLE [dbo].[TicketComments]  WITH CHECK ADD  CONSTRAINT [FK_TicketComments_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[TicketComments] CHECK CONSTRAINT [FK_TicketComments_User]
GO
ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_Ticket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Tickets] ([TicketId])
GO
ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_Ticket]
GO
ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_User] FOREIGN KEY([ChangedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_User]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_AssignedTo] FOREIGN KEY([AssignedTo])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Tickets_AssignedTo]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Tickets_CreatedBy]
GO
ALTER TABLE [dbo].[TicketWatchers]  WITH CHECK ADD FOREIGN KEY([TicketId])
REFERENCES [dbo].[Tickets] ([TicketId])
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users]
GO
ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_Users]
GO
ALTER TABLE [dbo].[BankTransferBatches]  WITH CHECK ADD  CONSTRAINT [CK_BankTransfer_Status] CHECK  (([Status]='Failed' OR [Status]='Completed' OR [Status]='Processed' OR [Status]='Uploaded' OR [Status]='Generated' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[BankTransferBatches] CHECK CONSTRAINT [CK_BankTransfer_Status]
GO
ALTER TABLE [dbo].[EmployeeAdvances]  WITH CHECK ADD  CONSTRAINT [CK_EmployeeAdvances_Status] CHECK  (([Status]='Cancelled' OR [Status]='Recovered' OR [Status]='UnderRecovery' OR [Status]='Disbursed' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[EmployeeAdvances] CHECK CONSTRAINT [CK_EmployeeAdvances_Status]
GO
ALTER TABLE [dbo].[EmployeeBankDetails]  WITH CHECK ADD  CONSTRAINT [CK_BankDetails_AccountType] CHECK  (([AccountType]='NRO' OR [AccountType]='NRE' OR [AccountType]='Current' OR [AccountType]='Savings'))
GO
ALTER TABLE [dbo].[EmployeeBankDetails] CHECK CONSTRAINT [CK_BankDetails_AccountType]
GO
ALTER TABLE [dbo].[EmployeeLoans]  WITH CHECK ADD  CONSTRAINT [CK_EmployeeLoans_Status] CHECK  (([Status]='Cancelled' OR [Status]='Closed' OR [Status]='Active' OR [Status]='Disbursed' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[EmployeeLoans] CHECK CONSTRAINT [CK_EmployeeLoans_Status]
GO
ALTER TABLE [dbo].[EmployeeReimbursements]  WITH CHECK ADD  CONSTRAINT [CK_Reimbursements_PaymentStatus] CHECK  (([PaymentStatus]='Failed' OR [PaymentStatus]='Paid' OR [PaymentStatus]='Pending'))
GO
ALTER TABLE [dbo].[EmployeeReimbursements] CHECK CONSTRAINT [CK_Reimbursements_PaymentStatus]
GO
ALTER TABLE [dbo].[EmployeeReimbursements]  WITH CHECK ADD  CONSTRAINT [CK_Reimbursements_Status] CHECK  (([Status]='Cancelled' OR [Status]='Paid' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[EmployeeReimbursements] CHECK CONSTRAINT [CK_Reimbursements_Status]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents]  WITH CHECK ADD  CONSTRAINT [CK_EmpSalaryComp_CalcType] CHECK  (([CalculationType]='Manual' OR [CalculationType]='Attendance' OR [CalculationType]='Formula' OR [CalculationType]='Percentage' OR [CalculationType]='Fixed'))
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] CHECK CONSTRAINT [CK_EmpSalaryComp_CalcType]
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents]  WITH CHECK ADD  CONSTRAINT [CK_EmpSalaryComp_Type] CHECK  (([ComponentType]='Deduction' OR [ComponentType]='Earning'))
GO
ALTER TABLE [dbo].[EmployeeSalaryComponents] CHECK CONSTRAINT [CK_EmpSalaryComp_Type]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations]  WITH CHECK ADD  CONSTRAINT [CK_TaxDeclaration_Regime] CHECK  (([SelectedTaxRegime]='New' OR [SelectedTaxRegime]='Old'))
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] CHECK CONSTRAINT [CK_TaxDeclaration_Regime]
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations]  WITH CHECK ADD  CONSTRAINT [CK_TaxDeclaration_Status] CHECK  (([Status]='Locked' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Submitted' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[EmployeeTaxDeclarations] CHECK CONSTRAINT [CK_TaxDeclaration_Status]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement]  WITH CHECK ADD  CONSTRAINT [CK_FnF_PaymentStatus] CHECK  (([PaymentStatus]='OnHold' OR [PaymentStatus]='Paid' OR [PaymentStatus]='Pending'))
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] CHECK CONSTRAINT [CK_FnF_PaymentStatus]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement]  WITH CHECK ADD  CONSTRAINT [CK_FnF_SeparationType] CHECK  (([SeparationType]='Deceased' OR [SeparationType]='Absconding' OR [SeparationType]='Retirement' OR [SeparationType]='Termination' OR [SeparationType]='Resignation'))
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] CHECK CONSTRAINT [CK_FnF_SeparationType]
GO
ALTER TABLE [dbo].[FullAndFinalSettlement]  WITH CHECK ADD  CONSTRAINT [CK_FnF_Status] CHECK  (([Status]='Completed' OR [Status]='Paid' OR [Status]='Approved' OR [Status]='Calculated' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[FullAndFinalSettlement] CHECK CONSTRAINT [CK_FnF_Status]
GO
ALTER TABLE [dbo].[LeaveRequests]  WITH CHECK ADD  CONSTRAINT [CHK_LeaveRequest_Dates] CHECK  (([EndDate]>=[StartDate]))
GO
ALTER TABLE [dbo].[LeaveRequests] CHECK CONSTRAINT [CHK_LeaveRequest_Dates]
GO
ALTER TABLE [dbo].[LeaveRequests]  WITH CHECK ADD  CONSTRAINT [CHK_LeaveRequest_Status] CHECK  (([Status]='Cancelled' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[LeaveRequests] CHECK CONSTRAINT [CHK_LeaveRequest_Status]
GO
ALTER TABLE [dbo].[LoanEMISchedule]  WITH CHECK ADD  CONSTRAINT [CK_LoanEMI_Status] CHECK  (([Status]='Waived' OR [Status]='Skipped' OR [Status]='PartiallyPaid' OR [Status]='Paid' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[LoanEMISchedule] CHECK CONSTRAINT [CK_LoanEMI_Status]
GO
ALTER TABLE [dbo].[LoanTypes]  WITH CHECK ADD  CONSTRAINT [CK_LoanTypes_EMICalc] CHECK  (([EMICalculationType]='ReducingBalance' OR [EMICalculationType]='FlatRate'))
GO
ALTER TABLE [dbo].[LoanTypes] CHECK CONSTRAINT [CK_LoanTypes_EMICalc]
GO
ALTER TABLE [dbo].[PayrollArrears]  WITH CHECK ADD  CONSTRAINT [CK_Arrears_PaymentStatus] CHECK  (([PaymentStatus]='Cancelled' OR [PaymentStatus]='Paid' OR [PaymentStatus]='Approved' OR [PaymentStatus]='Pending'))
GO
ALTER TABLE [dbo].[PayrollArrears] CHECK CONSTRAINT [CK_Arrears_PaymentStatus]
GO
ALTER TABLE [dbo].[PayrollCycle]  WITH CHECK ADD  CONSTRAINT [CK_PayrollCycle_PeriodType] CHECK  (([PeriodType]='Daily' OR [PeriodType]='Bi-Weekly' OR [PeriodType]='Weekly' OR [PeriodType]='Monthly'))
GO
ALTER TABLE [dbo].[PayrollCycle] CHECK CONSTRAINT [CK_PayrollCycle_PeriodType]
GO
ALTER TABLE [dbo].[PayrollCycle]  WITH CHECK ADD  CONSTRAINT [CK_PayrollCycle_Status] CHECK  (([Status]='Cancelled' OR [Status]='Closed' OR [Status]='Paid' OR [Status]='Approved' OR [Status]='Processed' OR [Status]='InProgress' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[PayrollCycle] CHECK CONSTRAINT [CK_PayrollCycle_Status]
GO
ALTER TABLE [dbo].[PayrollProcessing]  WITH CHECK ADD  CONSTRAINT [CK_PayrollProcessing_PaymentStatus] CHECK  (([PaymentStatus]='Cancelled' OR [PaymentStatus]='Failed' OR [PaymentStatus]='Paid' OR [PaymentStatus]='Pending'))
GO
ALTER TABLE [dbo].[PayrollProcessing] CHECK CONSTRAINT [CK_PayrollProcessing_PaymentStatus]
GO
ALTER TABLE [dbo].[PayrollProcessing]  WITH CHECK ADD  CONSTRAINT [CK_PayrollProcessing_Status] CHECK  (([Status]='OnHold' OR [Status]='Paid' OR [Status]='Approved' OR [Status]='Verified' OR [Status]='Calculated' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[PayrollProcessing] CHECK CONSTRAINT [CK_PayrollProcessing_Status]
GO
ALTER TABLE [dbo].[ProfessionalTaxSlabs]  WITH CHECK ADD  CONSTRAINT [CK_PTSlab_Month] CHECK  (([ApplicableMonth] IS NULL OR [ApplicableMonth]>=(1) AND [ApplicableMonth]<=(12)))
GO
ALTER TABLE [dbo].[ProfessionalTaxSlabs] CHECK CONSTRAINT [CK_PTSlab_Month]
GO
ALTER TABLE [dbo].[SalaryComponents]  WITH CHECK ADD  CONSTRAINT [CK_SalaryComponents_CalcType] CHECK  (([CalculationType]='Manual' OR [CalculationType]='Attendance' OR [CalculationType]='Formula' OR [CalculationType]='Percentage' OR [CalculationType]='Fixed'))
GO
ALTER TABLE [dbo].[SalaryComponents] CHECK CONSTRAINT [CK_SalaryComponents_CalcType]
GO
ALTER TABLE [dbo].[SalaryComponents]  WITH CHECK ADD  CONSTRAINT [CK_SalaryComponents_Type] CHECK  (([ComponentType]='Deduction' OR [ComponentType]='Earning'))
GO
ALTER TABLE [dbo].[SalaryComponents] CHECK CONSTRAINT [CK_SalaryComponents_Type]
GO
ALTER TABLE [dbo].[SalarySlips]  WITH CHECK ADD  CONSTRAINT [CK_SalarySlips_PDFStatus] CHECK  (([PDFGenerationStatus]='Failed' OR [PDFGenerationStatus]='Generated' OR [PDFGenerationStatus]='InProgress' OR [PDFGenerationStatus]='Pending'))
GO
ALTER TABLE [dbo].[SalarySlips] CHECK CONSTRAINT [CK_SalarySlips_PDFStatus]
GO
ALTER TABLE [dbo].[SalarySlips]  WITH CHECK ADD  CONSTRAINT [CK_SalarySlips_Status] CHECK  (([Status]='Downloaded' OR [Status]='Viewed' OR [Status]='Sent' OR [Status]='Generated' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[SalarySlips] CHECK CONSTRAINT [CK_SalarySlips_Status]
GO
ALTER TABLE [dbo].[SSRSReportConfigurations]  WITH CHECK ADD  CONSTRAINT [CK_SSRSReport_AuthType] CHECK  (([AuthenticationType]='Anonymous' OR [AuthenticationType]='Custom' OR [AuthenticationType]='SQL' OR [AuthenticationType]='Windows'))
GO
ALTER TABLE [dbo].[SSRSReportConfigurations] CHECK CONSTRAINT [CK_SSRSReport_AuthType]
GO
ALTER TABLE [dbo].[StatutorySettings]  WITH CHECK ADD  CONSTRAINT [CK_Statutory_Type] CHECK  (([SettingType]='LWF' OR [SettingType]='Bonus' OR [SettingType]='Gratuity' OR [SettingType]='TDS' OR [SettingType]='PT' OR [SettingType]='ESI' OR [SettingType]='PF'))
GO
ALTER TABLE [dbo].[StatutorySettings] CHECK CONSTRAINT [CK_Statutory_Type]
GO
ALTER TABLE [dbo].[TDSSlabs]  WITH CHECK ADD  CONSTRAINT [CK_TDSSlab_AgeGroup] CHECK  (([ApplicableForAge]='All' OR [ApplicableForAge]='Above80' OR [ApplicableForAge]='60to80' OR [ApplicableForAge]='Below60'))
GO
ALTER TABLE [dbo].[TDSSlabs] CHECK CONSTRAINT [CK_TDSSlab_AgeGroup]
GO
ALTER TABLE [dbo].[TDSSlabs]  WITH CHECK ADD  CONSTRAINT [CK_TDSSlab_Regime] CHECK  (([TaxRegime]='New' OR [TaxRegime]='Old'))
GO
ALTER TABLE [dbo].[TDSSlabs] CHECK CONSTRAINT [CK_TDSSlab_Regime]
GO
/****** Object:  StoredProcedure [dbo].[sp_AddEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 5. ADD EMPLOYEE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_AddEmployee]
    @Name NVARCHAR(100),
    @Email NVARCHAR(255),
    @DepartmentId INT,
    @Salary DECIMAL(18,2),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Address NVARCHAR(500) = NULL,
    @DateOfBirth DATE = NULL,
    @JoiningDate DATE = NULL,
    @Role NVARCHAR(50) = NULL,
    @ProfileImagePath NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @CreatedBy INT = NULL,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check for duplicate email
        IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND IsDeleted = 0)
        BEGIN
            RAISERROR('Email already exists', 16, 1);
            RETURN;
        END
        
        INSERT INTO Employees (
            Name, Email, DepartmentId, Salary, PhoneNumber, Address,
            DateOfBirth, JoiningDate, Role, ProfileImagePath,
            IsActive, IsDeleted, CreatedBy, CreatedDate
        )
        VALUES (
            @Name, @Email, @DepartmentId, @Salary, @PhoneNumber, @Address,
            @DateOfBirth, ISNULL(@JoiningDate, GETDATE()), @Role, @ProfileImagePath,
            @IsActive, 0, @CreatedBy, GETDATE()
        );
        
        SET @NewId = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        SET @NewId = 0;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddHoliday]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_AddHoliday]
    @Name        NVARCHAR(200),
    @Date        DATE,
    @Type        NVARCHAR(50) = 'Public',
    @Description NVARCHAR(500) = NULL,
    @CreatedBy   INT = NULL,
    @NewId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for duplicate date
    IF EXISTS (
        SELECT 1 FROM Holidays 
        WHERE Date = @Date AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('A holiday already exists on this date.', 16, 1);
        RETURN;
    END

    INSERT INTO Holidays (Name, Date, Day, Type, Description, Year, IsActive, IsDeleted, CreatedBy, CreatedDate)
    VALUES (
        @Name, 
        @Date, 
        DATENAME(WEEKDAY, @Date),  -- Auto-calculate day name
        @Type, 
        @Description, 
        YEAR(@Date),               -- Auto-set year
        1, 
        0, 
        @CreatedBy, 
        GETDATE()
    );

    SET @NewId = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddStudent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddStudent]
    @StudentId NVARCHAR(50),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @FullName NVARCHAR(200),
    @Class NVARCHAR(50),
    @Subjects NVARCHAR(500) = NULL,
    @Age INT = NULL,
    @DateOfBirth DATE = NULL,
    @JoiningDate DATE = NULL,
    @BatchTime NVARCHAR(50) = NULL,
    @PassportPhotoPath NVARCHAR(500) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Address NVARCHAR(500) = NULL,
    @ParentName NVARCHAR(200) = NULL,
    @ParentPhone NVARCHAR(20) = NULL,
    @ParentEmail NVARCHAR(100) = NULL,
    @CreatedBy INT = NULL,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Set default joining date if not provided
        IF @JoiningDate IS NULL
            SET @JoiningDate = GETDATE();
        
        -- Insert student
        INSERT INTO Students (
            StudentId, FirstName, LastName, FullName, Class, Subjects, 
            Age, DateOfBirth, JoiningDate, BatchTime, PassportPhotoPath,
            PhoneNumber, Email, Address, ParentName, ParentPhone, ParentEmail,
            IsActive, IsDeleted, CreatedBy, CreatedDate
        )
        VALUES (
            @StudentId, @FirstName, @LastName, @FullName, @Class, @Subjects,
            @Age, @DateOfBirth, @JoiningDate, @BatchTime, @PassportPhotoPath,
            @PhoneNumber, @Email, @Address, @ParentName, @ParentPhone, @ParentEmail,
            1, 0, @CreatedBy, GETDATE()
        );
        
        SET @NewId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddTemplateComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 14. Add Template Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_AddTemplateComponent]
(
    @TemplateId INT,
    @ComponentId INT,
    @CalculationType NVARCHAR(50),
    @CalculationBase NVARCHAR(50) = NULL,
    @Percentage DECIMAL(5,2) = NULL,
    @FixedAmount DECIMAL(18,2) = NULL,
    @MonthlyAmount DECIMAL(18,2),
    @AnnualAmount DECIMAL(18,2),
    @DisplayOrder INT = 0,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if component already exists in template
    IF EXISTS (SELECT 1 FROM SalaryTemplateComponents WHERE TemplateId = @TemplateId AND ComponentId = @ComponentId AND IsActive = 1)
    BEGIN
        RAISERROR('Component already exists in template', 16, 1);
        RETURN;
    END

    INSERT INTO SalaryTemplateComponents
    (
        TemplateId, ComponentId, CalculationType, CalculationBase,
        Percentage, FixedAmount, MonthlyAmount, AnnualAmount,
        DisplayOrder, IsActive, CreatedBy, CreatedDate
    )
    VALUES
    (
        @TemplateId, @ComponentId, @CalculationType, @CalculationBase,
        @Percentage, @FixedAmount, @MonthlyAmount, @AnnualAmount,
        @DisplayOrder, 1, @CreatedBy, GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS TemplateComponentId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddTicketAttachment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 10: Add Attachment
-- =============================================
CREATE PROCEDURE [dbo].[sp_AddTicketAttachment]
    @TicketId INT,
    @FileName NVARCHAR(255),
    @FilePath NVARCHAR(500),
    @FileSize BIGINT,
    @FileType NVARCHAR(100),
    @UploadedBy INT,
    @AttachmentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO TicketAttachments (TicketId, FileName, FilePath, FileSize, FileType, UploadedBy)
        VALUES (@TicketId, @FileName, @FilePath, @FileSize, @FileType, @UploadedBy);
        
        SET @AttachmentId = SCOPE_IDENTITY();
        
        UPDATE Tickets SET UpdatedDate = GETDATE() WHERE TicketId = @TicketId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
        VALUES (@TicketId, @UploadedBy, 'Attachment Added', @FileName);
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Attachment added successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddTicketComment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 8: Add Comment
-- =============================================
CREATE PROCEDURE [dbo].[sp_AddTicketComment]
    @TicketId INT,
    @UserId INT,
    @Comment NVARCHAR(MAX),
    @CommentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO TicketComments (TicketId, UserId, Comment)
        VALUES (@TicketId, @UserId, @Comment);
        
        SET @CommentId = SCOPE_IDENTITY();
        
        UPDATE Tickets SET UpdatedDate = GETDATE() WHERE TicketId = @TicketId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
        VALUES (@TicketId, @UserId, 'Comment Added', LEFT(@Comment, 200));
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Comment added successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AllocateDefaultLeaveForAllEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_AllocateDefaultLeaveForAllEmployees]
    @Year       INT,
    @CreatedBy  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO LeaveBalances (EmployeeId, LeaveTypeId, Year, TotalAllocated, CarryForward, CreatedBy)
        SELECT e.Id, lt.Id, @Year, lt.DefaultDays, 0, @CreatedBy
        FROM Employees e
        CROSS JOIN LeaveTypes lt
        WHERE e.IsActive = 1 AND e.IsDeleted = 0
          AND lt.IsActive = 1 AND lt.IsDeleted = 0
          AND lt.DefaultDays > 0
          AND NOT EXISTS (
              SELECT 1 FROM LeaveBalances lb
              WHERE lb.EmployeeId = e.Id
                AND lb.LeaveTypeId = lt.Id
                AND lb.Year = @Year
          );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AllocateFixedLeaveForAllEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- ✅ SP: Allocate fixed leaves to ALL employees
-- =============================================
CREATE   PROCEDURE [dbo].[sp_AllocateFixedLeaveForAllEmployees]
    @Year           INT,
    @LeavesPerType  DECIMAL(5,1) = 20,
    @CreatedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @InsertedCount INT = 0;

        INSERT INTO LeaveBalances (EmployeeId, LeaveTypeId, Year, TotalAllocated, 
                                   TotalUsed, TotalPending, CarryForward, CreatedBy, CreatedDate, IsActive)
        SELECT 
            e.Id,
            lt.Id,
            @Year,
            @LeavesPerType,
            0,
            0,
            0,
            @CreatedBy,
            GETDATE(),
            1
        FROM Employees e
        CROSS JOIN LeaveTypes lt
        WHERE e.IsActive = 1 
          AND e.IsDeleted = 0
          AND lt.IsActive = 1 
          AND lt.IsDeleted = 0
          AND NOT EXISTS (
              SELECT 1 FROM LeaveBalances lb 
              WHERE lb.EmployeeId = e.Id 
                AND lb.LeaveTypeId = lt.Id 
                AND lb.Year = @Year
          );

        SET @InsertedCount = @@ROWCOUNT;

        COMMIT TRANSACTION;

        SELECT @InsertedCount AS RecordsInserted, 
               'Leave allocated successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AllocateLeaveBalance]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_AllocateLeaveBalance]
    @EmployeeId     INT,
    @LeaveTypeId    INT,
    @Year           INT,
    @TotalAllocated DECIMAL(5,1),
    @CarryForward   DECIMAL(5,1) = 0,
    @CreatedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM LeaveBalances
        WHERE EmployeeId = @EmployeeId
          AND LeaveTypeId = @LeaveTypeId
          AND Year = @Year
    )
    BEGIN
        -- Update existing
        UPDATE LeaveBalances
        SET TotalAllocated = @TotalAllocated,
            CarryForward = @CarryForward,
            UpdatedBy = @CreatedBy,
            UpdatedDate = GETDATE()
        WHERE EmployeeId = @EmployeeId
          AND LeaveTypeId = @LeaveTypeId
          AND Year = @Year;
    END
    ELSE
    BEGIN
        -- Insert new
        INSERT INTO LeaveBalances (EmployeeId, LeaveTypeId, Year, TotalAllocated, CarryForward, CreatedBy)
        VALUES (@EmployeeId, @LeaveTypeId, @Year, @TotalAllocated, @CarryForward, @CreatedBy);
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AllocateLeaveForSingleEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- ✅ SP: Allocate leaves to SINGLE employee
-- =============================================
CREATE   PROCEDURE [dbo].[sp_AllocateLeaveForSingleEmployee]
    @EmployeeId     INT,
    @Year           INT,
    @LeavesPerType  DECIMAL(5,1) = 20,
    @CreatedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO LeaveBalances (EmployeeId, LeaveTypeId, Year, TotalAllocated, 
                                   TotalUsed, TotalPending, CarryForward, CreatedBy, CreatedDate, IsActive)
        SELECT 
            @EmployeeId,
            lt.Id,
            @Year,
            @LeavesPerType,
            0,
            0,
            0,
            @CreatedBy,
            GETDATE(),
            1
        FROM LeaveTypes lt
        WHERE lt.IsActive = 1 
          AND lt.IsDeleted = 0
          AND NOT EXISTS (
              SELECT 1 FROM LeaveBalances lb 
              WHERE lb.EmployeeId = @EmployeeId 
                AND lb.LeaveTypeId = lt.Id 
                AND lb.Year = @Year
          );

        COMMIT TRANSACTION;

        -- Return allocated balance
        SELECT lb.Id, lb.EmployeeId, lb.LeaveTypeId, lb.Year,
               lb.TotalAllocated, lb.TotalUsed, lb.TotalPending,
               lb.CarryForward,
               (lb.TotalAllocated + lb.CarryForward - lb.TotalUsed - lb.TotalPending) AS TotalAvailable,
               lt.Name AS LeaveTypeName,
               lt.Code AS LeaveTypeCode,
               lt.IsPaid,
               e.Name AS EmployeeName
        FROM LeaveBalances lb
        INNER JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id
        INNER JOIN Employees e ON lb.EmployeeId = e.Id
        WHERE lb.EmployeeId = @EmployeeId AND lb.Year = @Year;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApplyLeave]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ApplyLeave]
    @EmployeeId INT,
    @LeaveTypeId INT,
    @StartDate DATE,
    @EndDate DATE,
    @TotalDays DECIMAL(5,1),
    @Reason NVARCHAR(1000),
    @IsHalfDay BIT = 0,
    @HalfDayType NVARCHAR(20) = NULL,
    @EmergencyContact NVARCHAR(100) = NULL,
    @AttachmentPath NVARCHAR(500) = NULL,
    @Status NVARCHAR(20) = 'Pending',
    @CreatedBy INT,
    @NewLeaveRequestId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[LeaveRequests] (
            [EmployeeId], [LeaveTypeId], [StartDate], [EndDate], [TotalDays],
            [Reason], [Status], [IsHalfDay], [HalfDayType], [EmergencyContact],
            [AttachmentPath], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [AppliedDate]
        ) VALUES (
            @EmployeeId, @LeaveTypeId, @StartDate, @EndDate, @TotalDays,
            @Reason, @Status, @IsHalfDay, @HalfDayType, @EmergencyContact,
            @AttachmentPath, 1, 0, @CreatedBy, GETDATE(), GETDATE()
        );

        SET @NewLeaveRequestId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApproveLeave]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_ApproveLeave]
    @LeaveRequestId INT,
    @ApprovedBy     INT,
    @Remarks        NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @EmployeeId INT, @LeaveTypeId INT, @TotalDays DECIMAL(5,1), @StartDate DATE;

        SELECT @EmployeeId = EmployeeId,
               @LeaveTypeId = LeaveTypeId,
               @TotalDays = TotalDays,
               @StartDate = StartDate
        FROM LeaveRequests
        WHERE Id = @LeaveRequestId AND IsDeleted = 0;

        IF @EmployeeId IS NULL
        BEGIN
            RAISERROR('Leave request not found.', 16, 1);
            RETURN;
        END

        -- Update leave request status
        UPDATE LeaveRequests
        SET Status = 'Approved',
            ApprovedBy = @ApprovedBy,
            ApprovedDate = GETDATE(),
            Remarks = @Remarks,
            UpdatedBy = @ApprovedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @LeaveRequestId;

        -- Update leave balance: move from Pending to Used
        UPDATE LeaveBalances
        SET TotalUsed = TotalUsed + @TotalDays,
            TotalPending = TotalPending - @TotalDays,
            UpdatedBy = @ApprovedBy,
            UpdatedDate = GETDATE()
        WHERE EmployeeId = @EmployeeId
          AND LeaveTypeId = @LeaveTypeId
          AND Year = YEAR(@StartDate);

        -- Insert approval record
        INSERT INTO LeaveApprovals (LeaveRequestId, ApproverLevel, ApproverId, ApproverRole, Status, Comments, ActionDate)
        VALUES (@LeaveRequestId, 1, @ApprovedBy, 'Manager', 'Approved', @Remarks, GETDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApproveLoan]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN APPROVAL
-- =============================================

-- 10. Approve Loan
-- =============================================
CREATE   PROCEDURE [dbo].[sp_ApproveLoan]
(
    @LoanId INT,
    @ApprovedAmount DECIMAL(18,2),
    @ApprovedTenureMonths INT = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @ApprovedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InterestRate DECIMAL(5,2);
    DECLARE @TenureMonths INT;
    DECLARE @EMIAmount DECIMAL(18,2);
    DECLARE @TotalRepayable DECIMAL(18,2);

    -- Get current loan details
    SELECT @InterestRate = InterestRate,
           @TenureMonths = ISNULL(@ApprovedTenureMonths, TenureMonths)
    FROM EmployeeLoans 
    WHERE Id = @LoanId AND Status = 'Pending';

    IF @InterestRate IS NULL
    BEGIN
        RAISERROR('Loan not found or not in pending status', 16, 1);
        RETURN;
    END

    -- Recalculate EMI with approved amount
    DECLARE @MonthlyInterestRate DECIMAL(18,10) = @InterestRate / 12 / 100;
    
    IF @MonthlyInterestRate > 0
    BEGIN
        SET @EMIAmount = @ApprovedAmount * @MonthlyInterestRate * 
                         POWER(1 + @MonthlyInterestRate, @TenureMonths) / 
                         (POWER(1 + @MonthlyInterestRate, @TenureMonths) - 1);
    END
    ELSE
    BEGIN
        SET @EMIAmount = @ApprovedAmount / @TenureMonths;
    END

    SET @EMIAmount = ROUND(@EMIAmount, 2);
    SET @TotalRepayable = @EMIAmount * @TenureMonths;

    UPDATE EmployeeLoans 
    SET Status = 'Approved',
        ApprovedAmount = @ApprovedAmount,
        LoanAmount = @ApprovedAmount,
        TenureMonths = @TenureMonths,
        EMIAmount = @EMIAmount,
        TotalRepayableAmount = @TotalRepayable,
        ApprovedBy = @ApprovedBy,
        ApprovedDate = GETDATE(),
        ApprovalRemarks = @Remarks,
        UpdatedBy = @ApprovedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @LoanId AND Status = 'Pending';

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApprovePayrollCycle]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 6. Approve Payroll Cycle
-- =============================================
CREATE   PROCEDURE [dbo].[sp_ApprovePayrollCycle]
    @CycleId INT,
    @ApprovedBy INT,
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PayrollCycle
    SET Status = 'Approved',
        ApprovedBy = @ApprovedBy,
        ApprovedDate = GETDATE(),
        ApprovalRemarks = @Remarks,
        UpdatedBy = @ApprovedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @CycleId AND Status = 'Processed';

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApproveUserAndAssignRole]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_ApproveUserAndAssignRole]
    @UserId INT,
    @RoleId INT,
    @DepartmentId INT = NULL,
    @ApprovedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @UserId AND ISNULL(IsDeleted, 0) = 0)
        BEGIN
            THROW 51000, 'User does not exist or has been deleted.', 1;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId AND ISNULL(IsActive, 1) = 1)
        BEGIN
            THROW 51001, 'Selected role does not exist or is inactive.', 1;
        END;

        UPDATE dbo.Users
        SET IsActive = 1,
            RegistrationStatus = 'Approved',
            ApprovedBy = @ApprovedBy,
            ApprovedDate = GETDATE(),
            UpdatedBy = @ApprovedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @UserId;

        DELETE FROM dbo.UserRoles
        WHERE UserId = @UserId;

        INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedBy, IsActive)
        VALUES (@UserId, @RoleId, @ApprovedBy, 1);

        IF @DepartmentId IS NOT NULL
        BEGIN
            UPDATE e
            SET e.DepartmentId = @DepartmentId,
                e.UpdatedBy = @ApprovedBy,
                e.UpdatedDate = GETDATE()
            FROM dbo.Employees e
            INNER JOIN dbo.Users u ON u.Id = @UserId
            WHERE e.Email = u.Email;
        END;

        COMMIT TRANSACTION;

        SELECT
            u.Id,
            u.Username,
            u.Email,
            ISNULL(u.FullName, CONCAT(u.FirstName, ' ', u.LastName)) AS FullName,
            u.RegistrationStatus,
            r.RoleName AS RoleName,
            d.DepartmentName AS DepartmentName
        FROM dbo.Users u
        LEFT JOIN dbo.UserRoles ur ON u.Id = ur.UserId AND ISNULL(ur.IsActive, 1) = 1
        LEFT JOIN dbo.Roles r ON ur.RoleId = r.Id
        LEFT JOIN dbo.Employees e ON e.Email = u.Email
        LEFT JOIN dbo.Departments d ON e.DepartmentId = d.Id
        WHERE u.Id = @UserId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_AssignRoleToUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   13. ASSIGN ROLE TO USER
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_AssignRoleToUser]
    @UserId INT,
    @RoleId INT,
    @AssignedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 
            FROM Users 
            WHERE Id = @UserId 
              AND ISNULL(IsDeleted, 0) = 0
        )
        BEGIN
            SELECT 0 AS Success, 'User not found' AS Message;
            RETURN;
        END

        IF NOT EXISTS (
            SELECT 1 
            FROM Roles 
            WHERE Id = @RoleId 
              AND IsActive = 1
        )
        BEGIN
            SELECT 0 AS Success, 'Role not found or inactive' AS Message;
            RETURN;
        END

        IF EXISTS (
            SELECT 1 
            FROM UserRoles 
            WHERE UserId = @UserId 
              AND RoleId = @RoleId 
              AND IsActive = 1
        )
        BEGIN
            SELECT 0 AS Success, 'Role already assigned to user' AS Message;
            RETURN;
        END

        INSERT INTO UserRoles
        (
            UserId,
            RoleId,
            AssignedDate,
            AssignedBy,
            IsActive
        )
        VALUES
        (
            @UserId,
            @RoleId,
            GETDATE(),
            @AssignedBy,
            1
        );

        SELECT 1 AS Success, 'Role assigned successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AssignSalaryToEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 20. Assign Salary To Employee
-- =============================================
CREATE   PROCEDURE [dbo].[sp_AssignSalaryToEmployee]
(
    @EmployeeId INT,
    @TemplateId INT = NULL,
    @EffectiveFrom DATE,
    @CTC DECIMAL(18,2),
    @GrossSalary DECIMAL(18,2),
    @NetSalary DECIMAL(18,2),
    @BasicSalary DECIMAL(18,2) = NULL,
    @TotalEarnings DECIMAL(18,2) = NULL,
    @TotalDeductions DECIMAL(18,2) = NULL,
    @EmployerContributions DECIMAL(18,2) = 0,
    @RevisionReason NVARCHAR(500) = NULL,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @NewStructureId INT;
        DECLARE @RevisionNumber INT;
        DECLARE @PreviousStructureId INT;

        -- Get current revision number
        SELECT @RevisionNumber = ISNULL(MAX(RevisionNumber), 0) + 1,
               @PreviousStructureId = MAX(Id)
        FROM EmployeeSalaryStructure 
        WHERE EmployeeId = @EmployeeId;

        -- Mark previous structure as not current
        UPDATE EmployeeSalaryStructure
        SET IsCurrentStructure = 0,
            EffectiveTo = DATEADD(DAY, -1, @EffectiveFrom),
            UpdatedBy = @CreatedBy,
            UpdatedDate = GETDATE()
        WHERE EmployeeId = @EmployeeId AND IsCurrentStructure = 1;

        -- Insert new salary structure
        INSERT INTO EmployeeSalaryStructure
        (
            EmployeeId, TemplateId, EffectiveFrom, CTC, GrossSalary,
            NetSalary, BasicSalary, TotalEarnings, TotalDeductions,
            EmployerContributions, RevisionNumber, RevisionReason,
            PreviousStructureId, IsCurrentStructure, IsActive, Status,
            CreatedBy, CreatedDate
        )
        VALUES
        (
            @EmployeeId, @TemplateId, @EffectiveFrom, @CTC, @GrossSalary,
            @NetSalary, @BasicSalary, @TotalEarnings, @TotalDeductions,
            @EmployerContributions, @RevisionNumber, @RevisionReason,
            @PreviousStructureId, 1, 1, 'Active',
            @CreatedBy, GETDATE()
        );

        SET @NewStructureId = SCOPE_IDENTITY();

        -- Copy components from template if provided
        IF @TemplateId IS NOT NULL
        BEGIN
            INSERT INTO EmployeeSalaryComponents
            (
                EmployeeSalaryStructureId, ComponentId, ComponentType,
                CalculationType, CalculationBase, Percentage,
                Amount, MonthlyAmount, AnnualAmount, DisplayOrder,
                IsActive, CreatedBy, CreatedDate, IsDeleted
            )
            SELECT 
                @NewStructureId,
                stc.ComponentId,
                sc.ComponentType,
                stc.CalculationType,
                stc.CalculationBase,
                stc.Percentage,
                stc.MonthlyAmount,
                stc.MonthlyAmount,
                stc.AnnualAmount,
                stc.DisplayOrder,
                1,
                @CreatedBy,
                GETDATE(),
                0
            FROM SalaryTemplateComponents stc
            INNER JOIN SalaryComponents sc ON stc.ComponentId = sc.Id
            WHERE stc.TemplateId = @TemplateId AND stc.IsActive = 1;
        END

        COMMIT TRANSACTION;

        SELECT @NewStructureId AS NewStructureId, @RevisionNumber AS RevisionNumber;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AssignTicket]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 6: Assign Ticket
-- =============================================
CREATE PROCEDURE [dbo].[sp_AssignTicket]
    @TicketId INT,
    @AssignedTo INT,
    @AssignedBy INT,
    @Remarks NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @OldAssignee INT;
        DECLARE @OldAssigneeName NVARCHAR(200);
        DECLARE @NewAssigneeName NVARCHAR(200);
        DECLARE @ChangeType NVARCHAR(100);
        
        SELECT @OldAssignee = AssignedTo FROM Tickets WHERE TicketId = @TicketId;
        
        SELECT @OldAssigneeName = FullName FROM Users WHERE Id = @OldAssignee;
        SELECT @NewAssigneeName = FullName FROM Users WHERE Id = @AssignedTo;
        
        SET @ChangeType = CASE 
            WHEN @OldAssignee IS NULL THEN 'Assigned'
            ELSE 'Reassigned'
        END;
        
        UPDATE Tickets
        SET 
            AssignedTo = @AssignedTo,
            Status = CASE 
                WHEN Status = 'New' THEN 'Assigned'
                ELSE Status 
            END,
            UpdatedDate = GETDATE()
        WHERE TicketId = @TicketId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, OldValue, NewValue, Remarks)
        VALUES (
            @TicketId, 
            @AssignedBy, 
            @ChangeType, 
            ISNULL(@OldAssigneeName, 'Unassigned'),
            @NewAssigneeName,
            @Remarks
        );
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Ticket assigned successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BulkAddHolidays]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_BulkAddHolidays]
    @Year      INT,
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    -- This is a placeholder - actual bulk insert would use Table-Valued Parameter
    -- or be handled in application code
    SELECT 'Use application code for bulk import' AS Message;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BulkAssignSalary]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 27. Bulk Assign Salary
-- =============================================
CREATE   PROCEDURE [dbo].[sp_BulkAssignSalary]
(
    @EmployeeIds NVARCHAR(MAX), -- Comma separated IDs
    @TemplateId INT,
    @EffectiveFrom DATE,
    @AssignedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @CTC DECIMAL(18,2), @GrossSalary DECIMAL(18,2), @NetSalary DECIMAL(18,2);
        DECLARE @TotalEarnings DECIMAL(18,2), @TotalDeductions DECIMAL(18,2);

        -- Get template values
        SELECT @CTC = TotalCTC, @GrossSalary = GrossSalary, @NetSalary = NetSalary,
               @TotalEarnings = TotalEarnings, @TotalDeductions = TotalDeductions
        FROM SalaryTemplates WHERE Id = @TemplateId;

        -- Split employee IDs and process each
        DECLARE @EmployeeTable TABLE (EmployeeId INT);
        INSERT INTO @EmployeeTable
        SELECT value FROM STRING_SPLIT(@EmployeeIds, ',');

        DECLARE @EmployeeId INT;
        DECLARE employee_cursor CURSOR FOR SELECT EmployeeId FROM @EmployeeTable;

        OPEN employee_cursor;
        FETCH NEXT FROM employee_cursor INTO @EmployeeId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Call assign procedure for each employee
            EXEC sp_AssignSalaryToEmployee 
                @EmployeeId = @EmployeeId,
                @TemplateId = @TemplateId,
                @EffectiveFrom = @EffectiveFrom,
                @CTC = @CTC,
                @GrossSalary = @GrossSalary,
                @NetSalary = @NetSalary,
                @TotalEarnings = @TotalEarnings,
                @TotalDeductions = @TotalDeductions,
                @RevisionReason = 'Bulk Assignment',
                @CreatedBy = @AssignedBy;

            FETCH NEXT FROM employee_cursor INTO @EmployeeId;
        END

        CLOSE employee_cursor;
        DEALLOCATE employee_cursor;

        COMMIT TRANSACTION;

        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BulkDeleteEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 27. BULK DELETE EMPLOYEES (SOFT DELETE)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_BulkDeleteEmployees]
    @EmployeeIds NVARCHAR(MAX), -- Comma-separated IDs
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Employees
        SET 
            IsDeleted = 1,
            IsActive = 0,
            DeletedBy = @DeletedBy,
            DeletedDate = GETDATE()
        WHERE Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ','));
        
        COMMIT TRANSACTION;
        
        SELECT @@ROWCOUNT AS AffectedRows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BulkUpdateEmployeeDepartment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 25. BULK UPDATE EMPLOYEE DEPARTMENT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_BulkUpdateEmployeeDepartment]
    @EmployeeIds NVARCHAR(MAX), -- Comma-separated IDs
    @NewDepartmentId INT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Employees
        SET 
            DepartmentId = @NewDepartmentId,
            UpdatedBy = @UpdatedBy,
            UpdatedDate = GETDATE()
        WHERE Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ','));
        
        COMMIT TRANSACTION;
        
        SELECT @@ROWCOUNT AS AffectedRows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BulkUpdateEmployeeStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 26. BULK UPDATE EMPLOYEE STATUS
-- =============================================
CREATE   PROCEDURE [dbo].[sp_BulkUpdateEmployeeStatus]
    @EmployeeIds NVARCHAR(MAX), -- Comma-separated IDs
    @IsActive BIT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Employees
        SET 
            IsActive = @IsActive,
            UpdatedBy = @UpdatedBy,
            UpdatedDate = GETDATE()
        WHERE Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ','));
        
        COMMIT TRANSACTION;
        
        SELECT @@ROWCOUNT AS AffectedRows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateArrears]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_CalculateArrears
-- Description: Backdated salary revision साठी arrears calculate करणे
-- Parameters: 
--   @EmployeeId - Employee ID
--   @NewStructureId - New salary structure ID
--   @RevisionEffectiveDate - Effective date of revision
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateArrears]
    @EmployeeId INT,
    @NewStructureId INT,
    @RevisionEffectiveDate DATE,
    @CalculatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldStructureId INT;
        DECLARE @OldGross DECIMAL(18,2);
        DECLARE @NewGross DECIMAL(18,2);
        DECLARE @DifferencePerMonth DECIMAL(18,2);
        DECLARE @TotalMonths INT;
        DECLARE @TotalArrears DECIMAL(18,2);
        DECLARE @CurrentDate DATE = GETDATE();

        -- Get old structure
        SELECT TOP 1 
            @OldStructureId = Id,
            @OldGross = GrossSalary
        FROM EmployeeSalaryStructure
        WHERE EmployeeId = @EmployeeId
          AND EffectiveFrom < @RevisionEffectiveDate
          AND Id <> @NewStructureId
        ORDER BY EffectiveFrom DESC;

        -- Get new structure gross
        SELECT @NewGross = GrossSalary
        FROM EmployeeSalaryStructure
        WHERE Id = @NewStructureId;

        -- Calculate difference
        SET @DifferencePerMonth = @NewGross - @OldGross;

        -- Calculate number of months
        SET @TotalMonths = DATEDIFF(MONTH, @RevisionEffectiveDate, @CurrentDate);

        -- Calculate total arrears
        SET @TotalArrears = @DifferencePerMonth * @TotalMonths;

        -- Insert arrears record
        INSERT INTO PayrollArrears
        (EmployeeId, RevisionEffectiveDate, RevisionApprovedDate,
         OldSalaryStructureId, NewSalaryStructureId,
         ArrearsPeriodFrom, ArrearsPeriodTo, TotalMonths,
         OldGrossSalary, NewGrossSalary, DifferencePerMonth, TotalArrearsAmount,
         PaymentStatus, CalculatedBy, CalculatedDate)
        VALUES
        (@EmployeeId, @RevisionEffectiveDate, GETDATE(),
         @OldStructureId, @NewStructureId,
         @RevisionEffectiveDate, DATEADD(DAY, -1, @CurrentDate), @TotalMonths,
         @OldGross, @NewGross, @DifferencePerMonth, @TotalArrears,
         'Pending', @CalculatedBy, GETDATE());

        COMMIT TRANSACTION;

        -- Return summary
        SELECT 
            @EmployeeId AS EmployeeId,
            @OldGross AS OldGrossSalary,
            @NewGross AS NewGrossSalary,
            @DifferencePerMonth AS DifferencePerMonth,
            @TotalMonths AS TotalMonths,
            @TotalArrears AS TotalArrearsAmount;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateEmployeePayroll]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_CalculateEmployeePayroll
-- Description: Single employee साठी monthly payroll calculate करणे
-- Parameters: 
--   @PayrollCycleId - Payroll cycle ID
--   @EmployeeId - Employee ID
--   @TotalWorkingDays - Total working days in month
--   @PresentDays - Actual present days
--   @PaidLeaveDays - Paid leave days
--   @CalculatedBy - User ID
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateEmployeePayroll]
    @PayrollCycleId INT,
    @EmployeeId INT,
    @TotalWorkingDays INT,
    @PresentDays DECIMAL(5,2),
    @PaidLeaveDays DECIMAL(5,2) = 0,
    @WeeklyOffDays INT = 0,
    @HolidayDays INT = 0,
    @OvertimeHours DECIMAL(5,2) = 0,
    @CalculatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @StructureId INT;
        DECLARE @BasicSalary DECIMAL(18,2) = 0;
        DECLARE @GrossSalary DECIMAL(18,2) = 0;
        DECLARE @TotalEarnings DECIMAL(18,2) = 0;
        DECLARE @TotalDeductions DECIMAL(18,2) = 0;
        DECLARE @NetSalary DECIMAL(18,2) = 0;
        DECLARE @LOPDays DECIMAL(5,2) = 0;
        DECLARE @LOPAmount DECIMAL(18,2) = 0;
        DECLARE @PerDaySalary DECIMAL(18,2) = 0;
        DECLARE @PayrollProcessingId INT;
        DECLARE @Month INT, @Year INT;

        -- Get payroll cycle details
        SELECT @Month = Month, @Year = Year 
        FROM PayrollCycle 
        WHERE Id = @PayrollCycleId;

        -- Get current salary structure
        SELECT TOP 1 @StructureId = Id, @GrossSalary = GrossSalary
        FROM EmployeeSalaryStructure 
        WHERE EmployeeId = @EmployeeId 
          AND IsCurrentStructure = 1 
          AND EffectiveFrom <= (SELECT StartDate FROM PayrollCycle WHERE Id = @PayrollCycleId)
        ORDER BY EffectiveFrom DESC;

        IF @StructureId IS NULL
        BEGIN
            RAISERROR('No active salary structure found for employee', 16, 1);
            RETURN;
        END

        -- Calculate LOP
        SET @LOPDays = @TotalWorkingDays - @PresentDays - @PaidLeaveDays;
        IF @LOPDays < 0 SET @LOPDays = 0;

        SET @PerDaySalary = @GrossSalary / @TotalWorkingDays;
        SET @LOPAmount = @PerDaySalary * @LOPDays;

        -- Check if payroll already exists
        SELECT @PayrollProcessingId = Id 
        FROM PayrollProcessing 
        WHERE PayrollCycleId = @PayrollCycleId AND EmployeeId = @EmployeeId;

        IF @PayrollProcessingId IS NULL
        BEGIN
            -- Create new payroll record
            INSERT INTO PayrollProcessing 
            (PayrollCycleId, EmployeeId, EmployeeSalaryStructureId, 
             TotalWorkingDays, PresentDays, PaidLeaveDays, WeeklyOffDays, HolidayDays,
             LOPDays, LOPAmount, OvertimeHours, 
             Status, CalculatedDate, CreatedBy, CreatedDate)
            VALUES 
            (@PayrollCycleId, @EmployeeId, @StructureId,
             @TotalWorkingDays, @PresentDays, @PaidLeaveDays, @WeeklyOffDays, @HolidayDays,
             @LOPDays, @LOPAmount, @OvertimeHours,
             'Calculated', GETDATE(), @CalculatedBy, GETDATE());

            SET @PayrollProcessingId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Update existing record
            UPDATE PayrollProcessing 
            SET TotalWorkingDays = @TotalWorkingDays,
                PresentDays = @PresentDays,
                PaidLeaveDays = @PaidLeaveDays,
                LOPDays = @LOPDays,
                LOPAmount = @LOPAmount,
                OvertimeHours = @OvertimeHours,
                LastRecalculatedDate = GETDATE(),
                UpdatedBy = @CalculatedBy,
                UpdatedDate = GETDATE()
            WHERE Id = @PayrollProcessingId;

            -- Delete existing component details
            DELETE FROM PayrollProcessingDetails WHERE PayrollProcessingId = @PayrollProcessingId;
        END

        -- Calculate component-wise salary
        DECLARE @ComponentId INT, @ComponentCode NVARCHAR(20), @ComponentName NVARCHAR(100);
        DECLARE @ComponentType NVARCHAR(20), @CalculationType NVARCHAR(20);
        DECLARE @Amount DECIMAL(18,2), @Percentage DECIMAL(5,2), @CalculationBase NVARCHAR(50);
        DECLARE @MonthlyAmount DECIMAL(18,2), @FinalAmount DECIMAL(18,2);
        DECLARE @DisplayOrder INT;

        DECLARE comp_cursor CURSOR FOR
        SELECT 
            esc.ComponentId, sc.ComponentCode, sc.ComponentName, sc.ComponentType,
            esc.CalculationType, esc.Amount, esc.Percentage, esc.CalculationBase,
            esc.MonthlyAmount, esc.DisplayOrder
        FROM EmployeeSalaryComponents esc
        INNER JOIN SalaryComponents sc ON esc.ComponentId = sc.Id
        WHERE esc.EmployeeSalaryStructureId = @StructureId 
          AND esc.IsActive = 1
        ORDER BY sc.ComponentType, esc.DisplayOrder;

        OPEN comp_cursor;
        FETCH NEXT FROM comp_cursor INTO @ComponentId, @ComponentCode, @ComponentName, @ComponentType,
            @CalculationType, @Amount, @Percentage, @CalculationBase, @MonthlyAmount, @DisplayOrder;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @FinalAmount = @MonthlyAmount;

            -- Adjust for LOP if earning component
            IF @ComponentType = 'Earning' AND @LOPDays > 0 AND @ComponentCode != 'OT'
            BEGIN
                SET @FinalAmount = @MonthlyAmount - ((@MonthlyAmount / @TotalWorkingDays) * @LOPDays);
            END

            -- Track Basic Salary
            IF @ComponentCode = 'BASIC'
                SET @BasicSalary = @FinalAmount;

            -- Insert component detail
            INSERT INTO PayrollProcessingDetails 
            (PayrollProcessingId, ComponentId, ComponentCode, ComponentName, ComponentType,
             CalculationType, CalculationBase, Percentage, Amount, DisplayOrder, 
             AdjustedForLOP, OriginalAmount)
            VALUES 
            (@PayrollProcessingId, @ComponentId, @ComponentCode, @ComponentName, @ComponentType,
             @CalculationType, @CalculationBase, @Percentage, @FinalAmount, @DisplayOrder,
             CASE WHEN @FinalAmount != @MonthlyAmount THEN 1 ELSE 0 END, @MonthlyAmount);

            -- Sum totals
            IF @ComponentType = 'Earning'
                SET @TotalEarnings = @TotalEarnings + @FinalAmount;
            ELSE
                SET @TotalDeductions = @TotalDeductions + @FinalAmount;

            FETCH NEXT FROM comp_cursor INTO @ComponentId, @ComponentCode, @ComponentName, @ComponentType,
                @CalculationType, @Amount, @Percentage, @CalculationBase, @MonthlyAmount, @DisplayOrder;
        END

        CLOSE comp_cursor;
        DEALLOCATE comp_cursor;

        -- Add active loan EMIs
        DECLARE @LoanEMI DECIMAL(18,2) = 0;
        SELECT @LoanEMI = ISNULL(SUM(emi.EMIAmount), 0)
        FROM LoanEMISchedule emi
        INNER JOIN EmployeeLoans el ON emi.LoanId = el.Id
        WHERE el.EmployeeId = @EmployeeId 
          AND emi.Status = 'Pending'
          AND MONTH(emi.EMIDueDate) = @Month
          AND YEAR(emi.EMIDueDate) = @Year;

        IF @LoanEMI > 0
        BEGIN
            SET @TotalDeductions = @TotalDeductions + @LoanEMI;
            
            INSERT INTO PayrollProcessingDetails 
            (PayrollProcessingId, ComponentId, ComponentCode, ComponentName, ComponentType,
             CalculationType, Amount, DisplayOrder)
            SELECT @PayrollProcessingId, 16, 'LOAN_EMI', 'Loan EMI', 'Deduction',
                   'Fixed', @LoanEMI, 8;
        END

        -- Calculate net salary
        SET @NetSalary = @TotalEarnings - @TotalDeductions;

        -- Update payroll processing totals
        UPDATE PayrollProcessing 
        SET BasicSalary = @BasicSalary,
            GrossSalary = @TotalEarnings,
            TotalEarnings = @TotalEarnings,
            TotalDeductions = @TotalDeductions,
            NetSalary = @NetSalary,
            LoanEMI = @LoanEMI,
            Status = 'Calculated'
        WHERE Id = @PayrollProcessingId;

        COMMIT TRANSACTION;

        -- Return summary
        SELECT 
            @PayrollProcessingId AS PayrollProcessingId,
            @EmployeeId AS EmployeeId,
            @BasicSalary AS BasicSalary,
            @TotalEarnings AS TotalEarnings,
            @TotalDeductions AS TotalDeductions,
            @NetSalary AS NetSalary,
            @LOPDays AS LOPDays,
            @LOPAmount AS LOPAmount;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateFullAndFinal]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_CalculateFullAndFinal
-- Description: Employee च्या final settlement चा calculation
-- Parameters: 
--   @EmployeeId - Employee ID
--   @LastWorkingDate - Last working date
--   @SeparationType - Type of separation
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateFullAndFinal]
    @EmployeeId INT,
    @LastWorkingDate DATE,
    @SeparationType NVARCHAR(50),
    @NoticePeriodDays INT = 0,
    @CalculatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @SettlementNumber NVARCHAR(50);
        DECLARE @JoiningDate DATE;
        DECLARE @TotalServiceYears DECIMAL(5,2);
        DECLARE @CurrentGross DECIMAL(18,2);
        DECLARE @LastMonthSalary DECIMAL(18,2);
        DECLARE @WorkingDaysInMonth INT;
        DECLARE @ProRataSalary DECIMAL(18,2);
        
        -- Leave encashment
        DECLARE @UnusedLeaves DECIMAL(5,2);
        DECLARE @LeaveEncashment DECIMAL(18,2);
        
        -- Gratuity
        DECLARE @GratuityAmount DECIMAL(18,2) = 0;
        DECLARE @IsEligibleForGratuity BIT = 0;
        
        -- Deductions
        DECLARE @LoanOutstanding DECIMAL(18,2) = 0;
        DECLARE @AdvanceOutstanding DECIMAL(18,2) = 0;
        DECLARE @NoticePeriodRecovery DECIMAL(18,2) = 0;
        
        -- Totals
        DECLARE @TotalEarnings DECIMAL(18,2);
        DECLARE @TotalDeductions DECIMAL(18,2);
        DECLARE @NetSettlement DECIMAL(18,2);

        -- Get employee details
        SELECT 
            @JoiningDate = JoiningDate,
            @CurrentGross = (SELECT TOP 1 GrossSalary FROM EmployeeSalaryStructure 
                            WHERE EmployeeId = @EmployeeId AND IsCurrentStructure = 1)
        FROM Employees
        WHERE Id = @EmployeeId;

        -- Calculate service years
        SET @TotalServiceYears = DATEDIFF(DAY, @JoiningDate, @LastWorkingDate) / 365.0;

        -- Calculate pro-rata salary
        SET @WorkingDaysInMonth = DAY(@LastWorkingDate);
        SET @LastMonthSalary = @CurrentGross;
        SET @ProRataSalary = (@CurrentGross / 30) * @WorkingDaysInMonth;

        -- Get unused leave balance
        SELECT @UnusedLeaves = ISNULL(SUM(TotalAvailable), 0)
        FROM LeaveBalances
        WHERE EmployeeId = @EmployeeId
          AND Year = YEAR(@LastWorkingDate)
          AND IsActive = 1;

        -- Calculate leave encashment (based on basic salary)
        DECLARE @BasicSalary DECIMAL(18,2) = @CurrentGross * 0.50; -- Assuming Basic is 50% of Gross
        SET @LeaveEncashment = (@BasicSalary / 30) * @UnusedLeaves;

        -- Calculate gratuity (if eligible - 5+ years service)
        IF @TotalServiceYears >= 5
        BEGIN
            SET @IsEligibleForGratuity = 1;
            -- Formula: (Last drawn salary * Years of service * 15) / 26
            SET @GratuityAmount = (@BasicSalary * @TotalServiceYears * 15) / 26;
            
            -- Cap at 20 lakhs
            IF @GratuityAmount > 2000000
                SET @GratuityAmount = 2000000;
        END

        -- Calculate outstanding loans
        SELECT @LoanOutstanding = ISNULL(SUM(OutstandingAmount), 0)
        FROM EmployeeLoans
        WHERE EmployeeId = @EmployeeId
          AND Status IN ('Active', 'Disbursed')
          AND IsFullyPaid = 0;

        -- Calculate outstanding advances
        SELECT @AdvanceOutstanding = ISNULL(SUM(OutstandingAmount), 0)
        FROM EmployeeAdvances
        WHERE EmployeeId = @EmployeeId
          AND IsFullyRecovered = 0;

        -- Calculate notice period recovery
        IF @NoticePeriodDays > 0 AND @SeparationType = 'Resignation'
        BEGIN
            SET @NoticePeriodRecovery = (@CurrentGross / 30) * @NoticePeriodDays;
        END

        -- Calculate totals
        SET @TotalEarnings = @ProRataSalary + @LeaveEncashment + @GratuityAmount;
        SET @TotalDeductions = @LoanOutstanding + @AdvanceOutstanding + @NoticePeriodRecovery;
        SET @NetSettlement = @TotalEarnings - @TotalDeductions;

        -- Generate settlement number
        SET @SettlementNumber = 'FNF-' + CAST(YEAR(@LastWorkingDate) AS NVARCHAR) + '-' +
                               RIGHT('0000' + CAST(@EmployeeId AS NVARCHAR), 4);

        -- Insert F&F record
        INSERT INTO FullAndFinalSettlement
        (SettlementNumber, EmployeeId, SeparationType, LastWorkingDate,
         NoticePeriodDays, NoticePeriodShortfall,
         LastMonthSalary, WorkingDaysInLastMonth, ProRataSalary,
         UnusedLeaveBalance, LeaveEncashmentAmount,
         TotalServiceYears, IsEligibleForGratuity, GratuityAmount,
         NoticePeriodRecovery,
         LoanOutstanding, AdvanceOutstanding,
         TotalEarnings, TotalDeductions, NetSettlementAmount,
         Status, CreatedBy, CreatedDate)
        VALUES
        (@SettlementNumber, @EmployeeId, @SeparationType, @LastWorkingDate,
         @NoticePeriodDays, @NoticePeriodDays,
         @LastMonthSalary, @WorkingDaysInMonth, @ProRataSalary,
         @UnusedLeaves, @LeaveEncashment,
         @TotalServiceYears, @IsEligibleForGratuity, @GratuityAmount,
         @NoticePeriodRecovery,
         @LoanOutstanding, @AdvanceOutstanding,
         @TotalEarnings, @TotalDeductions, @NetSettlement,
         'Calculated', @CalculatedBy, GETDATE());

        COMMIT TRANSACTION;

        -- Return settlement summary
        SELECT 
            @SettlementNumber AS SettlementNumber,
            @ProRataSalary AS ProRataSalary,
            @LeaveEncashment AS LeaveEncashment,
            @GratuityAmount AS GratuityAmount,
            @LoanOutstanding AS LoanOutstanding,
            @AdvanceOutstanding AS AdvanceOutstanding,
            @NoticePeriodRecovery AS NoticePeriodRecovery,
            @TotalEarnings AS TotalEarnings,
            @TotalDeductions AS TotalDeductions,
            @NetSettlement AS NetSettlement;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateProfessionalTax]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_CalculateProfessionalTax
-- Description: राज्यानुसार व्यावसायिक कर calculate करणे
-- Parameters: 
--   @GrossSalary - Gross monthly salary
--   @StateCode - State code (MH, KA, TN, etc.)
--   @Month - Month number (1-12)
-- Returns: PT Amount
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateProfessionalTax]
    @GrossSalary DECIMAL(18,2),
    @StateCode NVARCHAR(10),
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PTAmount DECIMAL(18,2) = 0;
    DECLARE @FinancialYear NVARCHAR(10) = 
        CASE 
            WHEN @Month >= 4 THEN CAST(YEAR(GETDATE()) AS NVARCHAR) + '-' + RIGHT(CAST(YEAR(GETDATE()) + 1 AS NVARCHAR), 2)
            ELSE CAST(YEAR(GETDATE()) - 1 AS NVARCHAR) + '-' + RIGHT(CAST(YEAR(GETDATE()) AS NVARCHAR), 2)
        END;

    -- Get PT slab
    SELECT TOP 1 @PTAmount = PTAmount
    FROM ProfessionalTaxSlabs
    WHERE StateCode = @StateCode
      AND @GrossSalary BETWEEN MinSalary AND MaxSalary
      AND (ApplicableMonth IS NULL OR ApplicableMonth = @Month)
      AND FinancialYear = @FinancialYear
      AND IsActive = 1
    ORDER BY 
        CASE WHEN ApplicableMonth = @Month THEN 0 ELSE 1 END,
        MinSalary DESC;

    SELECT @PTAmount AS ProfessionalTax;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateSalaryBreakdown]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 28. Calculate Salary Breakdown
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateSalaryBreakdown]
(
    @TemplateId INT,
    @CTC DECIMAL(18,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sc.ComponentCode,
        sc.ComponentName,
        sc.ComponentType,
        stc.CalculationType,
        stc.Percentage,
        CASE 
            WHEN stc.CalculationType = 'Percentage' THEN (@CTC * stc.Percentage / 100) / 12
            WHEN stc.CalculationType = 'Fixed' THEN stc.FixedAmount
            ELSE stc.MonthlyAmount
        END AS MonthlyAmount,
        CASE 
            WHEN stc.CalculationType = 'Percentage' THEN @CTC * stc.Percentage / 100
            WHEN stc.CalculationType = 'Fixed' THEN stc.FixedAmount * 12
            ELSE stc.AnnualAmount
        END AS AnnualAmount,
        stc.DisplayOrder
    FROM SalaryTemplateComponents stc
    INNER JOIN SalaryComponents sc ON stc.ComponentId = sc.Id
    WHERE stc.TemplateId = @TemplateId AND stc.IsActive = 1
    ORDER BY sc.ComponentType, stc.DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CalculateTDS]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_CalculateTDS
-- Description: महिन्याचा TDS calculate करणे (Projected annual income based)
-- Parameters: 
--   @EmployeeId - Employee ID
--   @MonthlyGross - Monthly gross salary
--   @Month - Current month
--   @FinancialYear - Financial year
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CalculateTDS]
    @EmployeeId INT,
    @MonthlyGross DECIMAL(18,2),
    @Month INT,
    @FinancialYear NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AnnualGross DECIMAL(18,2);
    DECLARE @TaxableIncome DECIMAL(18,2);
    DECLARE @TotalDeductions DECIMAL(18,2) = 0;
    DECLARE @TaxRegime NVARCHAR(20) = 'New';
    DECLARE @StandardDeduction DECIMAL(18,2) = 50000;
    DECLARE @TotalTax DECIMAL(18,2) = 0;
    DECLARE @MonthlyTDS DECIMAL(18,2) = 0;
    DECLARE @RemainingMonths INT;

    -- Calculate annual projected income
    SET @AnnualGross = @MonthlyGross * 12;
    
    -- Get employee's tax regime selection
    SELECT @TaxRegime = ISNULL(SelectedTaxRegime, 'New'),
           @TotalDeductions = ISNULL(TotalDeductions, 0)
    FROM EmployeeTaxDeclarations
    WHERE EmployeeId = @EmployeeId 
      AND FinancialYear = @FinancialYear;

    -- Calculate taxable income
    IF @TaxRegime = 'Old'
    BEGIN
        SET @TaxableIncome = @AnnualGross - @StandardDeduction - @TotalDeductions;
    END
    ELSE
    BEGIN
        SET @TaxableIncome = @AnnualGross; -- No deductions in new regime
    END

    -- Calculate tax based on slabs
    DECLARE @SlabMin DECIMAL(18,2), @SlabMax DECIMAL(18,2), @SlabRate DECIMAL(5,2);
    DECLARE @SlabTax DECIMAL(18,2);

    DECLARE slab_cursor CURSOR FOR
    SELECT MinIncome, MaxIncome, TaxPercentage
    FROM TDSSlabs
    WHERE TaxRegime = @TaxRegime
      AND FinancialYear = @FinancialYear
      AND IsActive = 1
      AND ApplicableForAge = 'All'
    ORDER BY SlabNumber;

    OPEN slab_cursor;
    FETCH NEXT FROM slab_cursor INTO @SlabMin, @SlabMax, @SlabRate;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @TaxableIncome > @SlabMin
        BEGIN
            DECLARE @TaxableInSlab DECIMAL(18,2) = 
                CASE 
                    WHEN @TaxableIncome > @SlabMax THEN @SlabMax - @SlabMin
                    ELSE @TaxableIncome - @SlabMin
                END;

            SET @SlabTax = @TaxableInSlab * @SlabRate / 100;
            SET @TotalTax = @TotalTax + @SlabTax;
        END

        FETCH NEXT FROM slab_cursor INTO @SlabMin, @SlabMax, @SlabRate;
    END

    CLOSE slab_cursor;
    DEALLOCATE slab_cursor;

    -- Add 4% Health & Education Cess
    SET @TotalTax = @TotalTax * 1.04;

    -- Calculate monthly TDS
    SET @RemainingMonths = 12 - @Month + 1; -- Remaining months in FY
    SET @MonthlyTDS = CASE WHEN @RemainingMonths > 0 THEN @TotalTax / @RemainingMonths ELSE 0 END;

    -- Round to nearest rupee
    SET @MonthlyTDS = ROUND(@MonthlyTDS, 0);

    SELECT 
        @TaxableIncome AS TaxableIncome,
        @TotalTax AS AnnualTax,
        @MonthlyTDS AS MonthlyTDS,
        @TaxRegime AS TaxRegime;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CancelLeave]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_CancelLeave]
    @LeaveRequestId INT,
    @CancelledBy    INT,
    @CancelReason   NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @EmployeeId INT, @LeaveTypeId INT, @TotalDays DECIMAL(5,1),
                @StartDate DATE, @CurrentStatus NVARCHAR(20);

        SELECT @EmployeeId = EmployeeId,
               @LeaveTypeId = LeaveTypeId,
               @TotalDays = TotalDays,
               @StartDate = StartDate,
               @CurrentStatus = Status
        FROM LeaveRequests
        WHERE Id = @LeaveRequestId AND IsDeleted = 0;

        IF @EmployeeId IS NULL
        BEGIN
            RAISERROR('Leave request not found.', 16, 1);
            RETURN;
        END

        -- Update leave request
        UPDATE LeaveRequests
        SET Status = 'Cancelled',
            CancelledDate = GETDATE(),
            CancelReason = @CancelReason,
            UpdatedBy = @CancelledBy,
            UpdatedDate = GETDATE()
        WHERE Id = @LeaveRequestId;

        -- Restore balance based on previous status
        IF @CurrentStatus = 'Pending'
        BEGIN
            UPDATE LeaveBalances
            SET TotalPending = TotalPending - @TotalDays,
                UpdatedBy = @CancelledBy,
                UpdatedDate = GETDATE()
            WHERE EmployeeId = @EmployeeId
              AND LeaveTypeId = @LeaveTypeId
              AND Year = YEAR(@StartDate);
        END
        ELSE IF @CurrentStatus = 'Approved'
        BEGIN
            UPDATE LeaveBalances
            SET TotalUsed = TotalUsed - @TotalDays,
                UpdatedBy = @CancelledBy,
                UpdatedDate = GETDATE()
            WHERE EmployeeId = @EmployeeId
              AND LeaveTypeId = @LeaveTypeId
              AND Year = YEAR(@StartDate);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ChangePassword]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_ChangePassword]  
    @UserId INT,  
    @NewPasswordHash NVARCHAR(500),
	@NewPasswordSalt NVARCHAR(500),
    @UpdatedBy INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    UPDATE Users  
    SET   
        PasswordHash = @NewPasswordHash,  
        UpdatedDate = GETDATE(),  
		 PasswordSalt = @NewPasswordSalt, 
        PasswordChangedDate = GETDATE()  
    WHERE Id = @UserId;  
  
    SELECT @@ROWCOUNT AS RowsAffected;  
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckEmployeeEmailExists]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 31. CHECK EMAIL EXISTS
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CheckEmployeeEmailExists]
    @Email NVARCHAR(255),
    @ExcludeId INT = NULL,
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1 FROM Employees 
        WHERE Email = @Email 
        AND IsDeleted = 0
        AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
    )
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckLoanEligibility]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 27. Check Loan Eligibility
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CheckLoanEligibility]
(
    @EmployeeId INT,
    @LoanTypeId INT,
    @RequestedAmount DECIMAL(18,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GrossSalary DECIMAL(18,2);
    DECLARE @ExistingEMI DECIMAL(18,2);
    DECLARE @ExistingOutstanding DECIMAL(18,2);
    DECLARE @MaxAmount DECIMAL(18,2);
    DECLARE @MinAmount DECIMAL(18,2);
    DECLARE @MaxLoanMultiplier DECIMAL(5,2);
    DECLARE @IsEligible BIT = 1;
    DECLARE @Remarks NVARCHAR(500) = '';

    -- Get employee salary
    SELECT @GrossSalary = GrossSalary 
    FROM EmployeeSalaryStructure 
    WHERE EmployeeId = @EmployeeId AND IsCurrentStructure = 1;

    -- Get existing loans
    SELECT @ExistingEMI = ISNULL(SUM(EMIAmount), 0),
           @ExistingOutstanding = ISNULL(SUM(OutstandingAmount), 0)
    FROM EmployeeLoans 
    WHERE EmployeeId = @EmployeeId 
      AND Status IN ('Active', 'Disbursed') 
      AND IsFullyPaid = 0;

    -- Get loan type limits
    SELECT @MaxAmount = MaxAmount,
           @MinAmount = MinAmount,
           @MaxLoanMultiplier = MaxLoanMultiplier
    FROM LoanTypes WHERE Id = @LoanTypeId;

    -- Check minimum amount
    IF @RequestedAmount < ISNULL(@MinAmount, 0)
    BEGIN
        SET @IsEligible = 0;
        SET @Remarks = @Remarks + 'Amount below minimum limit. ';
    END

    -- Check maximum amount
    IF @RequestedAmount > ISNULL(@MaxAmount, 9999999)
    BEGIN
        SET @IsEligible = 0;
        SET @Remarks = @Remarks + 'Amount exceeds maximum limit. ';
    END

    -- Check salary multiplier
    IF @MaxLoanMultiplier IS NOT NULL AND @RequestedAmount > (@GrossSalary * @MaxLoanMultiplier)
    BEGIN
        SET @IsEligible = 0;
        SET @Remarks = @Remarks + 'Amount exceeds salary multiplier limit. ';
    END

    -- Check EMI to salary ratio (50% max)
    DECLARE @EstimatedEMI DECIMAL(18,2) = @RequestedAmount / 12; -- Simplified
    IF ((@ExistingEMI + @EstimatedEMI) / @GrossSalary) > 0.5
    BEGIN
        SET @IsEligible = 0;
        SET @Remarks = @Remarks + 'Total EMI exceeds 50% of salary. ';
    END

    SELECT 
        @IsEligible AS IsEligible,
        @GrossSalary AS GrossSalary,
        @ExistingEMI AS ExistingEMI,
        @ExistingOutstanding AS ExistingOutstanding,
        @MaxAmount AS MaxAllowedAmount,
        ISNULL(@GrossSalary * @MaxLoanMultiplier, @MaxAmount) AS MaxEligibleAmount,
        @Remarks AS Remarks;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CleanExpiredTokens]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored Procedure: Clean Expired Tokens (for maintenance)
CREATE PROCEDURE [dbo].[sp_CleanExpiredTokens]
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM RefreshTokens
    WHERE ExpiryDate < DATEADD(DAY, -30, GETUTCDATE());
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CleanupExpiredTokens]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 14. sp_CleanupExpiredTokens
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CleanupExpiredTokens]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RevokedCount INT;
    DECLARE @DeletedCount INT;

    -- Mark expired tokens as revoked
    UPDATE RefreshTokens
    SET 
        IsRevoked = 1,
        RevokedDate = GETDATE()
    WHERE ExpiryDate < GETDATE()
    AND IsRevoked = 0;

    SET @RevokedCount = @@ROWCOUNT;

    -- Delete old tokens (older than 30 days)
    DELETE FROM RefreshTokens
    WHERE RevokedDate < DATEADD(DAY, -30, GETDATE());

    SET @DeletedCount = @@ROWCOUNT;

    SELECT @RevokedCount AS RevokedCount, @DeletedCount AS DeletedCount;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CloseLoan]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 20. Close Loan
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CloseLoan]
(
    @LoanId INT,
    @ClosureType NVARCHAR(50) = 'Manual', -- 'Manual', 'Prepayment', 'Waiver'
    @ClosureRemarks NVARCHAR(500) = NULL,
    @ClosedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE EmployeeLoans 
    SET Status = 'Closed',
        IsFullyPaid = 1,
        ClosureDate = GETDATE(),
        ClosureType = @ClosureType,
        ClosureRemarks = @ClosureRemarks,
        ClosedBy = @ClosedBy,
        UpdatedBy = @ClosedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @LoanId AND Status IN ('Active', 'Disbursed');

    -- Cancel pending EMIs
    UPDATE LoanEMISchedule 
    SET Status = 'Cancelled',
        UpdatedDate = GETDATE()
    WHERE LoanId = @LoanId AND Status = 'Pending';

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateLoanApplication]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN APPLICATION
-- =============================================

-- 5. Create Loan Application
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CreateLoanApplication]
(
    @EmployeeId INT,
    @LoanTypeId INT,
    @RequestedAmount DECIMAL(18,2),
    @TenureMonths INT,
    @Purpose NVARCHAR(500) = NULL,
    @GuarantorEmployeeId INT = NULL,
    @GuarantorName NVARCHAR(200) = NULL,
    @GuarantorRelation NVARCHAR(100) = NULL,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @InterestRate DECIMAL(5,2);
        DECLARE @EMIAmount DECIMAL(18,2);
        DECLARE @TotalRepayable DECIMAL(18,2);
        DECLARE @LoanNumber NVARCHAR(50);
        DECLARE @ProcessingFee DECIMAL(18,2);

        -- Get loan type details
        SELECT @InterestRate = InterestRate,
               @ProcessingFee = @RequestedAmount * ProcessingFeePercent / 100
        FROM LoanTypes 
        WHERE Id = @LoanTypeId AND IsDeleted = 0;

        IF @InterestRate IS NULL
        BEGIN
            RAISERROR('Loan type not found', 16, 1);
            RETURN;
        END

        -- Calculate EMI
        DECLARE @MonthlyInterestRate DECIMAL(18,10) = @InterestRate / 12 / 100;
        
        IF @MonthlyInterestRate > 0
        BEGIN
            -- EMI Formula: P * r * (1+r)^n / ((1+r)^n - 1)
            SET @EMIAmount = @RequestedAmount * @MonthlyInterestRate * 
                             POWER(1 + @MonthlyInterestRate, @TenureMonths) / 
                             (POWER(1 + @MonthlyInterestRate, @TenureMonths) - 1);
        END
        ELSE
        BEGIN
            SET @EMIAmount = @RequestedAmount / @TenureMonths;
        END

        SET @EMIAmount = ROUND(@EMIAmount, 2);
        SET @TotalRepayable = @EMIAmount * @TenureMonths;

        -- Generate loan number
        SET @LoanNumber = 'LOAN-' + FORMAT(GETDATE(), 'yyyyMM') + '-' + RIGHT('0000' + CAST(@EmployeeId AS VARCHAR), 4) + '-' + RIGHT('000' + CAST((SELECT COUNT(*) + 1 FROM EmployeeLoans WHERE EmployeeId = @EmployeeId) AS VARCHAR), 3);

        -- Insert loan application
        INSERT INTO EmployeeLoans 
        (
            LoanNumber, EmployeeId, LoanTypeId, LoanAmount, InterestRate, TenureMonths,
            EMIAmount, TotalRepayableAmount, ProcessingFee, ApplicationDate, RequestedAmount, Purpose,
            GuarantorEmployeeId, GuarantorName, GuarantorRelation,
            Status, OutstandingAmount, OutstandingPrincipal, TotalAmountPaid, PrincipalPaid, InterestPaid,
            TotalEMIsPaid, IsFullyPaid, CreatedBy, CreatedDate, IsActive, IsDeleted
        )
        VALUES 
        (
            @LoanNumber, @EmployeeId, @LoanTypeId, @RequestedAmount, @InterestRate, @TenureMonths,
            @EMIAmount, @TotalRepayable, @ProcessingFee, GETDATE(), @RequestedAmount, @Purpose,
            @GuarantorEmployeeId, @GuarantorName, @GuarantorRelation,
            'Pending', 0, 0, 0, 0, 0, 0, 0, @CreatedBy, GETDATE(), 1, 0
        );

        DECLARE @LoanId INT = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT @LoanId AS LoanId, @LoanNumber AS LoanNumber, @EMIAmount AS EMIAmount, @TotalRepayable AS TotalRepayable;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateLoanType]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 3. Create Loan Type
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CreateLoanType]
(
    @LoanTypeCode NVARCHAR(50),
    @LoanTypeName NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @InterestRate DECIMAL(5,2) = 0,
    @MaxAmount DECIMAL(18,2) = NULL,
    @MinAmount DECIMAL(18,2) = NULL,
    @MaxTenureMonths INT = 60,
    @MinTenureMonths INT = 1,
    @RequiresGuarantor BIT = 0,
    @RequiresCollateral BIT = 0,
    @MaxLoanMultiplier DECIMAL(5,2) = NULL,
    @ProcessingFeePercent DECIMAL(5,2) = 0,
    @DisplayOrder INT = 0,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if loan type code already exists
    IF EXISTS (SELECT 1 FROM LoanTypes WHERE LoanTypeCode = @LoanTypeCode AND IsDeleted = 0)
    BEGIN
        RAISERROR('Loan type code already exists', 16, 1);
        RETURN;
    END

    INSERT INTO LoanTypes 
    (
        LoanTypeCode, LoanTypeName, Description, InterestRate,
        MaxAmount, MinAmount, MaxTenureMonths, MinTenureMonths,
        RequiresGuarantor, RequiresCollateral, MaxLoanMultiplier,
        ProcessingFeePercent, DisplayOrder, IsActive, CreatedBy, CreatedDate, IsDeleted
    )
    VALUES 
    (
        @LoanTypeCode, @LoanTypeName, @Description, @InterestRate,
        @MaxAmount, @MinAmount, @MaxTenureMonths, @MinTenureMonths,
        @RequiresGuarantor, @RequiresCollateral, @MaxLoanMultiplier,
        @ProcessingFeePercent, @DisplayOrder, 1, @CreatedBy, GETDATE(), 0
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS LoanTypeId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreatePayment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreatePayment]
    @EmployeeId INT,
    @Amount DECIMAL(10, 2),
    @OrderId NVARCHAR(100),
    @TransactionId NVARCHAR(100),
    @Description NVARCHAR(255),
    @PaymentMethod NVARCHAR(50),
    @PaymentStatus NVARCHAR(50),
    @CardNumber NVARCHAR(MAX) = NULL,
    @BankName NVARCHAR(50) = NULL,
    @IFSCCode NVARCHAR(11) = NULL,
    @AccountNumber NVARCHAR(20) = NULL,
    @UpiId NVARCHAR(255) = NULL,
    @WalletName NVARCHAR(50) = NULL,
    @WalletPhone NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Payments] (
        [EmployeeId], [Amount], [OrderId], [TransactionId], [Description], 
        [PaymentMethod], [PaymentStatus], [CardNumber], [BankName], [IFSCCode],
        [AccountNumber], [UpiId], [WalletName], [WalletPhone], [CreatedDate], [ModifiedDate]
    )
    VALUES (
        @EmployeeId, @Amount, @OrderId, @TransactionId, @Description,
        @PaymentMethod, @PaymentStatus, @CardNumber, @BankName, @IFSCCode,
        @AccountNumber, @UpiId, @WalletName, @WalletPhone, GETUTCDATE(), GETUTCDATE()
    );
    
    SELECT [Id], [EmployeeId], [Amount], [OrderId], [TransactionId], [PaymentStatus], [CreatedDate]
    FROM [dbo].[Payments]
    WHERE [Id] = SCOPE_IDENTITY();
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_CreatePayrollCycle]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. Create Payroll Cycle
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CreatePayrollCycle]
(
    @CycleName NVARCHAR(100),
    @CycleCode NVARCHAR(50),
    @PeriodType NVARCHAR(50),
    @StartDate DATE,
    @EndDate DATE,
    @FinancialYear NVARCHAR(20),
    @Month INT,
    @Year INT,
    @Status NVARCHAR(50),
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PayrollCycle
    (
        CycleName, CycleCode, PeriodType, StartDate, EndDate,
        FinancialYear, Month, Year, Status,
        CreatedBy, CreatedDate, IsActive
    )
    VALUES
    (
        @CycleName, @CycleCode, @PeriodType, @StartDate, @EndDate,
        @FinancialYear, @Month, @Year, @Status,
        @CreatedBy, GETDATE(), 1
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS CycleId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateRole]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   9. CREATE ROLE
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_CreateRole]
    @RoleName NVARCHAR(100),
    @RoleDescription NVARCHAR(500) = NULL,
    @CreatedBy INT = NULL,
    @NewRoleId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 
            FROM Roles 
            WHERE RoleName = @RoleName
        )
        BEGIN
            SELECT 0 AS Success, 'Role name already exists' AS Message;
            RETURN;
        END

        INSERT INTO Roles
        (
            RoleName,
            Description,
            RoleDescription,
            IsActive,
            CreatedDate
        )
        VALUES
        (
            @RoleName,
            @RoleDescription,
            @RoleDescription,
            1,
            GETDATE()
        );

        SET @NewRoleId = SCOPE_IDENTITY();

        SELECT 
            1 AS Success, 
            'Role created successfully' AS Message,
            @NewRoleId AS RoleId;
    END TRY
    BEGIN CATCH
        SELECT 
            0 AS Success, 
            ERROR_MESSAGE() AS Message,
            NULL AS RoleId;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateSalaryComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. Create Salary Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CreateSalaryComponent]
(
    @ComponentCode NVARCHAR(50),
    @ComponentName NVARCHAR(100),
    @ComponentType NVARCHAR(50),
    @Category NVARCHAR(50),
    @CalculationType NVARCHAR(50),
    @CalculationBase NVARCHAR(50),
    @DefaultPercentage DECIMAL(5,2) = NULL,
    @DefaultAmount DECIMAL(18,2) = NULL,
    @DisplayOrder INT = 0,
    @IsStatutory BIT = 0,
    @IsTaxable BIT = 1,
    @FormulaExpression NVARCHAR(500) = NULL,
    @MinAmount DECIMAL(18,2) = NULL,
    @MaxAmount DECIMAL(18,2) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if component code already exists
    IF EXISTS (SELECT 1 FROM SalaryComponents WHERE ComponentCode = @ComponentCode AND IsDeleted = 0)
    BEGIN
        RAISERROR('Component code already exists', 16, 1);
        RETURN;
    END

    INSERT INTO SalaryComponents 
    (
        ComponentCode, ComponentName, ComponentType, Category,
        CalculationType, CalculationBase, DefaultPercentage, DefaultAmount,
        DisplayOrder, IsStatutory, IsTaxable, FormulaExpression,
        MinAmount, MaxAmount, Description, Remarks,
        IsActive, CreatedBy, CreatedDate, IsDeleted
    )
    VALUES 
    (
        @ComponentCode, @ComponentName, @ComponentType, @Category,
        @CalculationType, @CalculationBase, @DefaultPercentage, @DefaultAmount,
        @DisplayOrder, @IsStatutory, @IsTaxable, @FormulaExpression,
        @MinAmount, @MaxAmount, @Description, @Remarks,
        1, @CreatedBy, GETDATE(), 0
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS ComponentId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateSalaryTemplate]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 11. Create Salary Template
-- =============================================
CREATE   PROCEDURE [dbo].[sp_CreateSalaryTemplate]
(
    @TemplateCode NVARCHAR(50),
    @TemplateName NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @DepartmentId INT = NULL,
    @DesignationId INT = NULL,
    @GradeLevel NVARCHAR(50) = NULL,
    @TotalCTC DECIMAL(18,2),
    @GrossSalary DECIMAL(18,2),
    @NetSalary DECIMAL(18,2),
    @TotalEarnings DECIMAL(18,2),
    @TotalDeductions DECIMAL(18,2),
    @EmployerContributions DECIMAL(18,2) = 0,
    @CreatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if template code already exists
    IF EXISTS (SELECT 1 FROM SalaryTemplates WHERE TemplateCode = @TemplateCode AND IsDeleted = 0)
    BEGIN
        RAISERROR('Template code already exists', 16, 1);
        RETURN;
    END

    INSERT INTO SalaryTemplates 
    (
        TemplateCode, TemplateName, Description, DepartmentId,
        DesignationId, GradeLevel, TotalCTC, GrossSalary, NetSalary,
        TotalEarnings, TotalDeductions, EmployerContributions,
        IsActive, CreatedBy, CreatedDate, IsDeleted
    )
    VALUES 
    (
        @TemplateCode, @TemplateName, @Description, @DepartmentId,
        @DesignationId, @GradeLevel, @TotalCTC, @GrossSalary, @NetSalary,
        @TotalEarnings, @TotalDeductions, @EmployerContributions,
        1, @CreatedBy, GETDATE(), 0
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS TemplateId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateTicket]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 2: Create Ticket
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateTicket]
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @TicketType NVARCHAR(50),
    @Priority NVARCHAR(50),
    @CreatedBy INT,
    @AssignedTo INT = NULL,
    @DueDate DATETIME = NULL,
    @StepsToReproduce NVARCHAR(MAX) = NULL,
    @ExpectedResult NVARCHAR(MAX) = NULL,
    @ActualResult NVARCHAR(MAX) = NULL,
    @Environment NVARCHAR(200) = NULL,
    @TicketId INT OUTPUT,
    @TicketNumber NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC sp_GenerateTicketNumber @TicketNumber OUTPUT;
        
        INSERT INTO Tickets (
            TicketNumber, Title, Description, TicketType, Priority,
            Status, CreatedBy, AssignedTo, DueDate,
            StepsToReproduce, ExpectedResult, ActualResult, Environment
        )
        VALUES (
            @TicketNumber, @Title, @Description, @TicketType, @Priority,
            CASE WHEN @AssignedTo IS NULL THEN 'New' ELSE 'Assigned' END,
            @CreatedBy, @AssignedTo, @DueDate,
            @StepsToReproduce, @ExpectedResult, @ActualResult, @Environment
        );
        
        SET @TicketId = SCOPE_IDENTITY();
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
        VALUES (@TicketId, @CreatedBy, 'Created', 'Ticket created');
        
        IF @AssignedTo IS NOT NULL
        BEGIN
            INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
            VALUES (@TicketId, @CreatedBy, 'Assigned', CAST(@AssignedTo AS NVARCHAR));
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_CreateUser]
    @Username NVARCHAR(100),
    @FullName NVARCHAR(200),
    @Email NVARCHAR(200),
    @PhoneNumber NVARCHAR(20) = NULL,
    @PasswordHash NVARCHAR(500),
    @PasswordSalt NVARCHAR(255),
    @RoleIds NVARCHAR(500),
    @DepartmentId INT = NULL,
    @DesignationId INT = NULL,
    @EmployeeCode NVARCHAR(50) = NULL,
    @CreatedBy INT,
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1
            FROM Users
            WHERE Email = @Email
              AND ISNULL(IsDeleted, 0) = 0
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Success,
                'Email already exists' AS Message,
                NULL AS UserId;

            RETURN;
        END;

        IF EXISTS (
            SELECT 1
            FROM Users
            WHERE Username = @Username
              AND ISNULL(IsDeleted, 0) = 0
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Success,
                'Username already exists' AS Message,
                NULL AS UserId;

            RETURN;
        END;

        DECLARE @FirstName NVARCHAR(100);
        DECLARE @LastName NVARCHAR(100);

        SET @FullName = LTRIM(RTRIM(@FullName));

        SET @FirstName =
            CASE
                WHEN CHARINDEX(' ', @FullName) > 0
                    THEN LEFT(@FullName, CHARINDEX(' ', @FullName) - 1)
                ELSE @FullName
            END;

        SET @LastName =
            CASE
                WHEN CHARINDEX(' ', @FullName) > 0
                    THEN LTRIM(SUBSTRING(@FullName, CHARINDEX(' ', @FullName) + 1, LEN(@FullName)))
                ELSE ''
            END;

        INSERT INTO Users
        (
            Username,
            FullName,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            PasswordHash,
            PasswordSalt,
            IsActive,
            IsEmailVerified,
            IsDeleted,
            CreatedBy,
            CreatedDate
        )
        VALUES
        (
            @Username,
            @FullName,
            @FirstName,
            @LastName,
            @Email,
            @PhoneNumber,
            @PasswordHash,
            ISNULL(@PasswordSalt, 'BCryptEmbeddedSalt'),
            1,
            0,
            0,
            @CreatedBy,
            GETDATE()
        );

        SET @NewUserId = SCOPE_IDENTITY();

        IF @RoleIds IS NOT NULL AND LEN(@RoleIds) > 0
        BEGIN
            INSERT INTO UserRoles
            (
                UserId,
                RoleId,
                AssignedBy,
                AssignedDate,
                IsActive
            )
            SELECT
                @NewUserId,
                TRY_CAST(LTRIM(RTRIM(value)) AS INT),
                @CreatedBy,
                GETDATE(),
                1
            FROM STRING_SPLIT(@RoleIds, ',')
            WHERE TRY_CAST(LTRIM(RTRIM(value)) AS INT) IS NOT NULL;
        END;

        IF @DepartmentId IS NOT NULL
        BEGIN
            INSERT INTO Employees
            (
                Name,
                Email,
                DepartmentId,
                PhoneNumber,
                EmployeeCode,
                Designation,
                IsActive,
                IsDeleted,
                CreatedBy,
                CreatedDate
            )
            VALUES
            (
                @FullName,
                @Email,
                @DepartmentId,
                @PhoneNumber,
                @EmployeeCode,
                (
                    SELECT TOP 1 DesignationName
                    FROM Designations
                    WHERE Id = @DesignationId
                      AND IsActive = 1
                ),
                1,
                0,
                @CreatedBy,
                GETDATE()
            );
        END;

        COMMIT TRANSACTION;

        SELECT
            1 AS Success,
            'User created successfully' AS Message,
            @NewUserId AS UserId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT
            0 AS Success,
            ERROR_MESSAGE() AS Message,
            NULL AS UserId;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteDepartment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteDepartment]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[Departments]
    WHERE [Id] = @Id;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 7. SOFT DELETE EMPLOYEE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_DeleteEmployee]
    @Id INT,
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees
    SET 
        IsDeleted = 1,
        IsActive = 0,
        DeletedBy = @DeletedBy,
        DeletedDate = GETDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteHoliday]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_DeleteHoliday]
    @Id        INT,
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Holidays
    SET IsDeleted = 1,
        IsActive = 0,
        UpdatedBy = @DeletedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRole]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   11. DELETE ROLE - SOFT DELETE
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_DeleteRole]
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 
            FROM UserRoles 
            WHERE RoleId = @RoleId 
              AND IsActive = 1
        )
        BEGIN
            SELECT 0 AS Success, 'Cannot delete role because active users are assigned to this role' AS Message;
            RETURN;
        END

        UPDATE Roles
        SET 
            IsActive = 0,
            UpdatedDate = GETDATE()
        WHERE Id = @RoleId;

        SELECT 1 AS Success, 'Role deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteSalaryComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 6. Delete Salary Component (Soft Delete)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_DeleteSalaryComponent]
    @ComponentId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if component is in use
    IF EXISTS (SELECT 1 FROM EmployeeSalaryComponents WHERE ComponentId = @ComponentId AND IsActive = 1)
    BEGIN
        RAISERROR('Component is in use and cannot be deleted', 16, 1);
        RETURN;
    END

    UPDATE SalaryComponents 
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedBy = @DeletedBy,
        DeletedDate = GETDATE()
    WHERE Id = @ComponentId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteSalaryTemplate]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 13. Delete Salary Template (Soft Delete)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_DeleteSalaryTemplate]
    @TemplateId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if template is in use
    IF EXISTS (SELECT 1 FROM EmployeeSalaryStructure WHERE TemplateId = @TemplateId AND IsCurrentStructure = 1)
    BEGIN
        RAISERROR('Template is in use and cannot be deleted', 16, 1);
        RETURN;
    END

    UPDATE SalaryTemplates 
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedBy = @DeletedBy,
        DeletedDate = GETDATE()
    WHERE Id = @TemplateId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteStudent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteStudent]  
    @StudentId NVARCHAR(50),  
    @DeletedBy INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
      
    UPDATE Students  
    SET   
        IsDeleted = 1,  
        IsActive = 0,  
        DeletedBy = @DeletedBy,  
        DeletedDate = GETDATE()  
    WHERE StudentId = @StudentId;  
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteTicket]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 14: Delete Ticket
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteTicket]
    @TicketId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Tickets
        SET IsDeleted = 1, UpdatedDate = GETDATE()
        WHERE TicketId = @TicketId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
        VALUES (@TicketId, @DeletedBy, 'Deleted', 'Ticket deleted');
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Ticket deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteTicketAttachment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 12: Delete Attachment
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteTicketAttachment]
    @AttachmentId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @TicketId INT;
        DECLARE @FileName NVARCHAR(255);
        
        SELECT @TicketId = TicketId, @FileName = FileName 
        FROM TicketAttachments 
        WHERE AttachmentId = @AttachmentId;
        
        UPDATE TicketAttachments
        SET IsDeleted = 1
        WHERE AttachmentId = @AttachmentId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, OldValue)
        VALUES (@TicketId, @DeletedBy, 'Attachment Deleted', @FileName);
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Attachment deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   5. DELETE USER - SOFT DELETE
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_DeleteUser]
    @UserId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Users
        SET 
            IsDeleted = 1,
            IsActive = 0,
            UpdatedBy = @DeletedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @UserId;

        UPDATE UserRoles
        SET IsActive = 0
        WHERE UserId = @UserId;

        SELECT 1 AS Success, 'User deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DisburseLoan]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN DISBURSEMENT
-- =============================================

-- 12. Disburse Loan
-- =============================================
CREATE   PROCEDURE [dbo].[sp_DisburseLoan]
(
    @LoanId INT,
    @DisbursementDate DATE,
    @DisbursementMode NVARCHAR(50),
    @ReferenceNo NVARCHAR(100) = NULL,
    @BankAccountNo NVARCHAR(50) = NULL,
    @DisbursedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @LoanAmount DECIMAL(18,2);
        DECLARE @TotalRepayable DECIMAL(18,2);

        -- Get loan amount
        SELECT @LoanAmount = LoanAmount,
               @TotalRepayable = TotalRepayableAmount
        FROM EmployeeLoans 
        WHERE Id = @LoanId AND Status = 'Approved';

        IF @LoanAmount IS NULL
        BEGIN
            RAISERROR('Loan not found or not in approved status', 16, 1);
            RETURN;
        END

        UPDATE EmployeeLoans 
        SET Status = 'Disbursed',
            DisbursementDate = @DisbursementDate,
            DisbursementMode = @DisbursementMode,
            DisbursementReferenceNo = @ReferenceNo,
            DisbursedBy = @DisbursedBy,
            OutstandingAmount = @TotalRepayable,
            OutstandingPrincipal = @LoanAmount,
            OutstandingInterest = @TotalRepayable - @LoanAmount,
            UpdatedBy = @DisbursedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @LoanId AND Status = 'Approved';

        -- Generate EMI schedule
        EXEC sp_GenerateEMISchedule @LoanId = @LoanId;

        COMMIT TRANSACTION;

        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateEMISchedule]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- EMI SCHEDULE
-- =============================================

-- 13. Generate EMI Schedule
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GenerateEMISchedule]
    @LoanId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LoanAmount DECIMAL(18,2);
    DECLARE @EMIAmount DECIMAL(18,2);
    DECLARE @InterestRate DECIMAL(5,2);
    DECLARE @TenureMonths INT;
    DECLARE @DisbursementDate DATE;

    -- Get loan details
    SELECT @LoanAmount = LoanAmount,
           @EMIAmount = EMIAmount,
           @InterestRate = InterestRate,
           @TenureMonths = TenureMonths,
           @DisbursementDate = DisbursementDate
    FROM EmployeeLoans 
    WHERE Id = @LoanId;

    IF @DisbursementDate IS NULL
    BEGIN
        RAISERROR('Loan not disbursed yet', 16, 1);
        RETURN;
    END

    -- Delete existing schedule
    DELETE FROM LoanEMISchedule WHERE LoanId = @LoanId;

    DECLARE @MonthlyInterestRate DECIMAL(18,10) = @InterestRate / 12 / 100;
    DECLARE @OutstandingPrincipal DECIMAL(18,2) = @LoanAmount;
    DECLARE @EMIDate DATE = DATEADD(MONTH, 1, @DisbursementDate);
    DECLARE @EMINumber INT = 1;

    WHILE @EMINumber <= @TenureMonths
    BEGIN
        DECLARE @InterestAmount DECIMAL(18,2) = ROUND(@OutstandingPrincipal * @MonthlyInterestRate, 2);
        DECLARE @PrincipalAmount DECIMAL(18,2);
        DECLARE @ClosingBalance DECIMAL(18,2);

        -- For last EMI, adjust to clear remaining principal
        IF @EMINumber = @TenureMonths
        BEGIN
            SET @PrincipalAmount = @OutstandingPrincipal;
            SET @ClosingBalance = 0;
        END
        ELSE
        BEGIN
            SET @PrincipalAmount = @EMIAmount - @InterestAmount;
            SET @ClosingBalance = @OutstandingPrincipal - @PrincipalAmount;
        END

        INSERT INTO LoanEMISchedule 
        (
            LoanId, EMINumber, EMIDueDate, EMIAmount, PrincipalAmount, InterestAmount,
            OpeningBalance, ClosingBalance, Status, CreatedDate
        )
        VALUES 
        (
            @LoanId, @EMINumber, @EMIDate, @EMIAmount, @PrincipalAmount, @InterestAmount,
            @OutstandingPrincipal, @ClosingBalance, 'Pending', GETDATE()
        );

        SET @OutstandingPrincipal = @ClosingBalance;
        SET @EMIDate = DATEADD(MONTH, 1, @EMIDate);
        SET @EMINumber = @EMINumber + 1;
    END

    -- Update loan with EMI dates
    UPDATE EmployeeLoans 
    SET FirstEMIDate = (SELECT MIN(EMIDueDate) FROM LoanEMISchedule WHERE LoanId = @LoanId),
        LastEMIDate = (SELECT MAX(EMIDueDate) FROM LoanEMISchedule WHERE LoanId = @LoanId),
        Status = 'Active'
    WHERE Id = @LoanId;

    SELECT @TenureMonths AS EMIsGenerated;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateNextAttendanceId]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GenerateNextAttendanceId]
    @NewAttendanceId NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @LastNumber INT;
    
    SELECT @LastNumber = ISNULL(
        MAX(CAST(REPLACE(AttendanceId, 'ATT_', '') AS INT)), 
        0
    )
    FROM Attendance;
    
    SET @LastNumber = @LastNumber + 1;
    SET @NewAttendanceId = 'ATT_' + RIGHT('0000' + CAST(@LastNumber AS VARCHAR(4)), 4);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateNextStudentId]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GenerateNextStudentId]
    @NewStudentId NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @LastNumber INT;
    
    -- Get the last student number
    SELECT @LastNumber = ISNULL(
        MAX(CAST(REPLACE(StudentId, 'CSM_', '') AS INT)), 
        0
    )
    FROM Students;
    
    -- Increment
    SET @LastNumber = @LastNumber + 1;
    
    -- Format as CSM_0001
    SET @NewStudentId = 'CSM_' + RIGHT('0000' + CAST(@LastNumber AS VARCHAR(4)), 4);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateSalarySlip]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_GenerateSalarySlip
-- Description: Employee साठी salary slip generate करणे
-- Parameters: 
--   @PayrollProcessingId - Payroll processing record ID
--   @GeneratedBy - User ID
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GenerateSalarySlip]
    @PayrollProcessingId INT,
    @GeneratedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @SlipId INT;
        DECLARE @SlipNumber NVARCHAR(50);
        DECLARE @EmployeeId INT;
        DECLARE @PayrollCycleId INT;
        DECLARE @Month INT, @Year INT;
        DECLARE @StartDate DATE, @EndDate DATE;

        -- Get payroll processing details
        SELECT 
            @EmployeeId = pp.EmployeeId,
            @PayrollCycleId = pp.PayrollCycleId,
            @Month = pc.Month,
            @Year = pc.Year,
            @StartDate = pc.StartDate,
            @EndDate = pc.EndDate
        FROM PayrollProcessing pp
        INNER JOIN PayrollCycle pc ON pp.PayrollCycleId = pc.Id
        WHERE pp.Id = @PayrollProcessingId;

        -- Generate unique slip number
        SET @SlipNumber = 'SLIP-' + CAST(@Year AS NVARCHAR) + '-' + 
                         RIGHT('0' + CAST(@Month AS NVARCHAR), 2) + '-' +
                         RIGHT('0000' + CAST(@EmployeeId AS NVARCHAR), 4);

        -- Check if slip already exists
        IF EXISTS (SELECT 1 FROM SalarySlips WHERE SlipNumber = @SlipNumber)
        BEGIN
            -- Update existing slip
            UPDATE SalarySlips
            SET Status = 'Generated',
                GeneratedDate = GETDATE(),
                GeneratedBy = @GeneratedBy,
                UpdatedDate = GETDATE()
            WHERE SlipNumber = @SlipNumber;

            SELECT @SlipId = Id FROM SalarySlips WHERE SlipNumber = @SlipNumber;
        END
        ELSE
        BEGIN
            -- Create new salary slip
            INSERT INTO SalarySlips 
            (SlipNumber, PayrollProcessingId, PayrollCycleId, EmployeeId,
             Month, Year, PayPeriodStart, PayPeriodEnd,
             BasicSalary, GrossSalary, TotalEarnings, TotalDeductions, NetSalary,
             TotalWorkingDays, PresentDays, PaidLeaveDays, LOPDays,
             SSRSReportPath, SSRSReportParameters,
             Status, GeneratedBy, GeneratedDate, CreatedBy, CreatedDate)
            SELECT 
                @SlipNumber,
                pp.Id,
                pp.PayrollCycleId,
                pp.EmployeeId,
                @Month,
                @Year,
                @StartDate,
                @EndDate,
                pp.BasicSalary,
                pp.GrossSalary,
                pp.TotalEarnings,
                pp.TotalDeductions,
                pp.NetSalary,
                pp.TotalWorkingDays,
                pp.PresentDays,
                pp.PaidLeaveDays,
                pp.LOPDays,
                '/PayrollReports/SalarySlip',
                '{"SlipId": ' + CAST(pp.Id AS NVARCHAR) + ', "EmployeeId": ' + CAST(pp.EmployeeId AS NVARCHAR) + '}',
                'Generated',
                @GeneratedBy,
                GETDATE(),
                @GeneratedBy,
                GETDATE()
            FROM PayrollProcessing pp
            WHERE pp.Id = @PayrollProcessingId;

            SET @SlipId = SCOPE_IDENTITY();
        END

        COMMIT TRANSACTION;

        -- Return slip details
        SELECT 
            @SlipId AS SlipId,
            @SlipNumber AS SlipNumber,
            @EmployeeId AS EmployeeId,
            @Month AS Month,
            @Year AS Year;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateSalarySlips]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GenerateSalarySlips]
    @CycleId INT,
    @GeneratedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO SalarySlips (
            SlipNumber, PayrollProcessingId, PayrollCycleId, EmployeeId, 
            [Month], [Year], BasicSalary, GrossSalary, NetSalary, 
            TotalWorkingDays, PresentDays, LOPDays, Status, 
            CreatedDate, GeneratedBy
        )
        SELECT 
            'SLIP-' + CAST(@CycleId AS VARCHAR) + '-' + CAST(pp.Id AS VARCHAR),
            pp.Id,
            pc.Id,
            pp.EmployeeId,
            pc.[Month],
            pc.[Year],
            pp.BasicSalary,
            pp.GrossSalary,
            pp.NetSalary,
            pp.TotalWorkingDays,
            pp.PresentDays,
            pp.LOPDays,
            'Generated',
            GETDATE(),
            @GeneratedBy
        FROM PayrollProcessing pp
        JOIN PayrollCycle pc ON pp.PayrollCycleId = pc.Id
        WHERE pp.PayrollCycleId = @CycleId 
          AND pp.Status IN ('Generated', 'Approved')
          AND NOT EXISTS (SELECT 1 FROM SalarySlips ss WHERE ss.PayrollProcessingId = pp.Id);

        SELECT 1 AS Result; -- Success
    END TRY
    BEGIN CATCH
        SELECT 0 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateTicketNumber]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 1: Generate Ticket Number
-- =============================================
CREATE PROCEDURE [dbo].[sp_GenerateTicketNumber]
    @TicketNumber NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Year INT = YEAR(GETDATE());
    DECLARE @LastNumber INT;
    
    SELECT @LastNumber = ISNULL(MAX(CAST(RIGHT(TicketNumber, 4) AS INT)), 0)
    FROM Tickets
    WHERE TicketNumber LIKE 'TKT-' + CAST(@Year AS NVARCHAR) + '-%';
    
    SET @LastNumber = @LastNumber + 1;
    SET @TicketNumber = 'TKT-' + CAST(@Year AS NVARCHAR) + '-' + RIGHT('0000' + CAST(@LastNumber AS NVARCHAR), 4);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetActiveEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 12. GET ACTIVE EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetActiveEmployees]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsActive = 1 AND e.IsDeleted = 0
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetActiveRoles]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   12. GET ACTIVE ROLES FOR DROPDOWN
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_GetActiveRoles]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id AS RoleId,
        RoleName,
        ISNULL(RoleDescription, Description) AS RoleDescription
    FROM Roles
    WHERE IsActive = 1
    ORDER BY RoleName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAdminLeaveDashboard]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- ✅ SP: Admin Leave Dashboard Stats
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAdminLeaveDashboard]
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    -- ===== Result Set 1: Overall Stats =====
    SELECT 
        (SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND IsDeleted = 0) AS TotalEmployees,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0) AS TotalRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Pending') AS PendingRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Approved') AS ApprovedRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Rejected') AS RejectedRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Cancelled') AS CancelledRequests,
        (SELECT ISNULL(SUM(TotalDays), 0) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Approved') AS TotalApprovedDays,
        (SELECT COUNT(DISTINCT EmployeeId) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Approved') AS EmployeesOnLeave,
        (SELECT COUNT(*) FROM LeaveRequests WHERE CAST(StartDate AS DATE) = CAST(GETDATE() AS DATE) AND IsDeleted = 0 AND Status = 'Approved') AS OnLeaveToday,
        (SELECT COUNT(*) FROM Holidays WHERE Year = @Year AND IsActive = 1 AND IsDeleted = 0) AS TotalHolidays,
        (SELECT COUNT(*) FROM Holidays WHERE Year = @Year AND IsActive = 1 AND IsDeleted = 0 AND Date >= CAST(GETDATE() AS DATE)) AS UpcomingHolidays;

    -- ===== Result Set 2: Monthly Breakdown =====
    SELECT 
        MONTH(lr.StartDate) AS MonthNumber,
        DATENAME(MONTH, lr.StartDate) AS MonthName,
        COUNT(*) AS TotalRequests,
        SUM(CASE WHEN lr.Status = 'Approved' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN lr.Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,
        SUM(CASE WHEN lr.Status = 'Pending' THEN 1 ELSE 0 END) AS Pending,
        SUM(CASE WHEN lr.Status = 'Cancelled' THEN 1 ELSE 0 END) AS Cancelled,
        SUM(CASE WHEN lr.Status = 'Approved' THEN lr.TotalDays ELSE 0 END) AS ApprovedDays
    FROM LeaveRequests lr
    WHERE YEAR(lr.StartDate) = @Year AND lr.IsDeleted = 0
    GROUP BY MONTH(lr.StartDate), DATENAME(MONTH, lr.StartDate)
    ORDER BY MONTH(lr.StartDate);

    -- ===== Result Set 3: Leave Type Breakdown =====
    SELECT 
        lt.Name AS LeaveTypeName,
        lt.Code AS LeaveTypeCode,
        COUNT(lr.Id) AS TotalRequests,
        SUM(CASE WHEN lr.Status = 'Approved' THEN lr.TotalDays ELSE 0 END) AS ApprovedDays,
        SUM(CASE WHEN lr.Status = 'Pending' THEN lr.TotalDays ELSE 0 END) AS PendingDays
    FROM LeaveTypes lt
    LEFT JOIN LeaveRequests lr ON lt.Id = lr.LeaveTypeId AND YEAR(lr.StartDate) = @Year AND lr.IsDeleted = 0
    WHERE lt.IsActive = 1 AND lt.IsDeleted = 0
    GROUP BY lt.Name, lt.Code
    ORDER BY TotalRequests DESC;

    -- ===== Result Set 4: Department Breakdown =====
    SELECT 
        d.DepartmentName AS DepartmentName,
        COUNT(DISTINCT e.Id) AS TotalEmployees,
        COUNT(lr.Id) AS TotalRequests,
        SUM(CASE WHEN lr.Status = 'Approved' THEN lr.TotalDays ELSE 0 END) AS ApprovedDays,
        SUM(CASE WHEN lr.Status = 'Pending' THEN 1 ELSE 0 END) AS PendingCount
    FROM Departments d
    LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1 AND e.IsDeleted = 0
    LEFT JOIN LeaveRequests lr ON e.Id = lr.EmployeeId AND YEAR(lr.StartDate) = @Year AND lr.IsDeleted = 0
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentName
    ORDER BY TotalRequests DESC;

    -- ===== Result Set 5: Recent Leave Requests =====
    SELECT TOP 10
        lr.Id,
        lr.EmployeeId,
        e.Name AS EmployeeName,
        d.DepartmentName AS DepartmentName,
        lt.Name AS LeaveTypeName,
        lt.Code AS LeaveTypeCode,
        lr.StartDate,
        lr.EndDate,
        lr.TotalDays,
        lr.Status,
        lr.AppliedDate,
        lr.Reason
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.IsDeleted = 0 AND YEAR(lr.StartDate) = @Year
    ORDER BY lr.AppliedDate DESC;

    -- ===== Result Set 6: Employees On Leave Today =====
    SELECT 
        e.Name AS EmployeeName,
        d.DepartmentName AS DepartmentName,
        lt.Name AS LeaveTypeName,
        lr.StartDate,
        lr.EndDate,
        lr.TotalDays
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.IsDeleted = 0
      AND lr.Status = 'Approved'
      AND CAST(GETDATE() AS DATE) BETWEEN lr.StartDate AND lr.EndDate
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllEmployeeBalances]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- ✅ SP: Get ALL employees balances
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllEmployeeBalances]
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT lb.Id, lb.EmployeeId, lb.LeaveTypeId, lb.Year,
           lb.TotalAllocated, lb.TotalUsed, lb.TotalPending,
           lb.CarryForward,
           (lb.TotalAllocated + lb.CarryForward - lb.TotalUsed - lb.TotalPending) AS TotalAvailable,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode,
           lt.IsPaid,
           e.Name AS EmployeeName
    FROM LeaveBalances lb
    INNER JOIN LeaveTypes lt ON lb.LeaveTypeId = lt.Id
    INNER JOIN Employees e ON lb.EmployeeId = e.Id
    WHERE lb.Year = @Year 
      AND lb.IsActive = 1
      AND e.IsActive = 1
      AND e.IsDeleted = 0
    ORDER BY e.Name, lt.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- EMPLOYEE MANAGEMENT STORED PROCEDURES
-- =============================================

 
-- =============================================
-- 1. GET ALL EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllEmployees]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLeaveRequests]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetAllLeaveRequests]
    @Status         NVARCHAR(20) = NULL,
    @DepartmentId   INT = NULL,
    @LeaveTypeId    INT = NULL,
    @StartDate      DATE = NULL,
    @EndDate        DATE = NULL,
    @PageNumber     INT = 1,
    @PageSize       INT = 10,
    @TotalRecords   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TotalRecords = COUNT(*)
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    WHERE lr.IsDeleted = 0
      AND (@Status IS NULL OR lr.Status = @Status)
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@LeaveTypeId IS NULL OR lr.LeaveTypeId = @LeaveTypeId)
      AND (@StartDate IS NULL OR lr.StartDate >= @StartDate)
      AND (@EndDate IS NULL OR lr.EndDate <= @EndDate);

    SELECT lr.Id, lr.EmployeeId, lr.LeaveTypeId, lr.StartDate, lr.EndDate,
           lr.TotalDays, lr.Reason, lr.Status, lr.IsHalfDay, lr.HalfDayType,
           lr.AttachmentPath, lr.EmergencyContact, lr.Remarks,
           lr.AppliedDate, lr.ApprovedBy, lr.ApprovedDate,
           lr.RejectedBy, lr.RejectedDate, lr.CancelledDate, lr.CancelReason,
           lr.IsActive, lr.IsDeleted, lr.CreatedBy, lr.CreatedDate,
           lr.UpdatedBy, lr.UpdatedDate,
           e.Name AS EmployeeName,
           e.Email AS EmployeeEmail,
           e.DepartmentId,
           d.DepartmentName AS DepartmentName,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.IsDeleted = 0
      AND (@Status IS NULL OR lr.Status = @Status)
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@LeaveTypeId IS NULL OR lr.LeaveTypeId = @LeaveTypeId)
      AND (@StartDate IS NULL OR lr.StartDate >= @StartDate)
      AND (@EndDate IS NULL OR lr.EndDate <= @EndDate)
    ORDER BY lr.AppliedDate DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLeaveTypes]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetAllLeaveTypes]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, Description, DefaultDays, MaxDays,
           IsCarryForward, MaxCarryForward, IsPaid, IsActive,
           IsDeleted, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    FROM LeaveTypes
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLoans]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 9. Get All Loans (Admin View)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllLoans]
(
    @Status NVARCHAR(50) = NULL,
    @LoanTypeId INT = NULL,
    @DepartmentId INT = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        el.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        lt.LoanTypeName,
        d.DepartmentName,
        ua.FullName AS ApprovedByName
    FROM EmployeeLoans el
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    LEFT JOIN Users ua ON el.ApprovedBy = ua.Id
    WHERE el.IsDeleted = 0
      AND (@Status IS NULL OR el.Status = @Status)
      AND (@LoanTypeId IS NULL OR el.LoanTypeId = @LoanTypeId)
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@FromDate IS NULL OR el.ApplicationDate >= @FromDate)
      AND (@ToDate IS NULL OR el.ApplicationDate <= @ToDate)
    ORDER BY el.ApplicationDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLoanTypes]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN TYPES
-- =============================================

-- 1. Get All Loan Types
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllLoanTypes]
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM LoanTypes 
    WHERE IsDeleted = 0
      AND (@ActiveOnly = 0 OR IsActive = 1)
    ORDER BY DisplayOrder, LoanTypeName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllRolesWithCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetAllRolesWithCount]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.Id AS RoleId,
        r.RoleName,
        ISNULL(r.RoleDescription, r.Description) AS RoleDescription,
        r.IsActive,
        r.CreatedDate,
        COUNT(ur.UserId) AS UserCount
    FROM dbo.Roles r
    LEFT JOIN dbo.UserRoles ur 
        ON r.Id = ur.RoleId
    GROUP BY
        r.Id,
        r.RoleName,
        r.RoleDescription,
        r.Description,
        r.IsActive,
        r.CreatedDate
    ORDER BY r.CreatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllSalaryComponents]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SALARY COMPONENTS
-- =============================================

-- 1. Get All Salary Components
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllSalaryComponents]
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM SalaryComponents 
    WHERE IsDeleted = 0
      AND (@ActiveOnly = 0 OR IsActive = 1)
    ORDER BY ComponentType, DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllSalaryTemplates]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SALARY TEMPLATES
-- =============================================

-- 7. Get All Templates
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAllSalaryTemplates]
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        st.*,
        d.DepartmentName,
        (SELECT COUNT(*) FROM EmployeeSalaryStructure WHERE TemplateId = st.Id AND IsCurrentStructure = 1) AS EmployeeCount
    FROM SalaryTemplates st
    LEFT JOIN Departments d ON st.DepartmentId = d.Id
    WHERE st.IsDeleted = 0
      AND (@ActiveOnly = 0 OR st.IsActive = 1)
    ORDER BY st.TemplateName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllStudents]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        StudentId,
        FirstName,
        LastName,
        FullName,
        Class,
        Subjects,
        Age,
        DateOfBirth,
        JoiningDate,
        BatchTime,
        PassportPhotoPath,
        PhoneNumber,
        Email,
        Address,
        ParentName,
        ParentPhone,
        ParentEmail,
        IsActive,
        IsDeleted,
        CreatedBy,
        CreatedDate,
        UpdatedBy,
        UpdatedDate
    FROM Students
    WHERE IsDeleted = 0
    ORDER BY FullName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllUsers]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   1. GET ALL USERS WITH ROLES
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_GetAllUsers]
    @SearchTerm NVARCHAR(200) = NULL,
    @RoleFilter NVARCHAR(100) = NULL,
    @StatusFilter NVARCHAR(20) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT COUNT(DISTINCT u.Id) AS TotalRecords
    FROM Users u
    LEFT JOIN UserRoles ur 
        ON u.Id = ur.UserId 
        AND ur.IsActive = 1
    LEFT JOIN Roles r 
        ON ur.RoleId = r.Id 
        AND r.IsActive = 1
    WHERE ISNULL(u.IsDeleted, 0) = 0
        AND (
            @SearchTerm IS NULL OR
            u.FullName LIKE '%' + @SearchTerm + '%' OR
            u.Email LIKE '%' + @SearchTerm + '%' OR
            u.Username LIKE '%' + @SearchTerm + '%'
        )
        AND (
            @RoleFilter IS NULL OR
            r.RoleName = @RoleFilter
        )
        AND (
            @StatusFilter IS NULL OR
            (@StatusFilter = 'Active' AND u.IsActive = 1) OR
            (@StatusFilter = 'Inactive' AND u.IsActive = 0)
        );

    SELECT
        u.Id AS UserId,
        u.Username,
        u.FullName,
        u.Email,
        u.PhoneNumber,
        u.ProfilePicture,
        u.IsActive,
        u.IsEmailVerified,
        u.CreatedDate,
        u.LastLoginDate,
        STRING_AGG(r.RoleName, ', ') AS Roles,
        STRING_AGG(CAST(r.Id AS VARCHAR(20)), ',') AS RoleIds
    FROM Users u
    LEFT JOIN UserRoles ur 
        ON u.Id = ur.UserId 
        AND ur.IsActive = 1
    LEFT JOIN Roles r 
        ON ur.RoleId = r.Id 
        AND r.IsActive = 1
    WHERE ISNULL(u.IsDeleted, 0) = 0
        AND (
            @SearchTerm IS NULL OR
            u.FullName LIKE '%' + @SearchTerm + '%' OR
            u.Email LIKE '%' + @SearchTerm + '%' OR
            u.Username LIKE '%' + @SearchTerm + '%'
        )
        AND (
            @StatusFilter IS NULL OR
            (@StatusFilter = 'Active' AND u.IsActive = 1) OR
            (@StatusFilter = 'Inactive' AND u.IsActive = 0)
        )
    GROUP BY
        u.Id,
        u.Username,
        u.FullName,
        u.Email,
        u.PhoneNumber,
        u.ProfilePicture,
        u.IsActive,
        u.IsEmailVerified,
        u.CreatedDate,
        u.LastLoginDate
    HAVING 
        @RoleFilter IS NULL 
        OR STRING_AGG(r.RoleName, ', ') LIKE '%' + @RoleFilter + '%'
    ORDER BY u.CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllUsersWithDetails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetAllUsersWithDetails]
    @Search NVARCHAR(200) = NULL,
    @Role NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.Username,
        u.FullName,
        u.Email,
        u.PhoneNumber,
        u.IsActive,
        u.CreatedDate,
        u.LastLoginDate,

        ISNULL(
            STUFF((
                SELECT ', ' + r.RoleName
                FROM UserRoles ur
                INNER JOIN Roles r ON r.Id = ur.RoleId
                WHERE ur.UserId = u.Id
                  AND ISNULL(ur.IsActive, 1) = 1
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
            ''
        ) AS Roles,

        e.EmployeeCode,
        e.DepartmentId,
        d.DepartmentName,
        e.Designation

    FROM Users u
    LEFT JOIN Employees e
        ON e.Email = u.Email
       AND ISNULL(e.IsDeleted, 0) = 0
    LEFT JOIN Departments d
        ON d.Id = e.DepartmentId
       AND ISNULL(d.IsActive, 1) = 1
    WHERE ISNULL(u.IsDeleted, 0) = 0
      AND (
            @Search IS NULL OR @Search = ''
            OR u.FullName LIKE '%' + @Search + '%'
            OR u.Username LIKE '%' + @Search + '%'
            OR u.Email LIKE '%' + @Search + '%'
          )
      AND (
            @Status IS NULL OR @Status = ''
            OR (@Status = 'Active' AND u.IsActive = 1)
            OR (@Status = 'Inactive' AND u.IsActive = 0)
          )
      AND (
            @Role IS NULL OR @Role = ''
            OR EXISTS (
                SELECT 1
                FROM UserRoles ur2
                INNER JOIN Roles r2 ON r2.Id = ur2.RoleId
                WHERE ur2.UserId = u.Id
                  AND r2.RoleName = @Role
                  AND ISNULL(ur2.IsActive, 1) = 1
            )
          )
    ORDER BY u.CreatedDate DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllUsersWithRoles]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_GetAllUsersWithRoles]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Username,
        u.Email,
        ISNULL(u.FullName, CONCAT(u.FirstName, ' ', u.LastName)) AS FullName,
        ISNULL(u.PhoneNumber, '') AS PhoneNumber,
        u.IsActive,
        ISNULL(u.RegistrationStatus, CASE WHEN u.IsActive = 1 THEN 'Approved' ELSE 'Pending' END) AS RegistrationStatus,
        ISNULL(u.CreatedDate, GETDATE()) AS CreatedDate,
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.DepartmentId,
        d.DepartmentName AS DepartmentName,
        (
            SELECT STRING_AGG(r.RoleName, ', ')
            FROM dbo.UserRoles ur
            INNER JOIN dbo.Roles r ON ur.RoleId = r.Id
            WHERE ur.UserId = u.Id AND ISNULL(ur.IsActive, 1) = 1
        ) AS AssignedRoles
    FROM dbo.Users u
    LEFT JOIN dbo.Employees e ON e.Email = u.Email
    LEFT JOIN dbo.Departments d ON e.DepartmentId = d.Id
    WHERE ISNULL(u.IsDeleted, 0) = 0
    ORDER BY
        CASE
            WHEN u.RegistrationStatus = 'Pending' OR u.IsActive = 0 THEN 1
            ELSE 2
        END,
        u.CreatedDate DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAttendanceByDate]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAttendanceByDate]
    @AttendanceDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.Id,
        a.AttendanceId,
        a.StudentId,
        s.StudentId AS StudentCode,
        s.FullName,
        s.Class,
        s.BatchTime,
        s.PassportPhotoPath,
        a.AttendanceDate,
        a.AttendanceTime,
        a.Status,
        a.CapturedImagePath,
        a.ConfidenceScore,
        a.Remarks,
        a.CreatedDate
    FROM Attendance a
    INNER JOIN Students s ON a.StudentId = s.Id
    WHERE a.AttendanceDate = @AttendanceDate
    ORDER BY a.AttendanceTime;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAttendanceReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- ✅ REPORT 2: ATTENDANCE REPORT
-- (Based on Leave data - shows present/absent)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetAttendanceReport]
    @Month          INT,
    @Year           INT,
    @DepartmentId   INT = NULL,
    @EmployeeId     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartDate DATE = DATEFROMPARTS(@Year, @Month, 1);
    DECLARE @EndDate DATE = EOMONTH(@StartDate);
    DECLARE @TotalWorkingDays INT = 0;

    -- Calculate working days (exclude weekends)
    DECLARE @TempDate DATE = @StartDate;
    WHILE @TempDate <= @EndDate
    BEGIN
        IF DATEPART(WEEKDAY, @TempDate) NOT IN (1, 7) -- Not Sunday(1) or Saturday(7)
        BEGIN
            SET @TotalWorkingDays = @TotalWorkingDays + 1;
        END
        SET @TempDate = DATEADD(DAY, 1, @TempDate);
    END

    -- Subtract holidays
    DECLARE @HolidayCount INT;
    SELECT @HolidayCount = COUNT(*)
    FROM Holidays
    WHERE Date BETWEEN @StartDate AND @EndDate
      AND IsActive = 1 AND IsDeleted = 0
      AND DATEPART(WEEKDAY, Date) NOT IN (1, 7);

    SET @TotalWorkingDays = @TotalWorkingDays - @HolidayCount;

    -- Result Set 1: Summary
    SELECT 
        @TotalWorkingDays AS TotalWorkingDays,
        @HolidayCount AS HolidaysInMonth,
        @Month AS ReportMonth,
        @Year AS ReportYear,
        DATENAME(MONTH, @StartDate) AS MonthName;

    -- Result Set 2: Employee Attendance Summary
    SELECT 
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.Email,
        d.DepartmentName AS DepartmentName,
        e.Role,
        @TotalWorkingDays AS TotalWorkingDays,
        ISNULL(
            (SELECT SUM(
                CASE 
                    WHEN lr.StartDate >= @StartDate AND lr.EndDate <= @EndDate THEN lr.TotalDays
                    WHEN lr.StartDate < @StartDate AND lr.EndDate <= @EndDate THEN DATEDIFF(DAY, @StartDate, lr.EndDate) + 1
                    WHEN lr.StartDate >= @StartDate AND lr.EndDate > @EndDate THEN DATEDIFF(DAY, lr.StartDate, @EndDate) + 1
                    ELSE DATEDIFF(DAY, @StartDate, @EndDate) + 1
                END)
             FROM LeaveRequests lr 
             WHERE lr.EmployeeId = e.Id 
               AND lr.Status = 'Approved'
               AND lr.IsDeleted = 0
               AND lr.StartDate <= @EndDate 
               AND lr.EndDate >= @StartDate
            ), 0
        ) AS LeaveDays,
        @TotalWorkingDays - ISNULL(
            (SELECT SUM(
                CASE 
                    WHEN lr2.StartDate >= @StartDate AND lr2.EndDate <= @EndDate THEN lr2.TotalDays
                    WHEN lr2.StartDate < @StartDate AND lr2.EndDate <= @EndDate THEN DATEDIFF(DAY, @StartDate, lr2.EndDate) + 1
                    WHEN lr2.StartDate >= @StartDate AND lr2.EndDate > @EndDate THEN DATEDIFF(DAY, lr2.StartDate, @EndDate) + 1
                    ELSE DATEDIFF(DAY, @StartDate, @EndDate) + 1
                END)
             FROM LeaveRequests lr2 
             WHERE lr2.EmployeeId = e.Id 
               AND lr2.Status = 'Approved'
               AND lr2.IsDeleted = 0
               AND lr2.StartDate <= @EndDate 
               AND lr2.EndDate >= @StartDate
            ), 0
        ) AS PresentDays,
        -- Leave type breakdown
        ISNULL((SELECT SUM(lr3.TotalDays) FROM LeaveRequests lr3 INNER JOIN LeaveTypes lt ON lr3.LeaveTypeId = lt.Id WHERE lr3.EmployeeId = e.Id AND lr3.Status = 'Approved' AND lr3.IsDeleted = 0 AND lt.Code = 'CL' AND lr3.StartDate <= @EndDate AND lr3.EndDate >= @StartDate), 0) AS CasualLeave,
        ISNULL((SELECT SUM(lr4.TotalDays) FROM LeaveRequests lr4 INNER JOIN LeaveTypes lt2 ON lr4.LeaveTypeId = lt2.Id WHERE lr4.EmployeeId = e.Id AND lr4.Status = 'Approved' AND lr4.IsDeleted = 0 AND lt2.Code = 'SL' AND lr4.StartDate <= @EndDate AND lr4.EndDate >= @StartDate), 0) AS SickLeave,
        ISNULL((SELECT SUM(lr5.TotalDays) FROM LeaveRequests lr5 INNER JOIN LeaveTypes lt3 ON lr5.LeaveTypeId = lt3.Id WHERE lr5.EmployeeId = e.Id AND lr5.Status = 'Approved' AND lr5.IsDeleted = 0 AND lt3.Code = 'EL' AND lr5.StartDate <= @EndDate AND lr5.EndDate >= @StartDate), 0) AS EarnedLeave,
        ISNULL((SELECT SUM(lr6.TotalDays) FROM LeaveRequests lr6 INNER JOIN LeaveTypes lt4 ON lr6.LeaveTypeId = lt4.Id WHERE lr6.EmployeeId = e.Id AND lr6.Status = 'Approved' AND lr6.IsDeleted = 0 AND lt4.Code = 'LWP' AND lr6.StartDate <= @EndDate AND lr6.EndDate >= @StartDate), 0) AS LWP,
        -- Attendance percentage
        CASE WHEN @TotalWorkingDays > 0 
            THEN CAST(
                (@TotalWorkingDays - ISNULL(
                    (SELECT SUM(lr7.TotalDays) FROM LeaveRequests lr7 WHERE lr7.EmployeeId = e.Id AND lr7.Status = 'Approved' AND lr7.IsDeleted = 0 AND lr7.StartDate <= @EndDate AND lr7.EndDate >= @StartDate)
                , 0)) * 100.0 / @TotalWorkingDays AS DECIMAL(5,1))
            ELSE 100.0
        END AS AttendancePercentage
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsActive = 1 AND e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@EmployeeId IS NULL OR e.Id = @EmployeeId)
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAttendanceSummary]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAttendanceSummary]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Class NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FromDate IS NULL
        SET @FromDate = DATEADD(MONTH, -1, GETDATE());
    IF @ToDate IS NULL
        SET @ToDate = GETDATE();
    
    SELECT 
        s.Id AS StudentId,
        s.StudentId AS StudentCode,
        s.FullName,
        s.Class,
        s.BatchTime,
        COUNT(a.Id) AS TotalDays,
        COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS TotalPresent,
        COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS TotalAbsent,
        COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS TotalLate,
        CAST(
            CASE 
                WHEN COUNT(a.Id) > 0 
                THEN (COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) * 100.0) / COUNT(a.Id)
                ELSE 0 
            END AS DECIMAL(5,2)
        ) AS AttendancePercentage
    FROM Students s
    LEFT JOIN Attendance a ON s.Id = a.StudentId
        AND a.AttendanceDate BETWEEN @FromDate AND @ToDate
    WHERE s.IsDeleted = 0
    AND (@Class IS NULL OR s.Class = @Class)
    GROUP BY s.Id, s.StudentId, s.FullName, s.Class, s.BatchTime
    ORDER BY s.FullName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAuditLogs]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAuditLogs]  
    @PageNumber INT = 1,  
    @PageSize INT = 50,  
    @UserId INT = NULL,  
    @Action NVARCHAR(100) = NULL,  
    @EntityName NVARCHAR(100) = NULL,  
    @StartDate DATETIME2 = NULL,  
    @EndDate DATETIME2 = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT   
        Id,          -- ✅ Make sure Id is first!
        UserId, 
        Username, 
        Action, 
        EntityName, 
        EntityId,  
        OldValues, 
        NewValues, 
        IpAddress, 
        UserAgent, 
        Timestamp  
    FROM AuditLogs  
    ORDER BY Timestamp DESC  
    OFFSET @Offset ROWS  
    FETCH NEXT @PageSize ROWS ONLY;  
  
    SELECT COUNT(*) AS TotalRecords FROM AuditLogs;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDashboardStatistics]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetDashboardStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalStudents INT;
    DECLARE @ActiveStudents INT;
    DECLARE @TodayPresent INT;
    DECLARE @TodayAbsent INT;
    DECLARE @TodayAttendancePercentage DECIMAL(5,2);
    DECLARE @MonthlyAvgAttendance DECIMAL(5,2);
    
    -- Total students
    SELECT @TotalStudents = COUNT(*) FROM Students WHERE IsDeleted = 0;
    SELECT @ActiveStudents = COUNT(*) FROM Students WHERE IsDeleted = 0 AND IsActive = 1;
    
    -- Today's attendance
    SELECT 
        @TodayPresent = COUNT(CASE WHEN Status = 'Present' THEN 1 END),
        @TodayAbsent = COUNT(CASE WHEN Status = 'Absent' THEN 1 END)
    FROM Attendance
    WHERE AttendanceDate = CAST(GETDATE() AS DATE);
    
    -- Today's percentage
    IF (@TodayPresent + @TodayAbsent) > 0
        SET @TodayAttendancePercentage = (@TodayPresent * 100.0) / (@TodayPresent + @TodayAbsent);
    ELSE
        SET @TodayAttendancePercentage = 0;
    
    -- Monthly average
    SELECT @MonthlyAvgAttendance = AVG(AttendancePercentage)
    FROM vw_AttendanceSummary;
    
    -- Return results
    SELECT 
        @TotalStudents AS TotalStudents,
        @ActiveStudents AS ActiveStudents,
        @TodayPresent AS TodayPresent,
        @TodayAbsent AS TodayAbsent,
        @TodayAttendancePercentage AS TodayAttendancePercentage,
        ISNULL(@MonthlyAvgAttendance, 0) AS MonthlyAvgAttendance;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDeductionsBreakdown]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 12. Deductions Breakdown
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetDeductionsBreakdown]
    @ProcessingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ComponentCode,
        ComponentName,
        Amount,
        AdjustedForLOP,
        OriginalAmount,
        DisplayOrder
    FROM PayrollProcessingDetails
    WHERE PayrollProcessingId = @ProcessingId
      AND ComponentType = 'Deduction'
    ORDER BY DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDeletedEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 14. GET DELETED EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetDeletedEmployees]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 1
    ORDER BY e.DeletedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDepartmentLeaveReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetDepartmentLeaveReport]
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT 
        d.Id AS DepartmentId,
        d.DepartmentName AS DepartmentName,
        COUNT(DISTINCT e.Id) AS TotalEmployees,
        ISNULL(SUM(lb.TotalAllocated), 0) AS TotalAllocated,
        ISNULL(SUM(lb.TotalUsed), 0) AS TotalUsed,
        ISNULL(SUM(lb.TotalPending), 0) AS TotalPending,
        ISNULL(SUM(lb.TotalAllocated + lb.CarryForward - lb.TotalUsed - lb.TotalPending), 0) AS TotalAvailable,
        (SELECT COUNT(*) 
         FROM LeaveRequests lr 
         INNER JOIN Employees e2 ON lr.EmployeeId = e2.Id
         WHERE e2.DepartmentId = d.Id 
           AND YEAR(lr.StartDate) = @Year 
           AND lr.IsDeleted = 0 
           AND lr.Status = 'Approved') AS ApprovedRequests,
        (SELECT COUNT(*) 
         FROM LeaveRequests lr2 
         INNER JOIN Employees e3 ON lr2.EmployeeId = e3.Id
         WHERE e3.DepartmentId = d.Id 
           AND YEAR(lr2.StartDate) = @Year 
           AND lr2.IsDeleted = 0 
           AND lr2.Status = 'Pending') AS PendingRequests,
        (SELECT COUNT(*) 
         FROM LeaveRequests lr3 
         INNER JOIN Employees e4 ON lr3.EmployeeId = e4.Id
         WHERE e4.DepartmentId = d.Id 
           AND YEAR(lr3.StartDate) = @Year 
           AND lr3.IsDeleted = 0 
           AND lr3.Status = 'Rejected') AS RejectedRequests
    FROM Departments d
    LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1 AND e.IsDeleted = 0
    LEFT JOIN LeaveBalances lb ON e.Id = lb.EmployeeId AND lb.Year = @Year
    WHERE d.IsActive = 1
    GROUP BY d.Id, d.DepartmentName
    ORDER BY d.DepartmentName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDepartments]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetDepartments]
AS
BEGIN
    SELECT 
        Id,
        DepartmentName
    FROM Departments
    ORDER BY DepartmentName
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDepartmentSummary]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 21. GET DEPARTMENT SUMMARY
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetDepartmentSummary]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.Id AS DepartmentId,
        d.DepartmentName,
        COUNT(e.Id) AS EmployeeCount,
        ISNULL(SUM(e.Salary), 0) AS TotalSalary,
        ISNULL(AVG(e.Salary), 0) AS AverageSalary
    FROM Departments d
    LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsDeleted = 0 AND e.IsActive = 1
    GROUP BY d.Id, d.DepartmentName
    ORDER BY d.DepartmentName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEarningsBreakdown]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 11. Earnings Breakdown
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEarningsBreakdown]
    @ProcessingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ComponentCode,
        ComponentName,
        Amount,
        AdjustedForLOP,
        OriginalAmount,
        DisplayOrder
    FROM PayrollProcessingDetails
    WHERE PayrollProcessingId = @ProcessingId
      AND ComponentType = 'Earning'
    ORDER BY DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeActiveCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 17. GET EMPLOYEE ACTIVE COUNT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeByEmail]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. GET EMPLOYEE BY EMAIL
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeByEmail]
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.Email = @Email AND e.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 2. GET EMPLOYEE BY ID
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.Id = @Id AND e.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeByIdIncludeDeleted]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 3. GET EMPLOYEE BY ID (INCLUDING DELETED)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeByIdIncludeDeleted]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeCountByDepartment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 20. GET EMPLOYEE COUNT BY DEPARTMENT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeCountByDepartment]
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM Employees 
    WHERE DepartmentId = @DepartmentId AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeCurrentSalary]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- EMPLOYEE SALARY STRUCTURE
-- =============================================

-- 17. Get Employee Current Salary
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeCurrentSalary]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ess.*,
        st.TemplateName,
        st.TemplateCode
    FROM EmployeeSalaryStructure ess
    LEFT JOIN SalaryTemplates st ON ess.TemplateId = st.Id
    WHERE ess.EmployeeId = @EmployeeId 
      AND ess.IsCurrentStructure = 1
      AND ess.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeDashboardStats]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 33. GET DASHBOARD STATISTICS
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Total counts
    SELECT 
        (SELECT COUNT(*) FROM Employees WHERE IsDeleted = 0) AS TotalEmployees,
        (SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND IsDeleted = 0) AS ActiveEmployees,
        (SELECT COUNT(*) FROM Employees WHERE IsActive = 0 AND IsDeleted = 0) AS InactiveEmployees,
        (SELECT COUNT(*) FROM Employees WHERE IsDeleted = 1) AS DeletedEmployees,
        (SELECT COUNT(*) FROM Departments WHERE IsActive = 1) AS TotalDepartments,
        (SELECT SUM(Salary) FROM Employees WHERE IsDeleted = 0 AND IsActive = 1) AS TotalSalary,
        (SELECT AVG(Salary) FROM Employees WHERE IsDeleted = 0 AND IsActive = 1) AS AverageSalary,
        (SELECT COUNT(*) FROM Employees WHERE CreatedDate >= DATEADD(MONTH, -1, GETDATE()) AND IsDeleted = 0) AS NewEmployeesThisMonth;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeDeletedCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 19. GET EMPLOYEE DELETED COUNT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeDeletedCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM Employees WHERE IsDeleted = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeInactiveCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 18. GET EMPLOYEE INACTIVE COUNT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeInactiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM Employees WHERE IsActive = 0 AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeLeaveHistory]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetEmployeeLeaveHistory]
    @EmployeeId INT,
    @Year       INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Year IS NULL
        SET @Year = YEAR(GETDATE());

    SELECT lr.Id, lr.EmployeeId, lr.LeaveTypeId, lr.StartDate, lr.EndDate,
           lr.TotalDays, lr.Reason, lr.Status, lr.IsHalfDay, lr.HalfDayType,
           lr.AttachmentPath, lr.EmergencyContact, lr.Remarks,
           lr.AppliedDate, lr.ApprovedBy, lr.ApprovedDate,
           lr.RejectedBy, lr.RejectedDate, lr.CancelledDate, lr.CancelReason,
           lr.IsActive, lr.IsDeleted, lr.CreatedBy, lr.CreatedDate,
           lr.UpdatedBy, lr.UpdatedDate,
           e.Name AS EmployeeName,
           e.Email AS EmployeeEmail,
           e.DepartmentId,
           d.DepartmentName AS DepartmentName,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.EmployeeId = @EmployeeId
      AND lr.IsDeleted = 0
      AND YEAR(lr.StartDate) = @Year
    ORDER BY lr.AppliedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeLeaveReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- LEAVE REPORT STORED PROCEDURES
-- =============================================

-- ✅ Employee Leave Report (Detailed)
CREATE   PROCEDURE [dbo].[sp_GetEmployeeLeaveReport]
    @Year         INT = NULL,
    @DepartmentId INT = NULL,
    @EmployeeId   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT 
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.Email AS EmployeeEmail,
        d.DepartmentName AS DepartmentName,
        lt.Name AS LeaveTypeName,
        lt.Code AS LeaveTypeCode,
        ISNULL(lb.TotalAllocated, 0) AS TotalAllocated,
        ISNULL(lb.TotalUsed, 0) AS TotalUsed,
        ISNULL(lb.TotalPending, 0) AS TotalPending,
        ISNULL(lb.CarryForward, 0) AS CarryForward,
        ISNULL(lb.TotalAllocated + lb.CarryForward - lb.TotalUsed - lb.TotalPending, 0) AS TotalAvailable,
        (SELECT COUNT(*) FROM LeaveRequests lr2 
         WHERE lr2.EmployeeId = e.Id AND lr2.LeaveTypeId = lt.Id 
         AND YEAR(lr2.StartDate) = @Year AND lr2.IsDeleted = 0 AND lr2.Status = 'Approved') AS ApprovedCount,
        (SELECT COUNT(*) FROM LeaveRequests lr3 
         WHERE lr3.EmployeeId = e.Id AND lr3.LeaveTypeId = lt.Id 
         AND YEAR(lr3.StartDate) = @Year AND lr3.IsDeleted = 0 AND lr3.Status = 'Rejected') AS RejectedCount,
        (SELECT COUNT(*) FROM LeaveRequests lr4 
         WHERE lr4.EmployeeId = e.Id AND lr4.LeaveTypeId = lt.Id 
         AND YEAR(lr4.StartDate) = @Year AND lr4.IsDeleted = 0 AND lr4.Status = 'Pending') AS PendingCount
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.Id
    CROSS JOIN LeaveTypes lt
    LEFT JOIN LeaveBalances lb ON e.Id = lb.EmployeeId AND lt.Id = lb.LeaveTypeId AND lb.Year = @Year
    WHERE e.IsActive = 1 AND e.IsDeleted = 0
      AND lt.IsActive = 1 AND lt.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@EmployeeId IS NULL OR e.Id = @EmployeeId)
    ORDER BY e.Name, lt.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeLoans]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 7. Get Employee Loans
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeLoans]
(
    @EmployeeId INT,
    @Status NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        el.*,
        lt.LoanTypeName,
        lt.LoanTypeCode
    FROM EmployeeLoans el
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    WHERE el.EmployeeId = @EmployeeId 
      AND el.IsDeleted = 0
      AND (@Status IS NULL OR el.Status = @Status)
    ORDER BY el.ApplicationDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeePayroll]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 9. Get Employee Payroll
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeePayroll]
    @CycleId INT,
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM PayrollProcessing
    WHERE PayrollCycleId = @CycleId
      AND EmployeeId = @EmployeeId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- ✅ REPORT 1: EMPLOYEE REPORT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeReport]
    @DepartmentId   INT = NULL,
    @IsActive       BIT = NULL,
    @JoiningFrom    DATE = NULL,
    @JoiningTo      DATE = NULL,
    @SearchTerm     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Summary Stats
    SELECT 
        COUNT(*) AS TotalEmployees,
        SUM(CASE WHEN e.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveEmployees,
        SUM(CASE WHEN e.IsActive = 0 THEN 1 ELSE 0 END) AS InactiveEmployees,
        COUNT(DISTINCT e.DepartmentId) AS TotalDepartments,
        ISNULL(AVG(e.Salary), 0) AS AverageSalary,
        ISNULL(SUM(e.Salary), 0) AS TotalSalary,
        SUM(CASE WHEN e.JoiningDate >= DATEADD(MONTH, -1, GETDATE()) THEN 1 ELSE 0 END) AS NewJoinersThisMonth,
        SUM(CASE WHEN MONTH(e.DateOfBirth) = MONTH(GETDATE()) AND DAY(e.DateOfBirth) >= DAY(GETDATE()) THEN 1 ELSE 0 END) AS UpcomingBirthdays
    FROM Employees e
    WHERE e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@IsActive IS NULL OR e.IsActive = @IsActive);

    -- Result Set 2: Employee Details
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.PhoneNumber,
        d.DepartmentName AS DepartmentName,
        e.Role,
        e.Salary,
        e.DateOfBirth,
        e.JoiningDate,
        e.Address,
        e.IsActive,
        e.ProfileImagePath,
        DATEDIFF(YEAR, e.JoiningDate, GETDATE()) AS YearsOfService,
        DATEDIFF(YEAR, e.DateOfBirth, GETDATE()) AS Age,
        (SELECT COUNT(*) FROM LeaveRequests lr WHERE lr.EmployeeId = e.Id AND lr.Status = 'Approved' AND YEAR(lr.StartDate) = YEAR(GETDATE())) AS LeavesThisYear,
        (SELECT ISNULL(SUM(lr2.TotalDays), 0) FROM LeaveRequests lr2 WHERE lr2.EmployeeId = e.Id AND lr2.Status = 'Approved' AND YEAR(lr2.StartDate) = YEAR(GETDATE())) AS LeaveDaysThisYear
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
      AND (@IsActive IS NULL OR e.IsActive = @IsActive)
      AND (@JoiningFrom IS NULL OR e.JoiningDate >= @JoiningFrom)
      AND (@JoiningTo IS NULL OR e.JoiningDate <= @JoiningTo)
      AND (@SearchTerm IS NULL OR e.Name LIKE '%' + @SearchTerm + '%' OR e.Email LIKE '%' + @SearchTerm + '%')
    ORDER BY e.Name;

    -- Result Set 3: Department Distribution
    SELECT 
        d.DepartmentName AS DepartmentName,
        COUNT(e.Id) AS EmployeeCount,
        ISNULL(AVG(e.Salary), 0) AS AvgSalary,
        ISNULL(SUM(e.Salary), 0) AS TotalSalary,
        SUM(CASE WHEN e.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveCount,
        SUM(CASE WHEN e.IsActive = 0 THEN 1 ELSE 0 END) AS InactiveCount
    FROM Departments d
    LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsDeleted = 0
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentName
    ORDER BY EmployeeCount DESC;

    -- Result Set 4: Role Distribution
    SELECT 
        ISNULL(e.Role, 'Not Assigned') AS RoleName,
        COUNT(*) AS EmployeeCount
    FROM Employees e
    WHERE e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    GROUP BY e.Role
    ORDER BY EmployeeCount DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeSalaryComponents]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 21. Get Employee Salary Components
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeSalaryComponents]
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        esc.*,
        sc.ComponentCode,
        sc.ComponentName,
        sc.IsStatutory,
        sc.IsTaxable,
        sc.Category
    FROM EmployeeSalaryComponents esc
    INNER JOIN SalaryComponents sc ON esc.ComponentId = sc.Id
    WHERE esc.EmployeeSalaryStructureId = @StructureId 
      AND esc.IsActive = 1
      AND esc.IsDeleted = 0
    ORDER BY esc.ComponentType, esc.DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeSalaryHistory]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 18. Get Employee Salary History
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeSalaryHistory]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ess.*,
        st.TemplateName,
        st.TemplateCode,
        u.FullName AS ApprovedByName
    FROM EmployeeSalaryStructure ess
    LEFT JOIN SalaryTemplates st ON ess.TemplateId = st.Id
    LEFT JOIN Users u ON ess.ApprovedBy = u.Id
    WHERE ess.EmployeeId = @EmployeeId
    ORDER BY ess.EffectiveFrom DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeSalaryStatistics]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 28. GET EMPLOYEE SALARY STATISTICS
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeSalaryStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalEmployees,
        MIN(Salary) AS MinSalary,
        MAX(Salary) AS MaxSalary,
        AVG(Salary) AS AverageSalary,
        SUM(Salary) AS TotalSalary
    FROM Employees
    WHERE IsDeleted = 0 AND IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesByDepartment]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 11. GET EMPLOYEES BY DEPARTMENT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeesByDepartment]
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.DepartmentId = @DepartmentId AND e.IsDeleted = 0
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesByJoiningDateRange]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 30. GET EMPLOYEES BY JOINING DATE RANGE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeesByJoiningDateRange]
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0 
    AND e.JoiningDate BETWEEN @StartDate AND @EndDate
    ORDER BY e.JoiningDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesBySalaryRange]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 26. Get Employees By Salary Range
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeesBySalaryRange]
(
    @MinSalary DECIMAL(18,2),
    @MaxSalary DECIMAL(18,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ess.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        d.DepartmentName
    FROM EmployeeSalaryStructure ess
    INNER JOIN Employees e ON ess.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE ess.IsCurrentStructure = 1
      AND ess.IsActive = 1
      AND ess.GrossSalary BETWEEN @MinSalary AND @MaxSalary
    ORDER BY ess.GrossSalary DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesFiltered]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetEmployeesFiltered]
    @Department NVARCHAR(100) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name  ,  -- Change to your actual column name
        e.Email,
        e.DepartmentId,
        d.DepartmentName AS DepartmentName,  -- Change to your actual column name
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0
    AND (@Department IS NULL OR d.DepartmentName = @Department)
    AND (@IsActive IS NULL OR e.IsActive = @IsActive)
    ORDER BY e.Name;  -- Change to your actual column name
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesForExport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 24. GET EMPLOYEES FOR EXPORT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeesForExport]
    @DepartmentId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        d.DepartmentName AS Department,
        e.Role,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        CASE WHEN e.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        e.CreatedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0
    AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    AND (@IsActive IS NULL OR e.IsActive = @IsActive)
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeesPaged]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 15. GET EMPLOYEES PAGED (WITH FILTERING & SORTING)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeesPaged]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @DepartmentId INT = NULL,
    @IsActive BIT = NULL,
    @SortBy NVARCHAR(50) = 'Id',
    @SortOrder NVARCHAR(4) = 'ASC',
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @CountSQL NVARCHAR(MAX);
    DECLARE @WhereClause NVARCHAR(MAX) = 'WHERE e.IsDeleted = 0';
    DECLARE @Params NVARCHAR(500);
    
    -- Build WHERE clause
    IF @SearchTerm IS NOT NULL AND @SearchTerm <> ''
    BEGIN
        SET @WhereClause = @WhereClause + ' AND (e.Name LIKE ''%'' + @SearchTerm + ''%'' 
            OR e.Email LIKE ''%'' + @SearchTerm + ''%''
            OR e.PhoneNumber LIKE ''%'' + @SearchTerm + ''%''
            OR d.DepartmentName LIKE ''%'' + @SearchTerm + ''%'')';
    END
    
    IF @DepartmentId IS NOT NULL
    BEGIN
        SET @WhereClause = @WhereClause + ' AND e.DepartmentId = @DepartmentId';
    END
    
    IF @IsActive IS NOT NULL
    BEGIN
        SET @WhereClause = @WhereClause + ' AND e.IsActive = @IsActive';
    END
    
    -- Validate sort column to prevent SQL injection
    IF @SortBy NOT IN ('Id', 'Name', 'Email', 'DepartmentName', 'Salary', 'JoiningDate', 'CreatedDate', 'IsActive')
    BEGIN
        SET @SortBy = 'Id';
    END
    
    IF @SortOrder NOT IN ('ASC', 'DESC')
    BEGIN
        SET @SortOrder = 'ASC';
    END
    
    -- Get total count
    SET @CountSQL = N'SELECT @TotalRecords = COUNT(*) 
        FROM Employees e
        LEFT JOIN Departments d ON e.DepartmentId = d.Id ' + @WhereClause;
    
    SET @Params = N'@SearchTerm NVARCHAR(100), @DepartmentId INT, @IsActive BIT, @TotalRecords INT OUTPUT';
    
    EXEC sp_executesql @CountSQL, @Params, 
        @SearchTerm = @SearchTerm, 
        @DepartmentId = @DepartmentId, 
        @IsActive = @IsActive,
        @TotalRecords = @TotalRecords OUTPUT;
    
    -- Get paged data with proper ORDER BY based on column
    SET @SQL = N'SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id ' 
    + @WhereClause + N'
    ORDER BY ' + 
        CASE @SortBy 
            WHEN 'DepartmentName' THEN 'd.DepartmentName'
            ELSE 'e.' + @SortBy 
        END + ' ' + @SortOrder + N'
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY';
    
    SET @Params = N'@SearchTerm NVARCHAR(100), @DepartmentId INT, @IsActive BIT, @Offset INT, @PageSize INT';
    
    EXEC sp_executesql @SQL, @Params, 
        @SearchTerm = @SearchTerm, 
        @DepartmentId = @DepartmentId, 
        @IsActive = @IsActive,
        @Offset = @Offset, 
        @PageSize = @PageSize;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeTotalCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 16. GET EMPLOYEE TOTAL COUNT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetEmployeeTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM Employees WHERE IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetHolidayById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetHolidayById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Date, Day, Type, Description, Year, 
           IsActive, IsDeleted, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    FROM Holidays
    WHERE Id = @Id AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetHolidays]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetHolidays]
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT Id, Name, Date, Day, Type, Description, Year, 
           IsActive, IsDeleted, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    FROM Holidays
    WHERE Year = @Year AND IsActive = 1 AND IsDeleted = 0
    ORDER BY Date;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetHolidaysCount]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetHolidaysCount]
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    SELECT 
        COUNT(*) AS TotalHolidays,
        SUM(CASE WHEN Type = 'Public' THEN 1 ELSE 0 END) AS PublicHolidays,
        SUM(CASE WHEN Type = 'Optional' THEN 1 ELSE 0 END) AS OptionalHolidays,
        SUM(CASE WHEN Type = 'Restricted' THEN 1 ELSE 0 END) AS RestrictedHolidays,
        SUM(CASE WHEN Date >= GETDATE() THEN 1 ELSE 0 END) AS UpcomingHolidays,
        SUM(CASE WHEN Date < GETDATE() THEN 1 ELSE 0 END) AS PastHolidays
    FROM Holidays
    WHERE Year = @Year AND IsActive = 1 AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetInactiveEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 13. GET INACTIVE EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetInactiveEmployees]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsActive = 0 AND e.IsDeleted = 0
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeaveBalance]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetLeaveBalance]
    @EmployeeId INT,
    @Year       INT = NULL
AS
BEGIN
    SET NOCOUNT ON;  -- ✅ Important!
    
    IF @Year IS NULL
        SET @Year = YEAR(GETDATE());

    SELECT lb.Id, lb.EmployeeId, lb.LeaveTypeId, lb.Year,
           lb.TotalAllocated, lb.TotalUsed, lb.TotalPending,
           lb.CarryForward,
           (lb.TotalAllocated + lb.CarryForward - lb.TotalUsed - lb.TotalPending) AS TotalAvailable,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode,
           lt.IsPaid,
           e.Name AS EmployeeName
    FROM LeaveBalances lb WITH (NOLOCK)  -- ✅ Read without locking
    INNER JOIN LeaveTypes lt WITH (NOLOCK) ON lb.LeaveTypeId = lt.Id
    INNER JOIN Employees e WITH (NOLOCK) ON lb.EmployeeId = e.Id
    WHERE lb.EmployeeId = @EmployeeId
      AND lb.Year = @Year
      AND lb.IsActive = 1
    ORDER BY lt.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeaveCalendarData]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetLeaveCalendarData]
    @Month        INT,
    @Year         INT,
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        lr.Id,
        lr.EmployeeId,
        e.Name AS EmployeeName,
        d.DepartmentName AS DepartmentName,
        lt.Name AS LeaveTypeName,
        lt.Code AS LeaveTypeCode,
        lr.StartDate,
        lr.EndDate,
        lr.TotalDays,
        lr.Status,
        lr.IsHalfDay,
        lr.HalfDayType,
        lr.Reason
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.IsDeleted = 0
      AND lr.Status IN ('Approved', 'Pending')
      AND (
          (MONTH(lr.StartDate) = @Month AND YEAR(lr.StartDate) = @Year)
          OR (MONTH(lr.EndDate) = @Month AND YEAR(lr.EndDate) = @Year)
          OR (lr.StartDate <= DATEFROMPARTS(@Year, @Month, 1) 
              AND lr.EndDate >= EOMONTH(DATEFROMPARTS(@Year, @Month, 1)))
      )
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    ORDER BY lr.StartDate, e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeaveDashboardStats]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetLeaveDashboardStats]
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    -- Overall Stats
    SELECT 
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0) AS TotalRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Approved') AS ApprovedRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Rejected') AS RejectedRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Pending') AS PendingRequests,
        (SELECT COUNT(*) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Cancelled') AS CancelledRequests,
        (SELECT ISNULL(SUM(TotalDays), 0) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0 AND Status = 'Approved') AS TotalApprovedDays,
        (SELECT COUNT(DISTINCT EmployeeId) FROM LeaveRequests WHERE YEAR(StartDate) = @Year AND IsDeleted = 0) AS EmployeesWithLeave,
        (SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND IsDeleted = 0) AS TotalActiveEmployees;

    -- Monthly breakdown
    SELECT 
        MONTH(lr.StartDate) AS MonthNumber,
        DATENAME(MONTH, lr.StartDate) AS MonthName,
        COUNT(*) AS TotalRequests,
        SUM(CASE WHEN lr.Status = 'Approved' THEN 1 ELSE 0 END) AS Approved,
        SUM(CASE WHEN lr.Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,
        SUM(CASE WHEN lr.Status = 'Pending' THEN 1 ELSE 0 END) AS Pending,
        SUM(CASE WHEN lr.Status = 'Approved' THEN lr.TotalDays ELSE 0 END) AS ApprovedDays
    FROM LeaveRequests lr
    WHERE YEAR(lr.StartDate) = @Year AND lr.IsDeleted = 0
    GROUP BY MONTH(lr.StartDate), DATENAME(MONTH, lr.StartDate)
    ORDER BY MONTH(lr.StartDate);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeaveRequestById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetLeaveRequestById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT lr.Id, lr.EmployeeId, lr.LeaveTypeId, lr.StartDate, lr.EndDate,
           lr.TotalDays, lr.Reason, lr.Status, lr.IsHalfDay, lr.HalfDayType,
           lr.AttachmentPath, lr.EmergencyContact, lr.Remarks,
           lr.AppliedDate, lr.ApprovedBy, lr.ApprovedDate,
           lr.RejectedBy, lr.RejectedDate, lr.CancelledDate, lr.CancelReason,
           lr.IsActive, lr.IsDeleted, lr.CreatedBy, lr.CreatedDate,
           lr.UpdatedBy, lr.UpdatedDate,
           e.Name AS EmployeeName,
           e.Email AS EmployeeEmail,
           e.DepartmentId,
           d.DepartmentName AS DepartmentName,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.Id = @Id AND lr.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLeaveTypeById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetLeaveTypeById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, Description, DefaultDays, MaxDays,
           IsCarryForward, MaxCarryForward, IsPaid, IsActive,
           IsDeleted, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
    FROM LeaveTypes
    WHERE Id = @Id AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 6. Get Loan By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanById]
    @LoanId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        el.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        lt.LoanTypeName,
        lt.LoanTypeCode,
        d.DepartmentName
    FROM EmployeeLoans el
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE el.Id = @LoanId AND el.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanDashboardStats]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 25. Get Loan Dashboard Stats
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        (SELECT COUNT(*) FROM EmployeeLoans WHERE Status = 'Pending' AND IsDeleted = 0) AS PendingApprovals,
        (SELECT COUNT(*) FROM EmployeeLoans WHERE Status = 'Approved' AND IsDeleted = 0) AS PendingDisbursements,
        (SELECT COUNT(*) FROM EmployeeLoans WHERE Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0 AND IsDeleted = 0) AS ActiveLoans,
        (SELECT ISNULL(SUM(LoanAmount), 0) FROM EmployeeLoans WHERE Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0 AND IsDeleted = 0) AS TotalDisbursedAmount,
        (SELECT ISNULL(SUM(OutstandingAmount), 0) FROM EmployeeLoans WHERE Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0 AND IsDeleted = 0) AS TotalOutstandingAmount,
        (SELECT ISNULL(SUM(TotalAmountPaid), 0) FROM EmployeeLoans WHERE IsDeleted = 0) AS TotalRecoveredAmount,
        (SELECT COUNT(*) FROM LoanEMISchedule les 
         INNER JOIN EmployeeLoans el ON les.LoanId = el.Id 
         WHERE les.Status = 'Pending' AND les.EMIDueDate < GETDATE() AND el.Status = 'Active') AS OverdueEMIs;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanDetails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN SUMMARY & REPORTS
-- =============================================

-- 21. Get Loan Details With Schedule
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanDetails]
    @LoanId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Loan header
    SELECT 
        el.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        lt.LoanTypeName,
        lt.LoanTypeCode,
        d.DepartmentName,
        ua.FullName AS ApprovedByName,
        ud.FullName AS DisbursedByName,
        ge.Name AS GuarantorEmployeeName
    FROM EmployeeLoans el
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    LEFT JOIN Users ua ON el.ApprovedBy = ua.Id
    LEFT JOIN Users ud ON el.DisbursedBy = ud.Id
    LEFT JOIN Employees ge ON el.GuarantorEmployeeId = ge.Id
    WHERE el.Id = @LoanId AND el.IsDeleted = 0;

    -- EMI schedule
    SELECT * FROM LoanEMISchedule 
    WHERE LoanId = @LoanId
    ORDER BY EMINumber;

    -- Prepayments
    SELECT * FROM LoanPrepayments
    WHERE LoanId = @LoanId
    ORDER BY PrepaymentDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanEMISchedule]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 14. Get Loan EMI Schedule
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanEMISchedule]
    @LoanId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM LoanEMISchedule 
    WHERE LoanId = @LoanId
    ORDER BY EMINumber;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanSummary]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 22. Get Loan Summary For Employee
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanSummary]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        COUNT(*) AS TotalLoans,
        SUM(CASE WHEN Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0 THEN 1 ELSE 0 END) AS ActiveLoans,
        SUM(CASE WHEN IsFullyPaid = 1 OR Status = 'Closed' THEN 1 ELSE 0 END) AS ClosedLoans,
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingLoans,
        SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS RejectedLoans,
        SUM(LoanAmount) AS TotalLoanAmount,
        SUM(CASE WHEN IsFullyPaid = 0 THEN OutstandingAmount ELSE 0 END) AS TotalOutstanding,
        SUM(TotalAmountPaid) AS TotalPaid,
        SUM(CASE WHEN Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0 THEN EMIAmount ELSE 0 END) AS MonthlyEMIDeduction
    FROM EmployeeLoans
    WHERE EmployeeId = @EmployeeId AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetLoanTypeById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 2. Get Loan Type By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetLoanTypeById]
    @LoanTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM LoanTypes 
    WHERE Id = @LoanTypeId AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetMonthlyEMIDeduction]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 24. Get Monthly EMI Deduction
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetMonthlyEMIDeduction]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(EMIAmount), 0) AS MonthlyEMI
    FROM EmployeeLoans
    WHERE EmployeeId = @EmployeeId 
      AND Status IN ('Active', 'Disbursed')
      AND IsFullyPaid = 0
      AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetMonthlyLeaveReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetMonthlyLeaveReport]
    @Month        INT,
    @Year         INT,
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        d.DepartmentName AS DepartmentName,
        lt.Name AS LeaveTypeName,
        COUNT(lr.Id) AS TotalRequests,
        SUM(lr.TotalDays) AS TotalDays,
        SUM(CASE WHEN lr.Status = 'Approved' THEN lr.TotalDays ELSE 0 END) AS ApprovedDays,
        SUM(CASE WHEN lr.Status = 'Rejected' THEN lr.TotalDays ELSE 0 END) AS RejectedDays,
        SUM(CASE WHEN lr.Status = 'Pending' THEN lr.TotalDays ELSE 0 END) AS PendingDays,
        SUM(CASE WHEN lr.Status = 'Cancelled' THEN lr.TotalDays ELSE 0 END) AS CancelledDays
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.IsDeleted = 0
      AND MONTH(lr.StartDate) = @Month
      AND YEAR(lr.StartDate) = @Year
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    GROUP BY e.Id, e.Name, d.DepartmentName, lt.Name
    ORDER BY e.Name, lt.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetMyTickets]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 17: Get My Tickets
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetMyTickets]
    @UserId INT,
    @UserRole NVARCHAR(50),
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TicketId,
        t.TicketNumber,
        t.Title,
        t.Description,
        t.TicketType,
        t.Priority,
        t.Status,
        t.CreatedDate,
        t.UpdatedDate,
        t.DueDate,
        t.IsOverdue,
        creator.FullName AS CreatedByName,
        assignee.FullName AS AssignedToName,
        (SELECT COUNT(*) FROM TicketComments WHERE TicketId = t.TicketId AND IsDeleted = 0) AS CommentCount,
        (SELECT COUNT(*) FROM TicketAttachments WHERE TicketId = t.TicketId AND IsDeleted = 0) AS AttachmentCount
    FROM Tickets t
    LEFT JOIN Users creator ON t.CreatedBy = creator.Id
    LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id
    WHERE t.IsDeleted = 0
        AND (
            (@UserRole = 'QA' AND t.CreatedBy = @UserId) OR
            (@UserRole = 'Developer' AND t.AssignedTo = @UserId) OR
            (@UserRole = 'Admin')
        )
        AND (@Status IS NULL OR t.Status = @Status)
    ORDER BY 
        CASE WHEN t.IsOverdue = 1 THEN 0 ELSE 1 END,
        CASE t.Priority
            WHEN 'Critical' THEN 1
            WHEN 'High' THEN 2
            WHEN 'Medium' THEN 3
            WHEN 'Low' THEN 4
        END,
        t.CreatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNextPendingEMI]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 15. Get Next Pending EMI
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetNextPendingEMI]
    @LoanId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * 
    FROM LoanEMISchedule 
    WHERE LoanId = @LoanId AND Status = 'Pending'
    ORDER BY EMINumber;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOverdueEMIs]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 26. Get Overdue EMIs
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetOverdueEMIs]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        les.*,
        el.LoanNumber,
        el.EmployeeId,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        d.DepartmentName,
        lt.LoanTypeName,
        DATEDIFF(DAY, les.EMIDueDate, GETDATE()) AS DaysOverdue
    FROM LoanEMISchedule les
    INNER JOIN EmployeeLoans el ON les.LoanId = el.Id
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    WHERE les.Status = 'Pending' 
      AND les.EMIDueDate < GETDATE()
      AND el.Status = 'Active'
    ORDER BY les.EMIDueDate;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPaymentById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPaymentById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [Id], [EmployeeId], [Amount], [TransactionId], [OrderId], [PaymentStatus], [PaymentMethod], [Currency], [Description], [CreatedDate], [CompletedDate]
    FROM [dbo].[Payments]
    WHERE [Id] = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPaymentByOrderId]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPaymentByOrderId]
    @OrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [Id], [EmployeeId], [Amount], [TransactionId], [OrderId], [PaymentStatus], [PaymentMethod], [Currency], [Description], [CreatedDate], [CompletedDate]
    FROM [dbo].[Payments]
    WHERE [OrderId] = @OrderId;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPaymentByTransactionId]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPaymentByTransactionId]
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [Id], [EmployeeId], [Amount], [TransactionId], [OrderId], [PaymentStatus], [PaymentMethod], [Currency], [Description], [CreatedDate], [CompletedDate]
    FROM [dbo].[Payments]
    WHERE [TransactionId] = @TransactionId;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPaymentsByEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPaymentsByEmployee]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [Id], [EmployeeId], [Amount], [TransactionId], [OrderId], [PaymentStatus], [PaymentMethod], [Currency], [Description], [CreatedDate], [CompletedDate]
    FROM [dbo].[Payments]
    WHERE [EmployeeId] = @EmployeeId
    ORDER BY [CreatedDate] DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollCycleById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 1. Get Payroll Cycle By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollCycleById]
    @CycleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM PayrollCycle
    WHERE Id = @CycleId AND IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollCycleByMonthYear]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 2. Get Payroll Cycle By Month & Year
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollCycleByMonthYear]
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM PayrollCycle
    WHERE Month = @Month 
      AND Year = @Year 
      AND IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollCycles]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 3. Get Payroll Cycles
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollCycles]
    @Year INT,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM PayrollCycle
    WHERE Year = @Year
      AND IsActive = 1
      AND (@Status IS NULL OR Status = @Status)
    ORDER BY Month DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollDetails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 10. Get Payroll Details
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollDetails]
    @ProcessingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM PayrollProcessingDetails
    WHERE PayrollProcessingId = @ProcessingId
    ORDER BY ComponentType, DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollProcessingByCycle]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 8. Get Payroll Processing By Cycle
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollProcessingByCycle]
    @CycleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        pp.*, 
        e.Name AS EmployeeName, 
        d.DepartmentName
    FROM PayrollProcessing pp
    INNER JOIN Employees e ON pp.EmployeeId = e.Id
    INNER JOIN Departments d ON e.DepartmentId = d.Id
    WHERE pp.PayrollCycleId = @CycleId
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPayrollProcessingById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 7. Get Payroll Processing By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPayrollProcessingById]
    @ProcessingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM PayrollProcessing
    WHERE Id = @ProcessingId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingEmailQueue]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPendingEmailQueue]
    @BatchSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    -- Top 10 pending emails ko uthao aur unka status 'Processing' kar do taaki dobara na uthaye
    UPDATE TOP (@BatchSize) PayrollEmailQueue
    SET Status = 'Processing', UpdatedDate = GETDATE()
    OUTPUT INSERTED.QueueId, INSERTED.EmployeeId, INSERTED.EmailAddress, 
           INSERTED.EmployeeName, INSERTED.Month, INSERTED.Year, INSERTED.PayrollProcessId
    WHERE Status = 'Pending' OR (Status = 'Failed' AND RetryCount < 3);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingEMIsForPayroll]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 16. Get All Pending EMIs For Payroll
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPendingEMIsForPayroll]
    @PayrollCycleId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CycleMonth INT, @CycleYear INT;
    
    SELECT @CycleMonth = Month, @CycleYear = Year
    FROM PayrollCycle WHERE Id = @PayrollCycleId;

    SELECT 
        les.*,
        el.EmployeeId,
        el.LoanNumber,
        e.Name AS EmployeeName
    FROM LoanEMISchedule les
    INNER JOIN EmployeeLoans el ON les.LoanId = el.Id
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    WHERE les.Status = 'Pending'
      AND el.Status = 'Active'
      AND el.IsFullyPaid = 0
      AND MONTH(les.EMIDueDate) = @CycleMonth
      AND YEAR(les.EMIDueDate) = @CycleYear
    ORDER BY e.Name, les.EMINumber;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingLeaveRequests]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetPendingLeaveRequests]
    @ApproverId     INT = NULL,
    @DepartmentId   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT lr.Id, lr.EmployeeId, lr.LeaveTypeId, lr.StartDate, lr.EndDate,
           lr.TotalDays, lr.Reason, lr.Status, lr.IsHalfDay, lr.HalfDayType,
           lr.AttachmentPath, lr.EmergencyContact, lr.Remarks,
           lr.AppliedDate, lr.ApprovedBy, lr.ApprovedDate,
           lr.RejectedBy, lr.RejectedDate, lr.CancelledDate, lr.CancelReason,
           lr.IsActive, lr.IsDeleted, lr.CreatedBy, lr.CreatedDate,
           lr.UpdatedBy, lr.UpdatedDate,
           e.Name AS EmployeeName,
           e.Email AS EmployeeEmail,
           e.DepartmentId,
           d.DepartmentName AS DepartmentName,
           lt.Name AS LeaveTypeName,
           lt.Code AS LeaveTypeCode
    FROM LeaveRequests lr
    INNER JOIN Employees e ON lr.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    INNER JOIN LeaveTypes lt ON lr.LeaveTypeId = lt.Id
    WHERE lr.Status = 'Pending'
      AND lr.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    ORDER BY lr.AppliedDate ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingLoanApprovals]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 8. Get Pending Loan Approvals
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetPendingLoanApprovals]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        el.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        lt.LoanTypeName,
        d.DepartmentName,
        ess.GrossSalary,
        (SELECT ISNULL(SUM(OutstandingAmount), 0) FROM EmployeeLoans 
         WHERE EmployeeId = el.EmployeeId AND Status IN ('Active', 'Disbursed') AND IsFullyPaid = 0) AS ExistingLoanOutstanding
    FROM EmployeeLoans el
    INNER JOIN Employees e ON el.EmployeeId = e.Id
    INNER JOIN LoanTypes lt ON el.LoanTypeId = lt.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    LEFT JOIN EmployeeSalaryStructure ess ON e.Id = ess.EmployeeId AND ess.IsCurrentStructure = 1
    WHERE el.Status = 'Pending' 
      AND el.IsDeleted = 0
    ORDER BY el.ApplicationDate;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingPayments]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetPendingPayments]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [Id], [EmployeeId], [Amount], [TransactionId], [OrderId], [PaymentStatus], [PaymentMethod], [Currency], [Description], [CreatedDate]
    FROM [dbo].[Payments]
    WHERE [PaymentStatus] = 'Pending'
    ORDER BY [CreatedDate];
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetPendingUsers]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_GetPendingUsers]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id,
        u.Username,
        u.Email,
        ISNULL(u.FullName, CONCAT(u.FirstName, ' ', u.LastName)) AS FullName,
        ISNULL(u.PhoneNumber, '') AS PhoneNumber,
        u.IsActive,
        ISNULL(u.RegistrationStatus, CASE WHEN u.IsActive = 1 THEN 'Approved' ELSE 'Pending' END) AS RegistrationStatus,
        ISNULL(u.CreatedDate, GETDATE()) AS CreatedDate,
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.DepartmentId,
        d.DepartmentName AS DepartmentName,
        (
            SELECT STRING_AGG(r.RoleName, ', ')
            FROM dbo.UserRoles ur
            INNER JOIN dbo.Roles r ON ur.RoleId = r.Id
            WHERE ur.UserId = u.Id AND ISNULL(ur.IsActive, 1) = 1
        ) AS AssignedRoles
    FROM dbo.Users u
    LEFT JOIN dbo.Employees e ON e.Email = u.Email
    LEFT JOIN dbo.Departments d ON e.DepartmentId = d.Id
    WHERE u.IsActive = 0
       OR u.RegistrationStatus = 'Pending'
       OR NOT EXISTS (SELECT 1 FROM dbo.UserRoles ur2 WHERE ur2.UserId = u.Id AND ISNULL(ur2.IsActive, 1) = 1)
    ORDER BY u.CreatedDate DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetRecentEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 32. GET RECENT EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetRecentEmployees]
    @Days INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0 
    AND e.CreatedDate >= DATEADD(DAY, -@Days, GETDATE())
    ORDER BY e.CreatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRefreshToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 9. sp_GetRefreshToken
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetRefreshToken]
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        rt.Id,
        rt.UserId,
        rt.Token,
        rt.ExpiryDate,
        rt.CreatedDate,
        rt.IpAddress,
        rt.IsRevoked,
        rt.RevokedDate,
        rt.IsUsed,
        rt.UsedDate,
        u.Id AS User_Id,
        u.Username AS User_Username,
        u.Email AS User_Email,
        u.FullName AS User_FullName
    FROM RefreshTokens rt
    INNER JOIN Users u ON rt.UserId = u.Id
    WHERE rt.Token = @Token;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRoles]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetRoles]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id AS RoleId,
        r.RoleName,
        r.Description
    FROM UserRoles ur
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId 
      AND ur.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryComponentByCode]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 3. Get Salary Component By Code
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryComponentByCode]
    @ComponentCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM SalaryComponents 
    WHERE ComponentCode = @ComponentCode AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryComponentById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 2. Get Salary Component By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryComponentById]
    @ComponentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM SalaryComponents 
    WHERE Id = @ComponentId AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- ✅ REPORT 3: SALARY REPORT
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryReport]
    @DepartmentId INT = NULL,
    @Month        INT = NULL,
    @Year         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Month IS NULL SET @Month = MONTH(GETDATE());
    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    DECLARE @StartDate DATE = DATEFROMPARTS(@Year, @Month, 1);
    DECLARE @EndDate DATE = EOMONTH(@StartDate);

    -- Calculate working days
    DECLARE @TotalWorkingDays INT = 0;
    DECLARE @TempDate DATE = @StartDate;
    WHILE @TempDate <= @EndDate
    BEGIN
        IF DATEPART(WEEKDAY, @TempDate) NOT IN (1, 7)
            SET @TotalWorkingDays = @TotalWorkingDays + 1;
        SET @TempDate = DATEADD(DAY, 1, @TempDate);
    END

    -- Subtract holidays
    SELECT @TotalWorkingDays = @TotalWorkingDays - COUNT(*)
    FROM Holidays
    WHERE Date BETWEEN @StartDate AND @EndDate
      AND IsActive = 1 AND IsDeleted = 0
      AND DATEPART(WEEKDAY, Date) NOT IN (1, 7);

    -- Result Set 1: Summary
    SELECT 
        COUNT(*) AS TotalEmployees,
        ISNULL(SUM(e.Salary), 0) AS TotalMonthlySalary,
        ISNULL(AVG(e.Salary), 0) AS AverageSalary,
        ISNULL(MAX(e.Salary), 0) AS HighestSalary,
        ISNULL(MIN(e.Salary), 0) AS LowestSalary,
        @TotalWorkingDays AS WorkingDays,
        DATENAME(MONTH, @StartDate) AS MonthName,
        @Year AS ReportYear
    FROM Employees e
    WHERE e.IsActive = 1 AND e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId);

    -- Result Set 2: Employee Salary Details
    SELECT 
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.Email,
        d.DepartmentName AS DepartmentName,
        e.Role,
        e.Salary AS MonthlySalary,
        CAST(e.Salary / @TotalWorkingDays AS DECIMAL(10,2)) AS DailySalary,
        @TotalWorkingDays AS WorkingDays,
        ISNULL(
            (SELECT SUM(lr.TotalDays) FROM LeaveRequests lr 
             WHERE lr.EmployeeId = e.Id AND lr.Status = 'Approved' AND lr.IsDeleted = 0
               AND lr.StartDate <= @EndDate AND lr.EndDate >= @StartDate), 0
        ) AS LeaveDays,
        -- LWP Days (unpaid leave)
        ISNULL(
            (SELECT SUM(lr2.TotalDays) FROM LeaveRequests lr2 
             INNER JOIN LeaveTypes lt ON lr2.LeaveTypeId = lt.Id
             WHERE lr2.EmployeeId = e.Id AND lr2.Status = 'Approved' AND lr2.IsDeleted = 0
               AND lt.IsPaid = 0
               AND lr2.StartDate <= @EndDate AND lr2.EndDate >= @StartDate), 0
        ) AS UnpaidLeaveDays,
        -- Present Days
        @TotalWorkingDays - ISNULL(
            (SELECT SUM(lr3.TotalDays) FROM LeaveRequests lr3 
             WHERE lr3.EmployeeId = e.Id AND lr3.Status = 'Approved' AND lr3.IsDeleted = 0
               AND lr3.StartDate <= @EndDate AND lr3.EndDate >= @StartDate), 0
        ) AS PresentDays,
        -- Net Salary (deduct LWP days)
        e.Salary - (
            CAST(e.Salary / @TotalWorkingDays AS DECIMAL(10,2)) * 
            ISNULL(
                (SELECT SUM(lr4.TotalDays) FROM LeaveRequests lr4 
                 INNER JOIN LeaveTypes lt2 ON lr4.LeaveTypeId = lt2.Id
                 WHERE lr4.EmployeeId = e.Id AND lr4.Status = 'Approved' AND lr4.IsDeleted = 0
                   AND lt2.IsPaid = 0
                   AND lr4.StartDate <= @EndDate AND lr4.EndDate >= @StartDate), 0
            )
        ) AS NetSalary,
        -- Deduction Amount
        CAST(e.Salary / @TotalWorkingDays AS DECIMAL(10,2)) * 
        ISNULL(
            (SELECT SUM(lr5.TotalDays) FROM LeaveRequests lr5 
             INNER JOIN LeaveTypes lt3 ON lr5.LeaveTypeId = lt3.Id
             WHERE lr5.EmployeeId = e.Id AND lr5.Status = 'Approved' AND lr5.IsDeleted = 0
               AND lt3.IsPaid = 0
               AND lr5.StartDate <= @EndDate AND lr5.EndDate >= @StartDate), 0
        ) AS DeductionAmount,
        e.JoiningDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsActive = 1 AND e.IsDeleted = 0
      AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
    ORDER BY d.DepartmentName, e.Name;

    -- Result Set 3: Department Salary Summary
    SELECT 
        d.DepartmentName AS DepartmentName,
        COUNT(e.Id) AS EmployeeCount,
        ISNULL(SUM(e.Salary), 0) AS TotalSalary,
        ISNULL(AVG(e.Salary), 0) AS AvgSalary,
        ISNULL(MAX(e.Salary), 0) AS MaxSalary,
        ISNULL(MIN(e.Salary), 0) AS MinSalary
    FROM Departments d
    LEFT JOIN Employees e ON d.Id = e.DepartmentId AND e.IsActive = 1 AND e.IsDeleted = 0
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentName
    ORDER BY TotalSalary DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalarySlipData]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetSalarySlipData]
    @SlipId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- Get Basic Context (Performance Optimization)
    -- =============================================
    DECLARE @EmpId INT, @Year INT, @Month INT;

    SELECT 
        @EmpId = EmployeeId,
        @Year = Year,
        @Month = Month
    FROM SalarySlips
    WHERE Id = @SlipId;

    -- =============================================
    -- 1. Salary Slip Header
    -- =============================================
    SELECT 
        ss.SlipNumber,
        ss.Month,
        ss.Year,
        DATENAME(MONTH, DATEFROMPARTS(ss.Year, ss.Month, 1)) AS MonthName,
        ss.PayPeriodStart,
        ss.PayPeriodEnd,
        ss.PaymentDate,
        
        -- Employee details
        e.Id AS EmployeeId,
        e.Name AS EmployeeName,
        e.Email AS EmployeeEmail,
        e.PhoneNumber,
        d.DepartmentName,
        e.JoiningDate,
        
        -- Salary summary
        ss.BasicSalary,
        ss.GrossSalary,
        ss.TotalEarnings,
        ss.TotalDeductions,
        ss.NetSalary,
        
        -- Attendance summary
        ss.TotalWorkingDays,
        ss.PresentDays,
        ss.PaidLeaveDays,
        ss.LOPDays,
        
        -- Bank details
        eb.BankName,
        eb.AccountNumber,
        eb.IFSCCode,
        
        -- Company details
        'ABC Technologies Pvt Ltd' AS CompanyName,
        'Mumbai, Maharashtra' AS CompanyAddress,
        'GSTIN: 27AAAAA0000A1Z5' AS CompanyGSTIN,
        'PAN: AAAAA0000A' AS CompanyPAN
        
    FROM SalarySlips ss
    INNER JOIN Employees e ON ss.EmployeeId = e.Id
    INNER JOIN Departments d ON e.DepartmentId = d.Id
    LEFT JOIN EmployeeBankDetails eb 
        ON e.Id = eb.EmployeeId AND eb.IsPrimaryAccount = 1
    WHERE ss.Id = @SlipId;

    -- =============================================
    -- 2. Earnings Breakdown
    -- =============================================
    SELECT 
        ppd.ComponentName,
        ppd.ComponentCode,
        ppd.Amount,
        ppd.DisplayOrder,
        ppd.AdjustedForLOP,
        ppd.OriginalAmount
    FROM PayrollProcessingDetails ppd
    INNER JOIN SalarySlips ss 
        ON ppd.PayrollProcessingId = ss.PayrollProcessingId
    WHERE ss.Id = @SlipId
      AND ppd.ComponentType = 'Earning'
    ORDER BY ppd.DisplayOrder;

    -- =============================================
    -- 3. Deductions Breakdown
    -- =============================================
    SELECT 
        ppd.ComponentName,
        ppd.ComponentCode,
        ppd.Amount,
        ppd.DisplayOrder
    FROM PayrollProcessingDetails ppd
    INNER JOIN SalarySlips ss 
        ON ppd.PayrollProcessingId = ss.PayrollProcessingId
    WHERE ss.Id = @SlipId
      AND ppd.ComponentType = 'Deduction'
    ORDER BY ppd.DisplayOrder;

    -- =============================================
    -- 4. Year-To-Date Summary (FIXED)
    -- =============================================
    SELECT 
        SUM(ss.TotalEarnings) AS YTDEarnings,
        SUM(ss.TotalDeductions) AS YTDDeductions,
        SUM(ss.NetSalary) AS YTDNetSalary,

        SUM(CASE 
            WHEN ppd.ComponentCode = 'PF_EMP' THEN ppd.Amount 
            ELSE 0 
        END) AS YTDPF,

        SUM(CASE 
            WHEN ppd.ComponentCode = 'TDS' THEN ppd.Amount 
            ELSE 0 
        END) AS YTDTDS

    FROM SalarySlips ss
    INNER JOIN PayrollProcessing pp 
        ON ss.PayrollProcessingId = pp.Id
    LEFT JOIN PayrollProcessingDetails ppd 
        ON pp.Id = ppd.PayrollProcessingId

    WHERE ss.EmployeeId = @EmpId
      AND ss.Year = @Year
      AND ss.Month <= @Month;

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryStructureById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 19. Get Salary Structure By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryStructureById]
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ess.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        st.TemplateName,
        st.TemplateCode
    FROM EmployeeSalaryStructure ess
    INNER JOIN Employees e ON ess.EmployeeId = e.Id
    LEFT JOIN SalaryTemplates st ON ess.TemplateId = st.Id
    WHERE ess.Id = @StructureId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryStructureDetails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 22. Get Salary Structure Details (with components)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryStructureDetails]
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Return structure header
    SELECT 
        ess.*,
        e.Name AS EmployeeName,
        e.EmployeeCode,
        d.DepartmentName,
        st.TemplateName,
        st.TemplateCode
    FROM EmployeeSalaryStructure ess
    INNER JOIN Employees e ON ess.EmployeeId = e.Id
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    LEFT JOIN SalaryTemplates st ON ess.TemplateId = st.Id
    WHERE ess.Id = @StructureId;

    -- Return components breakdown
    SELECT 
        esc.ComponentId,
        sc.ComponentCode,
        sc.ComponentName,
        esc.ComponentType,
        esc.CalculationType,
        esc.Percentage,
        esc.CalculationBase,
        esc.MonthlyAmount,
        esc.AnnualAmount,
        sc.IsStatutory,
        sc.IsTaxable,
        esc.DisplayOrder
    FROM EmployeeSalaryComponents esc
    INNER JOIN SalaryComponents sc ON esc.ComponentId = sc.Id
    WHERE esc.EmployeeSalaryStructureId = @StructureId 
      AND esc.IsActive = 1
      AND esc.IsDeleted = 0
    ORDER BY esc.ComponentType, esc.DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryTemplateByCode]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 9. Get Template By Code
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryTemplateByCode]
    @TemplateCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        st.*,
        d.DepartmentName
    FROM SalaryTemplates st
    LEFT JOIN Departments d ON st.DepartmentId = d.Id
    WHERE st.TemplateCode = @TemplateCode AND st.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSalaryTemplateById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 8. Get Template By Id
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetSalaryTemplateById]
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        st.*,
        d.DepartmentName
    FROM SalaryTemplates st
    LEFT JOIN Departments d ON st.DepartmentId = d.Id
    WHERE st.Id = @TemplateId AND st.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentAttendanceReport]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetStudentAttendanceReport]
    @StudentId INT,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Set default dates if not provided
    IF @FromDate IS NULL
        SET @FromDate = DATEADD(MONTH, -1, GETDATE());
    IF @ToDate IS NULL
        SET @ToDate = GETDATE();
    
    SELECT 
        a.AttendanceId,
        a.AttendanceDate,
        a.AttendanceTime,
        a.Status,
        a.ConfidenceScore,
        a.Remarks,
        a.CapturedImagePath
    FROM Attendance a
    WHERE a.StudentId = @StudentId
    AND a.AttendanceDate BETWEEN @FromDate AND @ToDate
    ORDER BY a.AttendanceDate DESC, a.AttendanceTime DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetStudentById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        StudentId,
        FirstName,
        LastName,
        FullName,
        Class,
        Subjects,
        Age,
        DateOfBirth,
        JoiningDate,
        BatchTime,
        PassportPhotoPath,
        PhoneNumber,
        Email,
        Address,
        ParentName,
        ParentPhone,
        ParentEmail,
        IsActive,
        IsDeleted,
        CreatedBy,
        CreatedDate,
        UpdatedBy,
        UpdatedDate
    FROM Students
    WHERE Id = @Id AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentByStudentId]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetStudentByStudentId]
    @StudentId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        StudentId,
        FirstName,
        LastName,
        FullName,
        Class,
        Subjects,
        Age,
        DateOfBirth,
        JoiningDate,
        BatchTime,
        PassportPhotoPath,
        PhoneNumber,
        Email,
        Address,
        ParentName,
        ParentPhone,
        ParentEmail,
        IsActive,
        IsDeleted,
        CreatedBy,
        CreatedDate,
        UpdatedBy,
        UpdatedDate
    FROM Students
    WHERE StudentId = @StudentId AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentsByClass]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetStudentsByClass]
    @Class NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        StudentId,
        FirstName,
        LastName,
        FullName,
        Class,
        Subjects,
        Age,
        JoiningDate,
        BatchTime,
        PassportPhotoPath,
        PhoneNumber,
        Email,
        IsActive
    FROM Students
    WHERE Class = @Class AND IsDeleted = 0
    ORDER BY FullName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentsPaged]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetStudentsPaged]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL,
    @Class NVARCHAR(50) = NULL,
    @IsActive BIT = NULL,
    @SortBy NVARCHAR(50) = 'FullName',
    @SortOrder NVARCHAR(4) = 'ASC',
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate offset
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    SELECT @TotalRecords = COUNT(*)
    FROM Students
    WHERE IsDeleted = 0
    AND (@SearchTerm IS NULL OR (
        FullName LIKE '%' + @SearchTerm + '%'
        OR StudentId LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%'
    ))
    AND (@Class IS NULL OR Class = @Class)
    AND (@IsActive IS NULL OR IsActive = @IsActive);
    
    -- Get paginated data
    DECLARE @SQL NVARCHAR(MAX);
    SET @SQL = '
    SELECT 
        Id, StudentId, FirstName, LastName, FullName, Class, Subjects,
        Age, DateOfBirth, JoiningDate, BatchTime, PassportPhotoPath,
        PhoneNumber, Email, Address, IsActive, CreatedDate
    FROM Students
    WHERE IsDeleted = 0
    AND (@SearchTerm IS NULL OR (
        FullName LIKE ''%'' + @SearchTerm + ''%''
        OR StudentId LIKE ''%'' + @SearchTerm + ''%''
        OR Email LIKE ''%'' + @SearchTerm + ''%''
    ))
    AND (@Class IS NULL OR Class = @Class)
    AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY ' + QUOTENAME(@SortBy) + ' ' + @SortOrder + '
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY';
    
    EXEC sp_executesql @SQL,
        N'@SearchTerm NVARCHAR(100), @Class NVARCHAR(50), @IsActive BIT, @Offset INT, @PageSize INT',
        @SearchTerm, @Class, @IsActive, @Offset, @PageSize;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTemplateComponents]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 10. Get Template Components
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetTemplateComponents]
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        stc.*,
        sc.ComponentCode,
        sc.ComponentName,
        sc.ComponentType,
        sc.IsStatutory,
        sc.IsTaxable
    FROM SalaryTemplateComponents stc
    INNER JOIN SalaryComponents sc ON stc.ComponentId = sc.Id
    WHERE stc.TemplateId = @TemplateId AND stc.IsActive = 1
    ORDER BY sc.ComponentType, stc.DisplayOrder;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTicketAttachments]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 11: Get Ticket Attachments
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTicketAttachments]
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.AttachmentId,
        a.TicketId,
        a.FileName,
        a.FilePath,
        a.FileSize,
        a.FileType,
        a.UploadedDate,
        a.UploadedBy,
        u.FullName AS UploadedByName
    FROM TicketAttachments a
    INNER JOIN Users u ON a.UploadedBy = u.Id
    WHERE a.TicketId = @TicketId AND a.IsDeleted = 0
    ORDER BY a.UploadedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTicketById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 3: Get Ticket by ID
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTicketById]
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TicketId,
        t.TicketNumber,
        t.Title,
        t.Description,
        t.TicketType,
        t.Priority,
        t.Status,
        t.CreatedDate,
        t.UpdatedDate,
        t.DueDate,
        t.ResolvedDate,
        t.ClosedDate,
        t.StepsToReproduce,
        t.ExpectedResult,
        t.ActualResult,
        t.Environment,
        t.IsOverdue,
        
        t.CreatedBy,
        creator.FullName AS CreatedByName,
        creator.Email AS CreatedByEmail,
        
        t.AssignedTo,
        assignee.FullName AS AssignedToName,
        assignee.Email AS AssignedToEmail
        
    FROM Tickets t
    LEFT JOIN Users creator ON t.CreatedBy = creator.Id
    LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id
    WHERE t.TicketId = @TicketId AND t.IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTicketComments]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 9: Get Ticket Comments
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTicketComments]
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CommentId,
        c.TicketId,
        c.Comment,
        c.CreatedDate,
        c.UserId,
        u.FullName AS UserName,
        u.Email AS UserEmail,
        u.ProfilePicture
    FROM TicketComments c
    INNER JOIN Users u ON c.UserId = u.Id
    WHERE c.TicketId = @TicketId AND c.IsDeleted = 0
    ORDER BY c.CreatedDate ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTicketDashboard]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetTicketDashboard]
    @UserId INT = NULL,
    @UserRole NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- ============================================================
    -- 1. STATS (Result Set 1)
    -- ============================================================
    SELECT
        ISNULL(COUNT(*), 0) AS TotalTickets,

        -- ✅ Match status names with C# code
        ISNULL(SUM(CASE WHEN Status = 'New' THEN 1 ELSE 0 END), 0) AS NewTickets,
        ISNULL(SUM(CASE WHEN Status = 'Assigned' THEN 1 ELSE 0 END), 0) AS AssignedTickets,
        ISNULL(SUM(CASE WHEN Status IN ('InProgress', 'In Progress') THEN 1 ELSE 0 END), 0) AS InProgressTickets,
        ISNULL(SUM(CASE WHEN Status IN ('OnHold', 'On Hold', 'Blocked') THEN 1 ELSE 0 END), 0) AS BlockedTickets,
        ISNULL(SUM(CASE WHEN Status = 'Resolved' THEN 1 ELSE 0 END), 0) AS ResolvedTickets,
        ISNULL(SUM(CASE WHEN Status = 'Closed' THEN 1 ELSE 0 END), 0) AS ClosedTickets,
        ISNULL(SUM(CASE WHEN Status = 'Reopened' THEN 1 ELSE 0 END), 0) AS ReopenedTickets,
        ISNULL(SUM(CASE WHEN Priority = 'Critical' THEN 1 ELSE 0 END), 0) AS CriticalTickets,
        ISNULL(SUM(CASE WHEN Priority = 'High' THEN 1 ELSE 0 END), 0) AS HighPriorityTickets,
        ISNULL(SUM(CASE WHEN IsOverdue = 1 THEN 1 ELSE 0 END), 0) AS OverdueTickets
    FROM Tickets
    WHERE IsDeleted = 0
        AND (
            @UserId IS NULL
            OR (@UserRole = 'QA' AND CreatedBy = @UserId)
            OR (@UserRole IN ('Developer', 'Admin') AND AssignedTo = @UserId)
        );

    -- ============================================================
    -- 2. PRIORITY DISTRIBUTION (Result Set 2)
    -- ============================================================
    SELECT
        Priority,
        COUNT(*) AS Count
    FROM Tickets
    WHERE IsDeleted = 0
        AND Priority IS NOT NULL
        AND (
            @UserId IS NULL
            OR (@UserRole = 'QA' AND CreatedBy = @UserId)
            OR (@UserRole IN ('Developer', 'Admin') AND AssignedTo = @UserId)
        )
    GROUP BY Priority
    ORDER BY
        CASE Priority
            WHEN 'Critical' THEN 1
            WHEN 'High' THEN 2
            WHEN 'Medium' THEN 3
            WHEN 'Low' THEN 4
            ELSE 5
        END;

    -- ============================================================
    -- 3. TYPE DISTRIBUTION (Result Set 3)
    -- ============================================================
    SELECT
        TicketType,
        COUNT(*) AS Count
    FROM Tickets
    WHERE IsDeleted = 0
        AND TicketType IS NOT NULL
        AND (
            @UserId IS NULL
            OR (@UserRole = 'QA' AND CreatedBy = @UserId)
            OR (@UserRole IN ('Developer', 'Admin') AND AssignedTo = @UserId)
        )
    GROUP BY TicketType
    ORDER BY Count DESC;

    -- ============================================================
    -- 4. RECENT ACTIVITIES (Result Set 4)
    -- ============================================================
    SELECT TOP 10
        t.TicketId,
        ISNULL(t.TicketNumber, '') AS TicketNumber,
        ISNULL(t.Title, '') AS Title,
        ISNULL(t.Status, 'New') AS Status,
        ISNULL(t.Priority, 'Medium') AS Priority,
        t.UpdatedDate,
        ISNULL(creator.FullName, 'Unknown') AS CreatedByName,
        assignee.FullName AS AssignedToName
    FROM Tickets t
    LEFT JOIN Users creator ON t.CreatedBy = creator.Id
    LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id
    WHERE t.IsDeleted = 0
        AND (
            @UserId IS NULL
            OR (@UserRole = 'QA' AND t.CreatedBy = @UserId)
            OR (@UserRole IN ('Developer', 'Admin') AND t.AssignedTo = @UserId)
        )
    ORDER BY t.UpdatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTicketHistory]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 13: Get Ticket History
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTicketHistory]
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.HistoryId,
        h.TicketId,
        h.ChangeType,
        h.OldValue,
        h.NewValue,
        h.Remarks,
        h.ChangeDate,
        u.FullName AS ChangedByName,
        u.Email AS ChangedByEmail
    FROM TicketHistory h
    INNER JOIN Users u ON h.ChangedBy = u.Id
    WHERE h.TicketId = @TicketId
    ORDER BY h.ChangeDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTickets]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 4: Get Tickets with Filters
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTickets]
    @Status NVARCHAR(50) = NULL,
    @Priority NVARCHAR(50) = NULL,
    @TicketType NVARCHAR(50) = NULL,
    @AssignedTo INT = NULL,
    @CreatedBy INT = NULL,
    @SearchTerm NVARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        t.TicketId,
        t.TicketNumber,
        t.Title,
        t.Description,
        t.TicketType,
        t.Priority,
        t.Status,
        t.CreatedDate,
        t.UpdatedDate,
        t.DueDate,
        t.IsOverdue,
        
        creator.FullName AS CreatedByName,
        assignee.FullName AS AssignedToName,
        
        (SELECT COUNT(*) FROM TicketComments WHERE TicketId = t.TicketId AND IsDeleted = 0) AS CommentCount,
        (SELECT COUNT(*) FROM TicketAttachments WHERE TicketId = t.TicketId AND IsDeleted = 0) AS AttachmentCount
        
    FROM Tickets t
    LEFT JOIN Users creator ON t.CreatedBy = creator.Id
    LEFT JOIN Users assignee ON t.AssignedTo = assignee.Id
    WHERE t.IsDeleted = 0
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@Priority IS NULL OR t.Priority = @Priority)
        AND (@TicketType IS NULL OR t.TicketType = @TicketType)
        AND (@AssignedTo IS NULL OR t.AssignedTo = @AssignedTo)
        AND (@CreatedBy IS NULL OR t.CreatedBy = @CreatedBy)
        AND (@SearchTerm IS NULL OR 
             t.Title LIKE '%' + @SearchTerm + '%' OR 
             t.Description LIKE '%' + @SearchTerm + '%' OR
             t.TicketNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY t.CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    SELECT COUNT(*) AS TotalRecords
    FROM Tickets t
    WHERE t.IsDeleted = 0
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@Priority IS NULL OR t.Priority = @Priority)
        AND (@TicketType IS NULL OR t.TicketType = @TicketType)
        AND (@AssignedTo IS NULL OR t.AssignedTo = @AssignedTo)
        AND (@CreatedBy IS NULL OR t.CreatedBy = @CreatedBy)
        AND (@SearchTerm IS NULL OR 
             t.Title LIKE '%' + @SearchTerm + '%' OR 
             t.Description LIKE '%' + @SearchTerm + '%' OR
             t.TicketNumber LIKE '%' + @SearchTerm + '%');
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTotalLoanOutstanding]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 23. Get Total Loan Outstanding
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetTotalLoanOutstanding]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(OutstandingAmount), 0) AS TotalOutstanding
    FROM EmployeeLoans
    WHERE EmployeeId = @EmployeeId 
      AND Status IN ('Active', 'Disbursed')
      AND IsFullyPaid = 0
      AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTotalPaymentsByEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetTotalPaymentsByEmployee]
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT SUM([Amount]) AS TotalAmount
    FROM [dbo].[Payments]
    WHERE [EmployeeId] = @EmployeeId AND [PaymentStatus] IN ('Completed', 'Refunded');
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserActiveTokens]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 13. sp_GetUserActiveTokens
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetUserActiveTokens]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Token,
        ExpiryDate,
        CreatedDate,
        IpAddress,
        IsRevoked,
        RevokedDate,
        IsUsed,
        UsedDate
    FROM RefreshTokens
    WHERE UserId = @UserId
    AND IsRevoked = 0
    AND IsUsed = 0
    AND ExpiryDate > GETDATE()
    ORDER BY CreatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserActivityLog]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetUserActivityLog]
    @UserId INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    SELECT COUNT(*) AS TotalRecords
    FROM AuditLogs
    WHERE UserId = @UserId;
    
    -- Get paginated activity log
    SELECT 
        Id,
        UserId,
        Action,
        TableName,
        RecordId,
        OldValues,
        NewValues,
        IpAddress,
        UserAgent,
        CreatedDate
    FROM AuditLogs
    WHERE UserId = @UserId
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserByEmail]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 2. sp_GetUserByEmail (with roles)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetUserByEmail]
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    -- Get User
    SELECT 
        u.Id,
        u.Username,
        u.Email,
        u.PasswordHash,
        u.FirstName,
        u.LastName,
        u.FullName,
        u.PhoneNumber,
        u.IsActive,
        u.LastLoginDate,
        u.LastLoginIp,
        u.CreatedDate,
        u.UpdatedDate,
        u.PasswordChangedDate
    FROM Users u
    WHERE u.Email = @Email AND u.IsActive = 1;

    -- Get User Roles
    SELECT 
        ur.Id,
        ur.UserId,
        ur.RoleId,
        ur.IsActive,
        ur.AssignedDate,
        ur.AssignedBy,
        r.Id AS RoleId,
        r.RoleName,
        r.Description,
        r.IsActive AS RoleIsActive
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.Id
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE u.Email = @Email 
    AND u.IsActive = 1
    AND ur.IsActive = 1 
    AND r.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserById]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   2. GET USER BY ID
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_GetUserById]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.Username,
        u.FullName,
        u.Email,
        u.PhoneNumber,
        u.ProfilePicture,
        u.IsActive,
        u.IsEmailVerified,
        u.CreatedDate,
        u.LastLoginDate
    FROM Users u
    WHERE u.Id = @UserId
      AND ISNULL(u.IsDeleted, 0) = 0;

    SELECT
        ur.Id AS UserRoleId,
        ur.RoleId,
        r.RoleName,
        ISNULL(r.RoleDescription, r.Description) AS RoleDescription,
        ur.AssignedDate,
        ur.AssignedBy,
        ur.IsActive
    FROM UserRoles ur
    INNER JOIN Roles r 
        ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId
      AND ur.IsActive = 1
    ORDER BY r.RoleName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserByRefreshToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUserByRefreshToken]
(
    @RefreshToken NVARCHAR(500)
)
AS
BEGIN

    SELECT U.Id, U.Username, U.Email
    FROM Users U
    INNER JOIN RefreshTokens R
        ON U.Id = R.UserId
    WHERE R.Token = @RefreshToken
    AND R.ExpiryDate > GETUTCDATE()

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserByUsername]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- 1. sp_GetUserByUsername (with roles)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetUserByUsername]
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Get User
    SELECT 
        u.Id,
        u.Username,
        u.Email,
        u.PasswordHash,
        u.FirstName,
        u.LastName,
        u.FullName,
        u.PhoneNumber,
        u.IsActive,
        u.LastLoginDate,
        u.LastLoginIp,
        u.CreatedDate,
        u.UpdatedDate,
        u.PasswordChangedDate
    FROM Users u
    WHERE u.Username = @Username AND u.IsActive = 1;

    -- Get User Roles
    SELECT 
        ur.Id,
        ur.UserId,
        ur.RoleId,
        ur.IsActive,
        ur.AssignedDate,
        ur.AssignedBy,
        r.Id AS RoleId,
        r.RoleName,
        r.Description,
        r.IsActive AS RoleIsActive
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.Id
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE u.Username = @Username 
    AND u.IsActive = 1
    AND ur.IsActive = 1 
    AND r.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserByUsernameOrEmail]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. sp_GetUserByUsernameOrEmail (combined lookup)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_GetUserByUsernameOrEmail]
    @UsernameOrEmail NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    -- Get User
    SELECT 
        u.Id,
        u.Username,
        u.Email,
        u.PasswordHash,
        u.FirstName,
        u.LastName,
        u.FullName,
        u.PhoneNumber,
        u.IsActive,
        u.LastLoginDate,
        u.LastLoginIp,
        u.CreatedDate,
        u.UpdatedDate,
        u.PasswordChangedDate
    FROM Users u
    WHERE (u.Username = @UsernameOrEmail OR u.Email = @UsernameOrEmail) 
    AND u.IsActive = 1;

    -- Get User Roles
    SELECT 
        ur.Id,
        ur.UserId,
        ur.RoleId,
        ur.IsActive,
        ur.AssignedDate,
        ur.AssignedBy,
        r.Id AS RoleId,
        r.RoleName,
        r.Description,
        r.IsActive AS RoleIsActive
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.Id
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE (u.Username = @UsernameOrEmail OR u.Email = @UsernameOrEmail)
    AND u.IsActive = 1
    AND ur.IsActive = 1 
    AND r.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserManagementDropdowns]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetUserManagementDropdowns]
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Roles
    EXEC dbo.sp_GetAllRolesWithCount;

    -- 2. Departments
    SELECT
        Id,
        DepartmentName AS Name
    FROM dbo.Departments
    WHERE IsActive = 1
    ORDER BY DepartmentName;

    -- 3. Designations
    SELECT
        Id,
        DesignationName AS Name,
        DepartmentId
    FROM dbo.Designations
    WHERE IsActive = 1
    ORDER BY DesignationName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserPasswordHash]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_GetUserPasswordHash]  
    @UserId INT  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT PasswordHash  
    FROM Users  
    WHERE Id = @UserId AND IsActive = 1;  
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserPermissions]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUserPermissions]
    @UserId INT
AS
BEGIN
    SELECT 
        p.Id AS PermissionId,
        p.PermissionName AS PermissionName,
        p.Description,
        p.Module
    FROM UserRoles ur
    INNER JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
    INNER JOIN Permissions p ON rp.PermissionId = p.Id
    WHERE ur.UserId = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserProfile]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_GetUserProfile]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.Id,
        u.Username,
        u.Email,
        u.FirstName,
        u.LastName,
        u.PhoneNumber,
        u.DateOfBirth,
        u.Address,
        u.ProfilePicture,
        u.IsActive,
        u.EmailConfirmed,
        u.CreatedDate,
        u.LastLoginDate,
        u.LastLoginIp,
        u.FullName
    FROM Users u
    WHERE u.Id = @UserId AND u.IsDeleted = 0;
    
    SELECT 
        r.Id AS RoleId,
        r.RoleName,
        r.Description
    FROM UserRoles ur
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId 
        AND ur.IsActive = 1 
        AND r.IsActive = 1;
    
    SELECT DISTINCT
        p.Id AS PermissionId,
        p.PermissionName,
        p.Description,
        p.Module
    FROM UserRoles ur
    INNER JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
    INNER JOIN Permissions p ON rp.PermissionId = p.Id
    WHERE ur.UserId = @UserId 
        AND ur.IsActive = 1 
        AND p.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserRoles]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetUserRoles]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id AS RoleId,
        r.RoleName,
        r.Description
    FROM UserRoles ur
    INNER JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId 
    AND ur.IsActive = 1 
    AND r.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserSessions]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetUserSessions]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Token AS SessionToken,
        CreatedDate,
        ExpiryDate,
        IpAddress AS IpAddress,
        CASE 
            WHEN ExpiryDate > GETUTCDATE() AND RevokedDate IS NULL THEN 1
            ELSE 0
        END AS IsActive
    FROM RefreshTokens
    WHERE UserId = @UserId
        AND RevokedDate IS NULL
    ORDER BY CreatedDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserSettings]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetUserSettings]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Theme,
        Language,
        EmailNotifications,
        TwoFactorEnabled,
        CreatedDate,
        UpdatedDate
    FROM UserSettings
    WHERE UserId = @UserId;
    
    -- Return default if not found
    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 
            @UserId AS UserId,
            'light' AS Theme,
            'en' AS Language,
            1 AS EmailNotifications,
            0 AS TwoFactorEnabled,
            GETUTCDATE() AS CreatedDate,
            NULL AS UpdatedDate;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_HardDeleteEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 8. HARD DELETE EMPLOYEE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_HardDeleteEmployee]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM Employees WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertAuditLog]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored Procedure to Insert Audit Log
CREATE PROCEDURE [dbo].[sp_InsertAuditLog]
    @UserId INT = NULL,
    @Username NVARCHAR(100) = NULL,
    @Action NVARCHAR(100),
    @EntityName NVARCHAR(100) = NULL,
    @EntityId INT = NULL,
    @OldValues NVARCHAR(MAX) = NULL,
    @NewValues NVARCHAR(MAX) = NULL,
    @IpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO AuditLogs (UserId, Username, Action, EntityName, EntityId, OldValues, NewValues, IpAddress, UserAgent)
    VALUES (@UserId, @Username, @Action, @EntityName, @EntityId, @OldValues, @NewValues, @IpAddress, @UserAgent);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertBulkEmailQueue]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 2. SP: Bulk Entries Insert karna (Jab Admin "Send All" click kare)
CREATE PROCEDURE [dbo].[sp_InsertBulkEmailQueue]
    @PayrollCycleId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Sirf unhi employees ko queue me dalo jinka email valid hai aur payroll process ho chuka hai
        INSERT INTO PayrollEmailQueue (PayrollCycleId, EmployeeId, EmailAddress, EmployeeName, Month, Year, Status)
        SELECT 
            @PayrollCycleId, 
            e.Id, -- Assuming EmployeeId is Id in your Employee table
            e.Email, 
            e.Name, 
            @Month, 
            @Year, 
            'Pending'
        FROM Employee e
        INNER JOIN PayrollProcessing pp ON e.Id = pp.EmployeeId
        WHERE pp.PayrollCycleId = @PayrollCycleId 
          AND e.Email IS NOT NULL AND e.Email != ''
          AND e.IsActive = 1
          -- Duplicate check: agar pehle se queue me hai toh mat dalo
          AND NOT EXISTS (
              SELECT 1 FROM PayrollEmailQueue q 
              WHERE q.PayrollCycleId = @PayrollCycleId AND q.EmployeeId = e.Id
          );

        SELECT @@ROWCOUNT AS InsertedCount;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertBulkSalarySlipEmails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertBulkSalarySlipEmails]
    @PayrollProcessId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Sirf unhi employees ko queue me dalo jinka email valid hai aur payroll process ho chuka hai
        INSERT INTO PayrollEmailQueue (PayrollProcessId, EmployeeId, EmailAddress, EmployeeName, Month, Year, Status)
        SELECT 
            @PayrollProcessId, 
            e.EmployeeId, 
            e.Email, 
            e.FullName, 
            @Month, 
            @Year, 
            'Pending'
        FROM EmployeeMaster e
        INNER JOIN PayrollProcessDetail pd ON e.EmployeeId = pd.EmployeeId
        WHERE pd.PayrollProcessId = @PayrollProcessId 
          AND e.Email IS NOT NULL 
          AND e.Email != ''
          AND e.IsActive = 1
          -- Check if already in queue for this process
          AND NOT EXISTS (
              SELECT 1 FROM PayrollEmailQueue q 
              WHERE q.PayrollProcessId = @PayrollProcessId AND q.EmployeeId = e.EmployeeId
          );

        SELECT @@ROWCOUNT AS InsertedCount;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_LockPayrollCycle]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 5. Lock Payroll Cycle
-- =============================================
CREATE   PROCEDURE [dbo].[sp_LockPayrollCycle]
    @CycleId INT,
    @LockedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PayrollCycle
    SET IsLocked = 1,
        LockedBy = @LockedBy,
        LockedDate = GETDATE(),
        UpdatedBy = @LockedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @CycleId AND IsLocked = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_LogAudit]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_LogAudit]
    @UserId INT = NULL,
    @Action NVARCHAR(100),
    @TableName NVARCHAR(100) = NULL,
    @RecordId INT = NULL,
    @OldValues NVARCHAR(MAX) = NULL,
    @NewValues NVARCHAR(MAX) = NULL,
    @IpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate required parameters
        IF @Action IS NULL OR LTRIM(RTRIM(@Action)) = ''
        BEGIN
            RAISERROR('Action parameter is required', 16, 1);
            RETURN;
        END
        
        -- Validate UserId exists if provided
        IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId)
        BEGIN
            RAISERROR('Invalid UserId', 16, 1);
            RETURN;
        END
        
        -- Insert audit log
        INSERT INTO AuditLogs (
            UserId,
            Action,
            EntityName,
            EntityId,
            OldValues,
            NewValues,
            IpAddress,
            UserAgent,
            CreatedDate
        )
        VALUES (
            @UserId,
            @Action,
            @TableName,
            @RecordId,
            @OldValues,
            @NewValues,
            @IpAddress,
            @UserAgent,
            GETDATE()
        );
        
        DECLARE @AuditLogId BIGINT = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        -- Return the newly created audit log ID
        SELECT @AuditLogId AS AuditLogId;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_LoginUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_LoginUser]  
    @UsernameOrEmail NVARCHAR(255)  
AS  
BEGIN  
    SET NOCOUNT ON;  
      
    SELECT   
        Id,  
        Username,  
        Email,  
        PasswordHash,  
        PasswordSalt,  
        FirstName,  
        LastName,  
        PhoneNumber,  
        ProfilePicture,
        IsActive,  
        IsDeleted,  
        EmailConfirmed,  
        LastLoginDate,  
        FailedLoginAttempts,  
        LockoutEndDate  
    FROM Users
    WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail)  -- ✅ Use parameter
        AND IsDeleted = 0;  
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkAttendance]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_MarkAttendance]
    @StudentId INT,
    @AttendanceDate DATE,
    @AttendanceTime TIME,
    @Status NVARCHAR(20),
    @CapturedImagePath NVARCHAR(500) = NULL,
    @ConfidenceScore DECIMAL(5,2) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @MarkedBy INT = NULL,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if attendance already marked today
        IF EXISTS (
            SELECT 1 FROM Attendance 
            WHERE StudentId = @StudentId 
            AND AttendanceDate = @AttendanceDate
        )
        BEGIN
            -- Update existing record
            UPDATE Attendance
            SET 
                AttendanceTime = @AttendanceTime,
                Status = @Status,
                CapturedImagePath = @CapturedImagePath,
                ConfidenceScore = @ConfidenceScore,
                Remarks = @Remarks,
                MarkedBy = @MarkedBy
            WHERE StudentId = @StudentId 
            AND AttendanceDate = @AttendanceDate;
            
            SELECT @NewId = Id 
            FROM Attendance 
            WHERE StudentId = @StudentId 
            AND AttendanceDate = @AttendanceDate;
        END
        ELSE
        BEGIN
            -- Generate new AttendanceId
            DECLARE @AttendanceId NVARCHAR(50);
            EXEC sp_GenerateNextAttendanceId @AttendanceId OUTPUT;
            
            -- Insert new record
            INSERT INTO Attendance (
                AttendanceId, StudentId, AttendanceDate, AttendanceTime,
                Status, CapturedImagePath, ConfidenceScore, Remarks,
                MarkedBy, CreatedDate
            )
            VALUES (
                @AttendanceId, @StudentId, @AttendanceDate, @AttendanceTime,
                @Status, @CapturedImagePath, @ConfidenceScore, @Remarks,
                @MarkedBy, GETDATE()
            );
            
            SET @NewId = SCOPE_IDENTITY();
        END
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkEMIPaid]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 18. Mark EMI As Paid
-- =============================================
CREATE   PROCEDURE [dbo].[sp_MarkEMIPaid]
(
    @EMIId INT,
    @CycleId INT,
    @AmountPaid DECIMAL(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LoanId INT;
    DECLARE @EMIAmount DECIMAL(18,2);
    DECLARE @PrincipalAmount DECIMAL(18,2);
    DECLARE @InterestAmount DECIMAL(18,2);

    SELECT @LoanId = LoanId,
           @EMIAmount = EMIAmount,
           @PrincipalAmount = PrincipalAmount,
           @InterestAmount = InterestAmount
    FROM LoanEMISchedule WHERE Id = @EMIId;

    SET @AmountPaid = ISNULL(@AmountPaid, @EMIAmount);

    UPDATE LoanEMISchedule 
    SET Status = 'Paid',
        PaymentDate = GETDATE(),
        AmountPaid = @AmountPaid,
        PayrollCycleId = @CycleId,
        UpdatedDate = GETDATE()
    WHERE Id = @EMIId AND Status = 'Pending';

    -- Update loan
    UPDATE EmployeeLoans 
    SET TotalAmountPaid = TotalAmountPaid + @AmountPaid,
        PrincipalPaid = PrincipalPaid + @PrincipalAmount,
        InterestPaid = InterestPaid + @InterestAmount,
        OutstandingAmount = OutstandingAmount - @AmountPaid,
        OutstandingPrincipal = OutstandingPrincipal - @PrincipalAmount,
        OutstandingInterest = OutstandingInterest - @InterestAmount,
        TotalEMIsPaid = TotalEMIsPaid + 1,
        LastEMIPaidDate = GETDATE(),
        UpdatedDate = GETDATE()
    WHERE Id = @LoanId;

    -- Check if fully paid
    UPDATE EmployeeLoans 
    SET IsFullyPaid = 1, ClosureDate = GETDATE(), Status = 'Closed'
    WHERE Id = @LoanId AND TotalEMIsPaid >= TenureMonths;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkPasswordResetTokenUsed]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 19. sp_MarkPasswordResetTokenUsed
-- =============================================
CREATE   PROCEDURE [dbo].[sp_MarkPasswordResetTokenUsed]
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PasswordResetTokens
    SET 
        IsUsed = 1,
        UsedDate = GETDATE()
    WHERE Token = @Token;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkPayrollAsPaid]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 13. Mark Payroll As Paid
-- =============================================
CREATE   PROCEDURE [dbo].[sp_MarkPayrollAsPaid]
(
    @ProcessingId INT,
    @PaymentMode NVARCHAR(50),
    @ReferenceNo NVARCHAR(100),
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PayrollProcessing
    SET PaymentStatus = 'Paid',
        PaymentDate = GETDATE(),
        PaymentMode = @PaymentMode,
        PaymentReferenceNo = @ReferenceNo,
        Status = 'Paid',
        UpdatedBy = @UserId,
        UpdatedDate = GETDATE()
    WHERE Id = @ProcessingId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkRefreshTokenAsUsed]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 11. sp_MarkRefreshTokenAsUsed
-- =============================================
CREATE   PROCEDURE [dbo].[sp_MarkRefreshTokenAsUsed]
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET 
        IsUsed = 1,
        UsedDate = GETDATE()
    WHERE Token = @Token;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PrepayLoan]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- LOAN PREPAYMENT & CLOSURE
-- =============================================

-- 19. Prepay Loan
-- =============================================
CREATE   PROCEDURE [dbo].[sp_PrepayLoan]
(
    @LoanId INT,
    @PrepaymentAmount DECIMAL(18,2),
    @PrepaymentType NVARCHAR(50), -- 'Partial' or 'Full'
    @PaymentMode NVARCHAR(50),
    @ReferenceNo NVARCHAR(100) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @OutstandingPrincipal DECIMAL(18,2);
        DECLARE @OutstandingInterest DECIMAL(18,2);
        DECLARE @OutstandingAmount DECIMAL(18,2);
        DECLARE @EmployeeId INT;

        -- Get current loan status
        SELECT @OutstandingPrincipal = OutstandingPrincipal,
               @OutstandingInterest = OutstandingInterest,
               @OutstandingAmount = OutstandingAmount,
               @EmployeeId = EmployeeId
        FROM EmployeeLoans 
        WHERE Id = @LoanId AND Status = 'Active';

        IF @OutstandingAmount IS NULL
        BEGIN
            RAISERROR('Loan not found or not active', 16, 1);
            RETURN;
        END

        -- For full prepayment, use outstanding principal (no future interest)
        IF @PrepaymentType = 'Full'
        BEGIN
            SET @PrepaymentAmount = @OutstandingPrincipal;
        END

        -- Insert prepayment record
        INSERT INTO LoanPrepayments
        (
            LoanId, EmployeeId, PrepaymentDate, PrepaymentAmount,
            PrepaymentType, PaymentMode, ReferenceNo, Remarks,
            OutstandingBeforePrepay, OutstandingAfterPrepay,
            CreatedBy, CreatedDate
        )
        VALUES
        (
            @LoanId, @EmployeeId, GETDATE(), @PrepaymentAmount,
            @PrepaymentType, @PaymentMode, @ReferenceNo, @Remarks,
            @OutstandingPrincipal, @OutstandingPrincipal - @PrepaymentAmount,
            @UserId, GETDATE()
        );

        -- Update loan
        UPDATE EmployeeLoans 
        SET TotalAmountPaid = TotalAmountPaid + @PrepaymentAmount,
            PrincipalPaid = PrincipalPaid + @PrepaymentAmount,
            OutstandingAmount = OutstandingAmount - @PrepaymentAmount,
            OutstandingPrincipal = OutstandingPrincipal - @PrepaymentAmount,
            PrepaymentAmount = ISNULL(PrepaymentAmount, 0) + @PrepaymentAmount,
            LastPrepaymentDate = GETDATE(),
            UpdatedBy = @UserId,
            UpdatedDate = GETDATE()
        WHERE Id = @LoanId;

        -- If full prepayment, close loan
        IF @PrepaymentType = 'Full' OR @OutstandingPrincipal - @PrepaymentAmount <= 0
        BEGIN
            UPDATE EmployeeLoans 
            SET IsFullyPaid = 1,
                ClosureDate = GETDATE(),
                ClosureType = 'Prepayment',
                Status = 'Closed'
            WHERE Id = @LoanId;

            -- Cancel remaining EMIs
            UPDATE LoanEMISchedule 
            SET Status = 'Cancelled',
                UpdatedDate = GETDATE()
            WHERE LoanId = @LoanId AND Status = 'Pending';
        END
        ELSE
        BEGIN
            -- Regenerate EMI schedule for partial prepayment
            -- This is simplified - actual logic may vary based on prepayment rules
            DECLARE @RemainingPrincipal DECIMAL(18,2) = @OutstandingPrincipal - @PrepaymentAmount;
            DECLARE @RemainingEMIs INT;
            
            SELECT @RemainingEMIs = COUNT(*) 
            FROM LoanEMISchedule 
            WHERE LoanId = @LoanId AND Status = 'Pending';

            IF @RemainingEMIs > 0
            BEGIN
                -- Update remaining EMI amounts (simplified - equal distribution)
                DECLARE @NewEMIAmount DECIMAL(18,2) = ROUND(@RemainingPrincipal / @RemainingEMIs, 2);
                
                UPDATE LoanEMISchedule 
                SET PrincipalAmount = @NewEMIAmount,
                    EMIAmount = @NewEMIAmount + InterestAmount,
                    UpdatedDate = GETDATE()
                WHERE LoanId = @LoanId AND Status = 'Pending';
            END
        END

        COMMIT TRANSACTION;

        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessBulkPayroll]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: sp_ProcessBulkPayroll
-- Description: Entire payroll cycle साठी सर्व employees चा payroll process करणे
-- Parameters: 
--   @PayrollCycleId - Payroll cycle ID
--   @ProcessedBy - User ID
-- =============================================
CREATE   PROCEDURE [dbo].[sp_ProcessBulkPayroll]
    @PayrollCycleId INT,
    @ProcessedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TotalEmployees INT = 0;
        DECLARE @ProcessedCount INT = 0;
        DECLARE @TotalGross DECIMAL(18,2) = 0;
        DECLARE @TotalDeductions DECIMAL(18,2) = 0;
        DECLARE @TotalNet DECIMAL(18,2) = 0;
        DECLARE @StartDate DATE, @EndDate DATE;
        DECLARE @TotalWorkingDays INT;

        -- Get cycle details
        SELECT @StartDate = StartDate, @EndDate = EndDate
        FROM PayrollCycle WHERE Id = @PayrollCycleId;

        -- Calculate working days (excluding Sundays)
        SET @TotalWorkingDays = DATEDIFF(DAY, @StartDate, @EndDate) + 1 
            - (DATEDIFF(WEEK, @StartDate, @EndDate) * 1); -- Subtract Sundays

        -- Get all active employees
        SELECT @TotalEmployees = COUNT(*)
        FROM Employees 
        WHERE IsActive = 1 AND IsDeleted = 0;

        -- Process each employee
        DECLARE @EmployeeId INT;
        DECLARE @PresentDays DECIMAL(5,2);

        DECLARE emp_cursor CURSOR FOR
        SELECT e.Id,
               ISNULL((SELECT COUNT(*) FROM Attendance a 
                       WHERE a.StudentId = e.Id 
                         AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
                         AND a.Status = 'Present'), 0) AS PresentDays
        FROM Employees e
        WHERE e.IsActive = 1 AND e.IsDeleted = 0
          AND EXISTS (SELECT 1 FROM EmployeeSalaryStructure ess 
                      WHERE ess.EmployeeId = e.Id AND ess.IsCurrentStructure = 1);

        OPEN emp_cursor;
        FETCH NEXT FROM emp_cursor INTO @EmployeeId, @PresentDays;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            BEGIN TRY
                -- Calculate payroll for this employee
                EXEC sp_CalculateEmployeePayroll 
                    @PayrollCycleId = @PayrollCycleId,
                    @EmployeeId = @EmployeeId,
                    @TotalWorkingDays = @TotalWorkingDays,
                    @PresentDays = @PresentDays,
                    @PaidLeaveDays = 0,
                    @WeeklyOffDays = 4,
                    @HolidayDays = 0,
                    @OvertimeHours = 0,
                    @CalculatedBy = @ProcessedBy;

                SET @ProcessedCount = @ProcessedCount + 1;
            END TRY
            BEGIN CATCH
                -- Log error but continue processing
                PRINT 'Error processing employee ' + CAST(@EmployeeId AS NVARCHAR(10)) + ': ' + ERROR_MESSAGE();
            END CATCH

            FETCH NEXT FROM emp_cursor INTO @EmployeeId, @PresentDays;
        END

        CLOSE emp_cursor;
        DEALLOCATE emp_cursor;

        -- Calculate totals
        SELECT 
            @TotalGross = ISNULL(SUM(GrossSalary), 0),
            @TotalDeductions = ISNULL(SUM(TotalDeductions), 0),
            @TotalNet = ISNULL(SUM(NetSalary), 0)
        FROM PayrollProcessing
        WHERE PayrollCycleId = @PayrollCycleId;

        -- Update cycle summary
        UPDATE PayrollCycle
        SET TotalEmployees = @TotalEmployees,
            ProcessedEmployees = @ProcessedCount,
            TotalGrossSalary = @TotalGross,
            TotalDeductions = @TotalDeductions,
            TotalNetSalary = @TotalNet,
            Status = 'Processed',
            ProcessingDate = GETDATE(),
            UpdatedBy = @ProcessedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @PayrollCycleId;

        COMMIT TRANSACTION;

        -- Return summary
        SELECT 
            @PayrollCycleId AS PayrollCycleId,
            @TotalEmployees AS TotalEmployees,
            @ProcessedCount AS ProcessedEmployees,
            @TotalGross AS TotalGrossSalary,
            @TotalDeductions AS TotalDeductions,
            @TotalNet AS TotalNetSalary;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessLoanEMI]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 17. Process Loan EMI (Called from Payroll)
-- =============================================
CREATE   PROCEDURE [dbo].[sp_ProcessLoanEMI]
(
    @PayrollCycleId INT,
    @LoanId INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @EMIId INT;
        DECLARE @EMIAmount DECIMAL(18,2);
        DECLARE @PrincipalAmount DECIMAL(18,2);
        DECLARE @InterestAmount DECIMAL(18,2);
        DECLARE @EmployeeId INT;

        -- Get next pending EMI
        SELECT TOP 1 
            @EMIId = les.Id,
            @EMIAmount = les.EMIAmount,
            @PrincipalAmount = les.PrincipalAmount,
            @InterestAmount = les.InterestAmount,
            @EmployeeId = el.EmployeeId
        FROM LoanEMISchedule les
        INNER JOIN EmployeeLoans el ON les.LoanId = el.Id
        WHERE les.LoanId = @LoanId AND les.Status = 'Pending'
        ORDER BY les.EMINumber;

        IF @EMIId IS NULL
        BEGIN
            RAISERROR('No pending EMI found', 16, 1);
            RETURN;
        END

        -- Mark EMI as paid
        UPDATE LoanEMISchedule 
        SET Status = 'Paid',
            PaymentDate = GETDATE(),
            AmountPaid = EMIAmount,
            PayrollCycleId = @PayrollCycleId,
            UpdatedDate = GETDATE()
        WHERE Id = @EMIId;

        -- Update loan outstanding
        UPDATE EmployeeLoans 
        SET TotalAmountPaid = TotalAmountPaid + @EMIAmount,
            PrincipalPaid = PrincipalPaid + @PrincipalAmount,
            InterestPaid = InterestPaid + @InterestAmount,
            OutstandingAmount = OutstandingAmount - @EMIAmount,
            OutstandingPrincipal = OutstandingPrincipal - @PrincipalAmount,
            OutstandingInterest = OutstandingInterest - @InterestAmount,
            TotalEMIsPaid = TotalEMIsPaid + 1,
            LastEMIPaidDate = GETDATE(),
            UpdatedDate = GETDATE()
        WHERE Id = @LoanId;

        -- Check if loan is fully paid
        UPDATE EmployeeLoans 
        SET IsFullyPaid = 1,
            ClosureDate = GETDATE(),
            Status = 'Closed'
        WHERE Id = @LoanId 
          AND TotalEMIsPaid >= TenureMonths;

        COMMIT TRANSACTION;

        SELECT 1 AS Success, @EMIId AS EMIId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessPayrollCycle]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_ProcessPayrollCycle]
    @CycleId INT,
    @Month INT,
    @Year INT,
    @ProcessedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Active employees fetch karna jo process hone hain
        DECLARE @EmpTable TABLE (EmpId INT, StructureId INT, Basic DECIMAL(18,2));
        
        INSERT INTO @EmpTable (EmpId, StructureId, Basic)
        SELECT 
            e.Id, 
            ess.Id as StructureId, 
            ess.BasicSalary
        FROM Employees e
        JOIN EmployeeSalaryStructure ess ON e.Id = ess.EmployeeId
        WHERE e.IsActive = 1 
          AND e.IsDeleted = 0 
          AND ess.IsCurrentStructure = 1
          AND NOT EXISTS (SELECT 1 FROM PayrollProcessing pp WHERE pp.PayrollCycleId = @CycleId AND pp.EmployeeId = e.Id);

        -- 2. Har employee ke liye calculate karke insert karna
        INSERT INTO PayrollProcessing (
            PayrollCycleId, EmployeeId, EmployeeSalaryStructureId, 
            BasicSalary, GrossSalary, NetSalary, TotalWorkingDays, 
            PresentDays, AbsentDays, PaidLeaveDays, LOPDays, LOPAmount,
            Status, CreatedDate, CreatedBy
        )
        SELECT 
            @CycleId, 
            et.EmpId,
            et.StructureId,
            et.Basic,
            et.Basic * 2.5, -- Example: Gross calculation logic (Replace with actual formula)
            et.Basic * 2.2, -- Example: Net calculation logic
            30, -- Default working days
            30, -- Present (Placeholder, Attendance join karna padega real scenario me)
            0, 0, 0, 0,
            'Draft',
            GETDATE(),
            @ProcessedBy
        FROM @EmpTable et;

        -- 3. Cycle Update
        UPDATE PayrollCycle 
        SET Status = 'Generated', ProcessingDate = GETDATE(), ProcessedEmployees = (SELECT COUNT(*) FROM @EmpTable)
        WHERE Id = @CycleId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_QueueBulkEmails]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_QueueBulkEmails]
    @CycleId INT,
    @SubjectTemplate NVARCHAR(255),
    @BodyTemplate NVARCHAR(MAX),
    @TriggeredBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @CompanyName NVARCHAR(200);

        SELECT TOP 1 
            @CompanyName = CompanyName 
        FROM CompanyMaster 
        WHERE IsActive = 1;

        INSERT INTO PayrollEmailQueue 
        (
            CycleId, 
            SalarySlipId, 
            EmployeeId, 
            EmployeeEmail, 
            [Subject], 
            BodyHtml, 
            Status, 
            CreatedDate,
            CreatedBy
        )
        SELECT 
            @CycleId,
            ss.Id,
            e.Id,
            e.Email,
            REPLACE(
                @SubjectTemplate, 
                '{Month}', 
                DATENAME(MONTH, DATEFROMPARTS(YEAR(GETDATE()), ss.[Month], 1))
            ),
            REPLACE(
                REPLACE(@BodyTemplate, '{EmpName}', e.Name), 
                '{Company}', 
                ISNULL(@CompanyName, '')
            ),
            'Pending',
            GETDATE(),
            @TriggeredBy
        FROM SalarySlips ss
        INNER JOIN Employees e 
            ON ss.EmployeeId = e.Id
        WHERE ss.PayrollCycleId = @CycleId
          AND ss.Status = 'Generated'
          AND ISNULL(e.Email, '') <> ''
          AND ISNULL(ss.EmailStatus, '') <> 'Sent'
          AND NOT EXISTS
          (
              SELECT 1
              FROM PayrollEmailQueue q
              WHERE q.SalarySlipId = ss.Id
                AND q.EmployeeId = e.Id
                AND q.Status IN ('Pending', 'Sent')
          );

        SELECT COUNT(*) AS QueuedCount
        FROM PayrollEmailQueue
        WHERE CycleId = @CycleId
          AND Status = 'Pending';
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_RegisterUser]
    @Username NVARCHAR(100),
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(500),
    @PasswordSalt NVARCHAR(500),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @PhoneNumber NVARCHAR(20) = NULL,
    @RoleId INT = NULL,
    @CreatedBy INT = NULL,
    @UserId INT OUTPUT,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IsSelfRegistration BIT = CASE WHEN @CreatedBy IS NULL THEN 1 ELSE 0 END;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.Users WITH (UPDLOCK, HOLDLOCK) WHERE Username = @Username)
        BEGIN
            SET @UserId = 0;
            SET @Message = 'Username already exists';
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF EXISTS (SELECT 1 FROM dbo.Users WITH (UPDLOCK, HOLDLOCK) WHERE Email = @Email)
        BEGIN
            SET @UserId = 0;
            SET @Message = 'Email already exists';
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        IF @IsSelfRegistration = 0
        BEGIN
            IF @RoleId IS NULL
            BEGIN
                SET @UserId = 0;
                SET @Message = 'Role is required when an admin creates a user';
                ROLLBACK TRANSACTION;
                RETURN;
            END;

            IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId AND ISNULL(IsActive, 1) = 1)
            BEGIN
                SET @UserId = 0;
                SET @Message = 'Selected role does not exist or is inactive';
                ROLLBACK TRANSACTION;
                RETURN;
            END;
        END;

        INSERT INTO dbo.Users
        (
            Username,
            Email,
            PasswordHash,
            PasswordSalt,
            FirstName,
            LastName,
            PhoneNumber,
            FullName,
            IsActive,
            RegistrationStatus,
            ApprovedBy,
            ApprovedDate,
            CreatedBy
        )
        VALUES
        (
            @Username,
            @Email,
            @PasswordHash,
            @PasswordSalt,
            @FirstName,
            @LastName,
            @PhoneNumber,
            LEFT(CONCAT(@FirstName, ' ', @LastName), 100),
            CASE WHEN @IsSelfRegistration = 1 THEN 0 ELSE 1 END,
            CASE WHEN @IsSelfRegistration = 1 THEN 'Pending' ELSE 'Approved' END,
            CASE WHEN @IsSelfRegistration = 1 THEN NULL ELSE @CreatedBy END,
            CASE WHEN @IsSelfRegistration = 1 THEN NULL ELSE GETDATE() END,
            @CreatedBy
        );

        SET @UserId = CONVERT(INT, SCOPE_IDENTITY());

        IF @IsSelfRegistration = 0
        BEGIN
            INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedBy, IsActive)
            VALUES (@UserId, @RoleId, @CreatedBy, 1);

            SET @Message = 'User registered and role assigned successfully';
        END
        ELSE
        BEGIN
            SET @Message = 'Registration submitted successfully. Pending admin approval.';
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @UserId = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_RejectLeave]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_RejectLeave]
    @LeaveRequestId INT,
    @RejectedBy     INT,
    @Remarks        NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @EmployeeId INT, @LeaveTypeId INT, @TotalDays DECIMAL(5,1), @StartDate DATE;

        SELECT @EmployeeId = EmployeeId,
               @LeaveTypeId = LeaveTypeId,
               @TotalDays = TotalDays,
               @StartDate = StartDate
        FROM LeaveRequests
        WHERE Id = @LeaveRequestId AND IsDeleted = 0;

        IF @EmployeeId IS NULL
        BEGIN
            RAISERROR('Leave request not found.', 16, 1);
            RETURN;
        END

        -- Update leave request status
        UPDATE LeaveRequests
        SET Status = 'Rejected',
            RejectedBy = @RejectedBy,
            RejectedDate = GETDATE(),
            Remarks = @Remarks,
            UpdatedBy = @RejectedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @LeaveRequestId;

        -- Release pending balance
        UPDATE LeaveBalances
        SET TotalPending = TotalPending - @TotalDays,
            UpdatedBy = @RejectedBy,
            UpdatedDate = GETDATE()
        WHERE EmployeeId = @EmployeeId
          AND LeaveTypeId = @LeaveTypeId
          AND Year = YEAR(@StartDate);

        -- Insert approval record
        INSERT INTO LeaveApprovals (LeaveRequestId, ApproverLevel, ApproverId, ApproverRole, Status, Comments, ActionDate)
        VALUES (@LeaveRequestId, 1, @RejectedBy, 'Manager', 'Rejected', @Remarks, GETDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RejectLoan]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 11. Reject Loan
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RejectLoan]
(
    @LoanId INT,
    @Reason NVARCHAR(500),
    @RejectedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE EmployeeLoans 
    SET Status = 'Rejected',
        RejectedBy = @RejectedBy,
        RejectedDate = GETDATE(),
        RejectionReason = @Reason,
        UpdatedBy = @RejectedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @LoanId AND Status = 'Pending';

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RejectUserRegistration]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- ✅ FIX: Reject User
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RejectUserRegistration]
    @UserId         INT,
    @RejectedBy     INT,
    @RejectionReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET RegistrationStatus = 'Rejected',
        IsActive = 0,
        RejectionReason = @RejectionReason,
        ApprovedBy = @RejectedBy,
        ApprovedDate = GETDATE(),
        UpdatedBy = @RejectedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @UserId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveRoleFromUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   14. REMOVE ROLE FROM USER
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_RemoveRoleFromUser]
    @UserId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE UserRoles
        SET IsActive = 0
        WHERE UserId = @UserId
          AND RoleId = @RoleId
          AND IsActive = 1;

        SELECT 1 AS Success, 'Role removed successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveTemplateComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 16. Remove Template Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RemoveTemplateComponent]
    @TemplateComponentId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SalaryTemplateComponents
    SET IsActive = 0
    WHERE Id = @TemplateComponentId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ResetUserPassword]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   7. RESET USER PASSWORD
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_ResetUserPassword]
    @UserId INT,
    @NewPasswordHash NVARCHAR(500),
    @ResetBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET 
        PasswordHash = @NewPasswordHash,
        UpdatedBy = @ResetBy,
        UpdatedDate = GETDATE()
    WHERE Id = @UserId;

    SELECT 1 AS Success, 'Password reset successfully' AS Message;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RestoreEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 9. RESTORE DELETED EMPLOYEE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RestoreEmployee]
    @Id INT,
    @RestoredBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees
    SET 
        IsDeleted = 0,
        IsActive = 1,
        DeletedBy = NULL,
        DeletedDate = NULL,
        UpdatedBy = @RestoredBy,
        UpdatedDate = GETDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RevokeAllUserTokens]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 12. sp_RevokeAllUserTokens
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RevokeAllUserTokens]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET 
        IsRevoked = 1,
        RevokedDate = GETDATE()
    WHERE UserId = @UserId 
    AND IsRevoked = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RevokeRefreshToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 10. sp_RevokeRefreshToken
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RevokeRefreshToken]
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET 
        IsRevoked = 1,
        RevokedDate = GETDATE()
    WHERE Token = @Token;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RevokeSalaryStructure]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 25. Revoke Salary Structure
-- =============================================
CREATE   PROCEDURE [dbo].[sp_RevokeSalaryStructure]
(
    @StructureId INT,
    @RevokedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @EmployeeId INT;
        DECLARE @PreviousStructureId INT;

        -- Get employee and previous structure
        SELECT @EmployeeId = EmployeeId, @PreviousStructureId = PreviousStructureId
        FROM EmployeeSalaryStructure 
        WHERE Id = @StructureId;

        -- Mark current structure as inactive
        UPDATE EmployeeSalaryStructure 
        SET IsCurrentStructure = 0,
            IsActive = 0,
            Status = 'Revoked',
            UpdatedBy = @RevokedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @StructureId;

        -- Restore previous structure if exists
        IF @PreviousStructureId IS NOT NULL
        BEGIN
            UPDATE EmployeeSalaryStructure 
            SET IsCurrentStructure = 1,
                IsActive = 1,
                EffectiveTo = NULL,
                UpdatedBy = @RevokedBy,
                UpdatedDate = GETDATE()
            WHERE Id = @PreviousStructureId;
        END

        COMMIT TRANSACTION;

        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RevokeUserSession]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_RevokeUserSession]
    @UserId INT,
    @SessionToken NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE RefreshTokens
        SET 
            RevokedDate = GETUTCDATE(),
            UsedDate = 'Revoked by user'
        WHERE UserId = @UserId 
            AND Token = @SessionToken
            AND RevokedDate IS NULL;
        
        IF @@ROWCOUNT > 0
        BEGIN
            SELECT 1 AS Success, 'Session revoked successfully' AS Message;
        END
        ELSE
        BEGIN
            SELECT 0 AS Success, 'Session not found' AS Message;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SavePasswordResetToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SavePasswordResetToken]  
    @UserId INT,  
    @Token NVARCHAR(500),  
    @ExpiryDate DATETIME2,
    @IpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    DELETE FROM PasswordResetTokens WHERE UserId = @UserId;  
  
    INSERT INTO PasswordResetTokens (  
        UserId,  
        Token,  
        ExpiryDate,  
        IpAddress,
        UserAgent,
        CreatedDate,  
        IsUsed  
    )  
    VALUES (  
        @UserId,  
        @Token,  
        @ExpiryDate,  
        @IpAddress,
        @UserAgent,
        GETUTCDATE(),  -- ✅ Changed to GETUTCDATE()
        0  
    );  
  
    SELECT SCOPE_IDENTITY() AS TokenId;  
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SaveRefreshToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 8. sp_SaveRefreshToken
-- =============================================
CREATE   PROCEDURE [dbo].[sp_SaveRefreshToken]
    @UserId INT,
    @Token NVARCHAR(500),
    @ExpiryDate DATETIME2,
    @CreatedByIp NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RefreshTokens (
        UserId,
        Token,
        ExpiryDate,
        CreatedDate,
        IpAddress,
        IsRevoked,
        IsUsed
    )
    VALUES (
        @UserId,
        @Token,
        @ExpiryDate,
        GETDATE(),
        @CreatedByIp,
        0,
        0
    );

    SELECT SCOPE_IDENTITY() AS TokenId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchEmployees]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 10. SEARCH EMPLOYEES
-- =============================================
CREATE   PROCEDURE [dbo].[sp_SearchEmployees]
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Salary,
        e.PhoneNumber,
        e.Address,
        e.DateOfBirth,
        e.JoiningDate,
        e.ProfileImagePath,
        e.Role,
        e.IsActive,
        e.IsDeleted,
        e.CreatedBy,
        e.CreatedDate,
        e.UpdatedBy,
        e.UpdatedDate,
        e.DeletedBy,
        e.DeletedDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.IsDeleted = 0
    AND (
        e.Name LIKE '%' + @SearchTerm + '%'
        OR e.Email LIKE '%' + @SearchTerm + '%'
        OR e.PhoneNumber LIKE '%' + @SearchTerm + '%'
        OR d.DepartmentName LIKE '%' + @SearchTerm + '%'
        OR e.Role LIKE '%' + @SearchTerm + '%'
    )
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SearchStudents]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SearchStudents]
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        StudentId,
        FirstName,
        LastName,
        FullName,
        Class,
        Subjects,
        Age,
        JoiningDate,
        BatchTime,
        PassportPhotoPath,
        PhoneNumber,
        Email,
        IsActive
    FROM Students
    WHERE IsDeleted = 0
    AND (
        FullName LIKE '%' + @SearchTerm + '%'
        OR StudentId LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%'
        OR PhoneNumber LIKE '%' + @SearchTerm + '%'
        OR Class LIKE '%' + @SearchTerm + '%'
    )
    ORDER BY FullName;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ToggleUserStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   6. TOGGLE USER STATUS
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_ToggleUserStatus]
    @UserId INT,
    @IsActive BIT,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET 
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @UserId;

    SELECT 
        1 AS Success,
        CASE 
            WHEN @IsActive = 1 THEN 'User activated successfully'
            ELSE 'User deactivated successfully'
        END AS Message;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmailQueueStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateEmailQueueStatus]
    @QueueId BIGINT,
    @Status VARCHAR(20), -- 'Sent' or 'Failed'
    @ErrorMessage VARCHAR(MAX) = NULL,
    @PdfFilePath VARCHAR(500) = NULL
AS
BEGIN
    UPDATE PayrollEmailQueue
    SET Status = @Status,
        ErrorMessage = @ErrorMessage,
        PdfFilePath = @PdfFilePath,
        RetryCount = RetryCount + 1,
        SentDate = CASE WHEN @Status = 'Sent' THEN GETDATE() ELSE SentDate END,
        UpdatedDate = GETDATE()
    WHERE QueueId = @QueueId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmployee]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 6. UPDATE EMPLOYEE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateEmployee]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(255),
    @DepartmentId INT,
    @Salary DECIMAL(18,2),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Address NVARCHAR(500) = NULL,
    @Role NVARCHAR(50) = NULL,
    @ProfileImagePath NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @IsDeleted BIT = 0,
    @UpdatedBy INT = NULL,
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check for duplicate email (excluding current record)
        IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND Id <> @Id AND IsDeleted = 0)
        BEGIN
            RAISERROR('Email already exists', 16, 1);
            RETURN;
        END
        
        UPDATE Employees
        SET 
            Name = @Name,
            Email = @Email,
            DepartmentId = @DepartmentId,
            Salary = @Salary,
            PhoneNumber = @PhoneNumber,
            Address = @Address,
            Role = @Role,
            ProfileImagePath = @ProfileImagePath,
            IsActive = @IsActive,
            IsDeleted = @IsDeleted,
            UpdatedBy = @UpdatedBy,
            UpdatedDate = GETDATE(),
            DeletedBy = CASE WHEN @IsDeleted = 1 THEN @DeletedBy ELSE NULL END,
            DeletedDate = CASE WHEN @IsDeleted = 1 THEN GETDATE() ELSE NULL END
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmployeeComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 23. Update Employee Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateEmployeeComponent]
(
    @ComponentId INT,
    @Amount DECIMAL(18,2),
    @Percentage DECIMAL(5,2) = NULL,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE EmployeeSalaryComponents 
    SET MonthlyAmount = @Amount,
        Amount = @Amount,
        AnnualAmount = @Amount * 12,
        Percentage = @Percentage,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @ComponentId;

    -- Update parent structure totals
    DECLARE @StructureId INT;
    SELECT @StructureId = EmployeeSalaryStructureId FROM EmployeeSalaryComponents WHERE Id = @ComponentId;

    UPDATE EmployeeSalaryStructure
    SET TotalEarnings = (
            SELECT ISNULL(SUM(MonthlyAmount), 0) 
            FROM EmployeeSalaryComponents 
            WHERE EmployeeSalaryStructureId = @StructureId AND ComponentType = 'Earning' AND IsActive = 1
        ),
        TotalDeductions = (
            SELECT ISNULL(SUM(MonthlyAmount), 0) 
            FROM EmployeeSalaryComponents 
            WHERE EmployeeSalaryStructureId = @StructureId AND ComponentType = 'Deduction' AND IsActive = 1
        ),
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @StructureId;

    -- Recalculate Net Salary
    UPDATE EmployeeSalaryStructure
    SET NetSalary = TotalEarnings - TotalDeductions,
        GrossSalary = TotalEarnings
    WHERE Id = @StructureId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmployeeProfileImage]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 23. UPDATE EMPLOYEE PROFILE IMAGE
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateEmployeeProfileImage]
    @Id INT,
    @ProfileImagePath NVARCHAR(500),
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees
    SET 
        ProfileImagePath = @ProfileImagePath,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmployeeStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 22. UPDATE EMPLOYEE STATUS
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateEmployeeStatus]
    @Id INT,
    @IsActive BIT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees
    SET 
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateHoliday]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateHoliday]
    @Id          INT,
    @Name        NVARCHAR(200),
    @Date        DATE,
    @Type        NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @IsActive    BIT = 1,
    @UpdatedBy   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for duplicate date (exclude current record)
    IF EXISTS (
        SELECT 1 FROM Holidays 
        WHERE Date = @Date AND Id != @Id AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Another holiday already exists on this date.', 16, 1);
        RETURN;
    END

    UPDATE Holidays
    SET Name = @Name,
        Date = @Date,
        Day = DATENAME(WEEKDAY, @Date),
        Type = @Type,
        Description = @Description,
        Year = YEAR(@Date),
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateLoanType]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 4. Update Loan Type
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateLoanType]
(
    @LoanTypeId INT,
    @LoanTypeName NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @InterestRate DECIMAL(5,2) = 0,
    @MaxAmount DECIMAL(18,2) = NULL,
    @MinAmount DECIMAL(18,2) = NULL,
    @MaxTenureMonths INT = 60,
    @MinTenureMonths INT = 1,
    @RequiresGuarantor BIT = 0,
    @RequiresCollateral BIT = 0,
    @MaxLoanMultiplier DECIMAL(5,2) = NULL,
    @ProcessingFeePercent DECIMAL(5,2) = 0,
    @DisplayOrder INT = 0,
    @IsActive BIT = 1,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE LoanTypes 
    SET LoanTypeName = @LoanTypeName,
        Description = @Description,
        InterestRate = @InterestRate,
        MaxAmount = @MaxAmount,
        MinAmount = @MinAmount,
        MaxTenureMonths = @MaxTenureMonths,
        MinTenureMonths = @MinTenureMonths,
        RequiresGuarantor = @RequiresGuarantor,
        RequiresCollateral = @RequiresCollateral,
        MaxLoanMultiplier = @MaxLoanMultiplier,
        ProcessingFeePercent = @ProcessingFeePercent,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @LoanTypeId AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateLoginStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateLoginStatus]
    @UserId INT,
    @IsSuccess BIT,
    @IpAddress NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @IsSuccess = 1
    BEGIN
        UPDATE Users 
        SET LastLoginDate = GETDATE(),
            FailedLoginAttempts = 0,
            LockoutEndDate = NULL
        WHERE Id = @UserId;
    END
    ELSE
    BEGIN
        DECLARE @FailedAttempts INT;
        SELECT @FailedAttempts = FailedLoginAttempts FROM Users WHERE Id = @UserId;
        
        UPDATE Users 
        SET FailedLoginAttempts = @FailedAttempts + 1,
            LockoutEndDate = CASE 
                WHEN @FailedAttempts + 1 >= 5 THEN DATEADD(MINUTE, 30, GETDATE())
                ELSE LockoutEndDate 
            END
        WHERE Id = @UserId;
    END
    
    -- Log the attempt
    INSERT INTO AuditLogs (UserId, Action, IpAddress, CreatedDate)
    VALUES (@UserId, CASE WHEN @IsSuccess = 1 THEN 'Login Success' ELSE 'Login Failed' END, @IpAddress, GETDATE());
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateOverdueTickets]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 16: Update Overdue Tickets
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateOverdueTickets]
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Tickets
    SET IsOverdue = 1
    WHERE IsDeleted = 0
        AND DueDate IS NOT NULL
        AND DueDate < GETDATE()
        AND Status NOT IN ('Closed', 'Resolved')
        AND IsOverdue = 0;
    
    SELECT @@ROWCOUNT AS UpdatedCount;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdatePaymentStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdatePaymentStatus]
    @Id INT,
    @PaymentStatus NVARCHAR(50),
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Payments]
    SET [PaymentStatus] = @PaymentStatus,
        [CompletedDate] = CASE WHEN @PaymentStatus = 'COMPLETED' THEN GETUTCDATE() ELSE [CompletedDate] END,
        [ModifiedDate] = GETUTCDATE()
    WHERE [Id] = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateProfilePicture]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateProfilePicture]  
    @UserId INT,  
    @ProfilePicture NVARCHAR(500),  
    @UpdatedBy INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
      
    BEGIN TRY  
        UPDATE Users  
        SET   
            ProfilePicture = @ProfilePicture,  
            UpdatedDate = GETUTCDATE(),  
            UpdatedBy = @UpdatedBy  
        WHERE Id = @UserId AND IsDeleted = 0;  
          
        IF @@ROWCOUNT > 0  
        BEGIN  
            INSERT INTO AuditLogs (UserId, Action, TableName, RecordId, CreatedDate)  
            VALUES (@UpdatedBy, 'Profile Picture Updated', 'Users', @UserId, GETUTCDATE());  
              
            -- ✅ Cast to BIT
            SELECT CAST(1 AS BIT) AS Success, 'Profile picture updated successfully' AS Message;  
        END  
        ELSE  
        BEGIN  
            SELECT CAST(0 AS BIT) AS Success, 'User not found' AS Message;  
        END  
          
    END TRY  
    BEGIN CATCH  
        SELECT CAST(0 AS BIT) AS Success, ERROR_MESSAGE() AS Message;  
    END CATCH  
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateRole]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   10. UPDATE ROLE
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_UpdateRole]
    @RoleId INT,
    @RoleName NVARCHAR(100),
    @RoleDescription NVARCHAR(500) = NULL,
    @IsActive BIT,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 
            FROM Roles 
            WHERE RoleName = @RoleName 
              AND Id <> @RoleId
        )
        BEGIN
            SELECT 0 AS Success, 'Role name already exists' AS Message;
            RETURN;
        END

        UPDATE Roles
        SET 
            RoleName = @RoleName,
            Description = @RoleDescription,
            RoleDescription = @RoleDescription,
            IsActive = @IsActive,
            UpdatedDate = GETDATE()
        WHERE Id = @RoleId;

        SELECT 1 AS Success, 'Role updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSalaryComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 5. Update Salary Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateSalaryComponent]
(
    @ComponentId INT,
    @ComponentName NVARCHAR(100),
    @ComponentType NVARCHAR(50),
    @Category NVARCHAR(50),
    @CalculationType NVARCHAR(50),
    @CalculationBase NVARCHAR(50),
    @DefaultPercentage DECIMAL(5,2) = NULL,
    @DefaultAmount DECIMAL(18,2) = NULL,
    @DisplayOrder INT = 0,
    @IsStatutory BIT = 0,
    @IsTaxable BIT = 1,
    @FormulaExpression NVARCHAR(500) = NULL,
    @MinAmount DECIMAL(18,2) = NULL,
    @MaxAmount DECIMAL(18,2) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SalaryComponents 
    SET ComponentName = @ComponentName,
        ComponentType = @ComponentType,
        Category = @Category,
        CalculationType = @CalculationType,
        CalculationBase = @CalculationBase,
        DefaultPercentage = @DefaultPercentage,
        DefaultAmount = @DefaultAmount,
        DisplayOrder = @DisplayOrder,
        IsStatutory = @IsStatutory,
        IsTaxable = @IsTaxable,
        FormulaExpression = @FormulaExpression,
        MinAmount = @MinAmount,
        MaxAmount = @MaxAmount,
        Description = @Description,
        Remarks = @Remarks,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @ComponentId AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSalaryStructure]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 24. Update Salary Structure
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateSalaryStructure]
(
    @StructureId INT,
    @CTC DECIMAL(18,2),
    @GrossSalary DECIMAL(18,2),
    @NetSalary DECIMAL(18,2),
    @BasicSalary DECIMAL(18,2) = NULL,
    @TotalEarnings DECIMAL(18,2) = NULL,
    @TotalDeductions DECIMAL(18,2) = NULL,
    @EmployerContributions DECIMAL(18,2) = NULL,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE EmployeeSalaryStructure 
    SET CTC = @CTC,
        GrossSalary = @GrossSalary,
        NetSalary = @NetSalary,
        BasicSalary = ISNULL(@BasicSalary, BasicSalary),
        TotalEarnings = ISNULL(@TotalEarnings, TotalEarnings),
        TotalDeductions = ISNULL(@TotalDeductions, TotalDeductions),
        EmployerContributions = ISNULL(@EmployerContributions, EmployerContributions),
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @StructureId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSalaryTemplate]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 12. Update Salary Template
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateSalaryTemplate]
(
    @TemplateId INT,
    @TemplateName NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @DepartmentId INT = NULL,
    @DesignationId INT = NULL,
    @GradeLevel NVARCHAR(50) = NULL,
    @TotalCTC DECIMAL(18,2),
    @GrossSalary DECIMAL(18,2),
    @NetSalary DECIMAL(18,2),
    @TotalEarnings DECIMAL(18,2),
    @TotalDeductions DECIMAL(18,2),
    @EmployerContributions DECIMAL(18,2) = 0,
    @IsActive BIT = 1,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SalaryTemplates 
    SET TemplateName = @TemplateName,
        Description = @Description,
        DepartmentId = @DepartmentId,
        DesignationId = @DesignationId,
        GradeLevel = @GradeLevel,
        TotalCTC = @TotalCTC,
        GrossSalary = @GrossSalary,
        NetSalary = @NetSalary,
        TotalEarnings = @TotalEarnings,
        TotalDeductions = @TotalDeductions,
        EmployerContributions = @EmployerContributions,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @TemplateId AND IsDeleted = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateStudent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateStudent]
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @FullName NVARCHAR(200),
    @Class NVARCHAR(50),
    @Subjects NVARCHAR(500) = NULL,
    @Age INT = NULL,
    @DateOfBirth DATE = NULL,
    @BatchTime NVARCHAR(50) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Address NVARCHAR(500) = NULL,
    @ParentName NVARCHAR(200) = NULL,
    @ParentPhone NVARCHAR(20) = NULL,
    @ParentEmail NVARCHAR(100) = NULL,
    @IsActive BIT = 1,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Students
        SET 
            FirstName = @FirstName,
            LastName = @LastName,
            FullName = @FullName,
            Class = @Class,
            Subjects = @Subjects,
            Age = @Age,
            DateOfBirth = @DateOfBirth,
            BatchTime = @BatchTime,
            PhoneNumber = @PhoneNumber,
            Email = @Email,
            Address = @Address,
            ParentName = @ParentName,
            ParentPhone = @ParentPhone,
            ParentEmail = @ParentEmail,
            IsActive = @IsActive,
            UpdatedBy = @UpdatedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @Id AND IsDeleted = 0;
        
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateTemplateComponent]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 15. Update Template Component
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateTemplateComponent]
(
    @TemplateComponentId INT,
    @CalculationType NVARCHAR(50),
    @CalculationBase NVARCHAR(50) = NULL,
    @Percentage DECIMAL(5,2) = NULL,
    @FixedAmount DECIMAL(18,2) = NULL,
    @MonthlyAmount DECIMAL(18,2),
    @AnnualAmount DECIMAL(18,2),
    @DisplayOrder INT = 0,
    @UpdatedBy INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SalaryTemplateComponents
    SET CalculationType = @CalculationType,
        CalculationBase = @CalculationBase,
        Percentage = @Percentage,
        FixedAmount = @FixedAmount,
        MonthlyAmount = @MonthlyAmount,
        AnnualAmount = @AnnualAmount,
        DisplayOrder = @DisplayOrder,
        UpdatedBy = @UpdatedBy,
        UpdatedDate = GETDATE()
    WHERE Id = @TemplateComponentId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateTicket]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 7: Update Ticket
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateTicket]
    @TicketId INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @TicketType NVARCHAR(50),
    @Priority NVARCHAR(50),
    @DueDate DATETIME = NULL,
    @StepsToReproduce NVARCHAR(MAX) = NULL,
    @ExpectedResult NVARCHAR(MAX) = NULL,
    @ActualResult NVARCHAR(MAX) = NULL,
    @Environment NVARCHAR(200) = NULL,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Tickets
        SET 
            Title = @Title,
            Description = @Description,
            TicketType = @TicketType,
            Priority = @Priority,
            DueDate = @DueDate,
            StepsToReproduce = @StepsToReproduce,
            ExpectedResult = @ExpectedResult,
            ActualResult = @ActualResult,
            Environment = @Environment,
            UpdatedDate = GETDATE()
        WHERE TicketId = @TicketId AND IsDeleted = 0;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, NewValue)
        VALUES (@TicketId, @UpdatedBy, 'Updated', 'Ticket details updated');
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Ticket updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateTicketStatus]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- SP 5: Update Ticket Status
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateTicketStatus]
    @TicketId INT,
    @NewStatus NVARCHAR(50),
    @ChangedBy INT,
    @Remarks NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @OldStatus NVARCHAR(50);
        DECLARE @CurrentDate DATETIME = GETDATE();
        
        SELECT @OldStatus = Status FROM Tickets WHERE TicketId = @TicketId;
        
        UPDATE Tickets
        SET 
            Status = @NewStatus,
            UpdatedDate = @CurrentDate,
            ResolvedDate = CASE WHEN @NewStatus = 'Resolved' THEN @CurrentDate ELSE ResolvedDate END,
            ClosedDate = CASE WHEN @NewStatus = 'Closed' THEN @CurrentDate ELSE ClosedDate END
        WHERE TicketId = @TicketId;
        
        INSERT INTO TicketHistory (TicketId, ChangedBy, ChangeType, OldValue, NewValue, Remarks)
        VALUES (@TicketId, @ChangedBy, 'Status Changed', @OldStatus, @NewStatus, @Remarks);
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Status updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUser]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ============================================================
   4. UPDATE USER WITH ROLES
   ============================================================ */

CREATE   PROCEDURE [dbo].[sp_UpdateUser]
    @UserId INT,
    @FullName NVARCHAR(200),
    @Email NVARCHAR(200),
    @PhoneNumber NVARCHAR(20) = NULL,
    @IsActive BIT,
    @RoleIds NVARCHAR(500),
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1 
            FROM Users 
            WHERE Email = @Email 
              AND Id <> @UserId 
              AND ISNULL(IsDeleted, 0) = 0
        )
        BEGIN
            SELECT 0 AS Success, 'Email already in use' AS Message;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Users
        SET 
            FullName = @FullName,
            Email = @Email,
            PhoneNumber = @PhoneNumber,
            IsActive = @IsActive,
            UpdatedBy = @UpdatedBy,
            UpdatedDate = GETDATE()
        WHERE Id = @UserId;

        UPDATE UserRoles
        SET IsActive = 0
        WHERE UserId = @UserId;

        IF @RoleIds IS NOT NULL AND LEN(@RoleIds) > 0
        BEGIN
            INSERT INTO UserRoles
            (
                UserId,
                RoleId,
                AssignedBy,
                AssignedDate,
                IsActive
            )
            SELECT
                @UserId,
                CAST(LTRIM(RTRIM(value)) AS INT),
                @UpdatedBy,
                GETDATE(),
                1
            FROM STRING_SPLIT(@RoleIds, ',')
            WHERE LTRIM(RTRIM(value)) <> '';
        END

        COMMIT TRANSACTION;

        SELECT 1 AS Success, 'User updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserLastLogin]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- 7. sp_UpdateUserLastLogin
-- =============================================
CREATE   PROCEDURE [dbo].[sp_UpdateUserLastLogin]
    @UserId INT,
    @LastLoginDate DATETIME2,
    @LastLoginIp NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET 
        LastLoginDate = @LastLoginDate,
        LastLoginIp = @LastLoginIp,
        UpdatedDate = GETDATE()
    WHERE Id = @UserId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserProfile]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateUserProfile]  
    @UserId INT,  
    @FirstName NVARCHAR(100),  
    @LastName NVARCHAR(100),  
    @Email NVARCHAR(255),  
    @PhoneNumber NVARCHAR(20) = NULL,
    @DateOfBirth DATE = NULL,              -- ADD
    @Address NVARCHAR(500) = NULL,         -- ADD
    @ProfilePicture NVARCHAR(500) = NULL,  -- ADD
    @UpdatedBy INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
      
    BEGIN TRY  
        BEGIN TRANSACTION;  
          
        -- Check if email already exists for another user  
        IF EXISTS (  
            SELECT 1 FROM Users   
            WHERE Email = @Email   
                AND Id != @UserId   
                AND IsDeleted = 0  
        )  
        BEGIN  
            ROLLBACK TRANSACTION;  
            SELECT 0 AS Success, 'Email already exists' AS Message;  
            RETURN;  
        END  
          
        -- Update user profile  
        UPDATE Users  
        SET   
            FirstName = @FirstName,  
            LastName = @LastName,  
            Email = @Email,  
            PhoneNumber = @PhoneNumber,
            DateOfBirth = @DateOfBirth,           -- ADD
            Address = @Address,                   -- ADD
            ProfilePicture = COALESCE(@ProfilePicture, ProfilePicture),  -- Keep existing if NULL
            UpdatedDate = GETUTCDATE(),  
            UpdatedBy = @UpdatedBy  
        WHERE Id = @UserId AND IsDeleted = 0;  
          
        IF @@ROWCOUNT > 0  
        BEGIN  
            -- Log audit  
            INSERT INTO AuditLogs (UserId, Action, NewValues, CreatedDate)  
            VALUES (  
                @UpdatedBy,  
                'Profile Updated',  
                CONCAT('Updated profile for user: ', @FirstName, ' ', @LastName),  
                GETUTCDATE()  
            );  
              
            COMMIT TRANSACTION;  
            SELECT 1 AS Success, 'Profile updated successfully' AS Message;  
        END  
        ELSE  
        BEGIN  
            ROLLBACK TRANSACTION;  
            SELECT 0 AS Success, 'User not found' AS Message;  
        END  
          
    END TRY  
    BEGIN CATCH  
        IF @@TRANCOUNT > 0  
            ROLLBACK TRANSACTION;  
              
        SELECT   
            0 AS Success,   
            ERROR_MESSAGE() AS Message;  
    END CATCH  
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserSettings]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateUserSettings]
    @UserId INT,
    @Theme NVARCHAR(20) = 'light',
    @Language NVARCHAR(10) = 'en',
    @EmailNotifications BIT = 1,
    @TwoFactorEnabled BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if UserSettings table exists, if not create it
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserSettings')
    BEGIN
        CREATE TABLE UserSettings (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UserId INT NOT NULL UNIQUE,
            Theme NVARCHAR(20) DEFAULT 'light',
            Language NVARCHAR(10) DEFAULT 'en',
            EmailNotifications BIT DEFAULT 1,
            TwoFactorEnabled BIT DEFAULT 0,
            CreatedDate DATETIME DEFAULT GETUTCDATE(),
            UpdatedDate DATETIME NULL,
            FOREIGN KEY (UserId) REFERENCES Users(Id)
        );
    END
    
    -- Upsert user settings
    MERGE UserSettings AS target
    USING (SELECT @UserId AS UserId) AS source
    ON target.UserId = source.UserId
    WHEN MATCHED THEN
        UPDATE SET
            Theme = @Theme,
            Language = @Language,
            EmailNotifications = @EmailNotifications,
            TwoFactorEnabled = @TwoFactorEnabled,
            UpdatedDate = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (UserId, Theme, Language, EmailNotifications, TwoFactorEnabled, CreatedDate)
        VALUES (@UserId, @Theme, @Language, @EmailNotifications, @TwoFactorEnabled, GETUTCDATE());
    
    SELECT 1 AS Success, 'Settings updated successfully' AS Message;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ValidatePasswordResetToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ValidatePasswordResetToken]  
    @Token NVARCHAR(500)  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT   
        prt.Id,  
        prt.UserId,  
        prt.Token,  
        prt.ExpiryDate,  
        prt.IsUsed,  
        prt.CreatedDate,  -- ✅ Added
        u.Email,  
        u.Username,
        u.FirstName,      -- ✅ Added
        u.LastName        -- ✅ Added
    FROM PasswordResetTokens prt  
    INNER JOIN Users u ON prt.UserId = u.Id  
    WHERE prt.Token = @Token  
    AND prt.IsUsed = 0  
    AND prt.ExpiryDate > GETUTCDATE();  -- ✅ Changed to GETUTCDATE()
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ValidateRefreshToken]    Script Date: 10-06-2026 07:40:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_ValidateRefreshToken]
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        rt.Id,
        rt.UserId,
        rt.Token,
        rt.ExpiryDate,
        rt.RevokedDate,
        u.Username,
        u.Email,
        u.FirstName,
        u.LastName,
        u.IsActive
    FROM RefreshTokens rt
    INNER JOIN Users u ON rt.UserId = u.Id
    WHERE rt.Token = @Token
    AND rt.RevokedDate IS NULL
    AND rt.ExpiryDate > GETDATE()
    AND u.IsActive = 1
    AND u.IsDeleted = 0;
END
GO
USE [master]
GO
ALTER DATABASE [EmployeeDb] SET  READ_WRITE 
GO

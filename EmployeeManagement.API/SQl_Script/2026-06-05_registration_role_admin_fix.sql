USE [EmployeeDb];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RegisterUser]
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

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ApproveUserAndAssignRole]
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

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetPendingUsers]
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

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllUsersWithRoles]
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

-- =============================================
-- Sample Data: Auth Schema
-- Description: Insert sample users, roles, and user-role assignments
-- =============================================
-- Password for all users: "Password123!"
-- Hash generated using BCrypt with work factor 11
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

-- =============================================
-- 1. Insert Roles (if not exists)
-- =============================================
PRINT 'Inserting Roles...';

-- Declare role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @EmployerRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @StudentRoleId UNIQUEIDENTIFIER = NEWID();

-- Insert roles only if they don't exist
IF NOT EXISTS (SELECT 1 FROM [auth].[Roles] WHERE [Code] = 'ADMIN')
BEGIN
    INSERT INTO [auth].[Roles] ([RoleId], [Code], [Name])
    VALUES (@AdminRoleId, 'ADMIN', N'Quản trị viên');
END
ELSE
BEGIN
    SELECT @AdminRoleId = [RoleId] FROM [auth].[Roles] WHERE [Code] = 'ADMIN';
END

IF NOT EXISTS (SELECT 1 FROM [auth].[Roles] WHERE [Code] = 'EMPLOYER')
BEGIN
    INSERT INTO [auth].[Roles] ([RoleId], [Code], [Name])
    VALUES (@EmployerRoleId, 'EMPLOYER', N'Nhà tuyển dụng');
END
ELSE
BEGIN
    SELECT @EmployerRoleId = [RoleId] FROM [auth].[Roles] WHERE [Code] = 'EMPLOYER';
END

IF NOT EXISTS (SELECT 1 FROM [auth].[Roles] WHERE [Code] = 'STUDENT')
BEGIN
    INSERT INTO [auth].[Roles] ([RoleId], [Code], [Name])
    VALUES (@StudentRoleId, 'STUDENT', N'Sinh viên');
END
ELSE
BEGIN
    SELECT @StudentRoleId = [RoleId] FROM [auth].[Roles] WHERE [Code] = 'STUDENT';
END

PRINT 'Roles inserted successfully.';
GO

-- =============================================
-- 2. Insert Sample Users
-- =============================================
PRINT 'Inserting Sample Users...';

-- Declare user IDs
DECLARE @AdminUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Employer1UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Employer2UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';
DECLARE @Student1UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Student2UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @Student3UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';

-- Password hash for "Password123!" (BCrypt hash)
DECLARE @PasswordHash NVARCHAR(255) = '$2a$11$6BQKXn0ShQwSRx.sLGO4fOKdWpqyGKQHrYUPYlFfpF7EQrZr8ZXcG';

-- Admin User
IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'admin@ptj.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@AdminUserId, 'admin@ptj.com', 'ADMIN@PTJ.COM', '0912345678', N'Nguyễn Văn Admin',
     @PasswordHash, 1, 1, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Employer Users
IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'employer1@company.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Employer1UserId, 'employer1@company.com', 'EMPLOYER1@COMPANY.COM', '0923456789',
     N'Trần Thị Mai', @PasswordHash, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'employer2@techcorp.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Employer2UserId, 'employer2@techcorp.com', 'EMPLOYER2@TECHCORP.COM', '0934567890',
     N'Lê Văn Hùng', @PasswordHash, 1, 1, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Student Users
IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'student1@gmail.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Student1UserId, 'student1@gmail.com', 'STUDENT1@GMAIL.COM', '0945678901',
     N'Phạm Minh Tuấn', @PasswordHash, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'student2@gmail.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Student2UserId, 'student2@gmail.com', 'STUDENT2@GMAIL.COM', '0956789012',
     N'Ngô Thị Hương', @PasswordHash, 1, 1, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [auth].[Users] WHERE [Email] = 'student3@gmail.com')
BEGIN
    INSERT INTO [auth].[Users]
    ([UserId], [Email], [NormalizedEmail], [PhoneNumber], [FullName], [PasswordHash],
     [IsEmailVerified], [IsPhoneVerified], [IsActive], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Student3UserId, 'student3@gmail.com', 'STUDENT3@GMAIL.COM', '0967890123',
     N'Hoàng Văn Đức', @PasswordHash, 1, 0, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

PRINT 'Sample Users inserted successfully.';
GO

-- =============================================
-- 3. Assign Roles to Users
-- =============================================
PRINT 'Assigning Roles to Users...';

-- Get role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT [RoleId] FROM [auth].[Roles] WHERE [Code] = 'ADMIN');
DECLARE @EmployerRoleId UNIQUEIDENTIFIER = (SELECT [RoleId] FROM [auth].[Roles] WHERE [Code] = 'EMPLOYER');
DECLARE @StudentRoleId UNIQUEIDENTIFIER = (SELECT [RoleId] FROM [auth].[Roles] WHERE [Code] = 'STUDENT');

-- User IDs
DECLARE @AdminUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Employer1UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Employer2UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';
DECLARE @Student1UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Student2UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @Student3UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';

-- Assign ADMIN role
IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @AdminUserId AND [RoleId] = @AdminRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@AdminUserId, @AdminRoleId, GETUTCDATE());
END

-- Assign EMPLOYER roles
IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @Employer1UserId AND [RoleId] = @EmployerRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@Employer1UserId, @EmployerRoleId, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @Employer2UserId AND [RoleId] = @EmployerRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@Employer2UserId, @EmployerRoleId, GETUTCDATE());
END

-- Assign STUDENT roles
IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @Student1UserId AND [RoleId] = @StudentRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@Student1UserId, @StudentRoleId, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @Student2UserId AND [RoleId] = @StudentRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@Student2UserId, @StudentRoleId, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [auth].[UserRoles] WHERE [UserId] = @Student3UserId AND [RoleId] = @StudentRoleId)
BEGIN
    INSERT INTO [auth].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@Student3UserId, @StudentRoleId, GETUTCDATE());
END

PRINT 'Roles assigned successfully.';
GO

PRINT '';
PRINT '========================================';
PRINT 'Auth Sample Data Inserted Successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Sample Accounts:';
PRINT '  Admin:     admin@ptj.com / Password123!';
PRINT '  Employer1: employer1@company.com / Password123!';
PRINT '  Employer2: employer2@techcorp.com / Password123!';
PRINT '  Student1:  student1@gmail.com / Password123!';
PRINT '  Student2:  student2@gmail.com / Password123!';
PRINT '  Student3:  student3@gmail.com / Password123!';
PRINT '';

Use PartTimeJobs
Go

-- Insert default roles into auth.Roles table
-- Roles: ADMIN, EMPLOYER, STUDENT

-- Clear existing roles (optional - uncomment if needed)
-- DELETE FROM [auth].[Roles];

-- Insert roles
INSERT INTO [auth].[Roles] ([RoleId], [Code], [Name])
VALUES
    (NEWID(), 'ADMIN', N'Quản trị viên'),
    (NEWID(), 'EMPLOYER', N'Nhà tuyển dụng'),
    (NEWID(), 'STUDENT', N'Sinh viên');

-- Verify inserted roles
SELECT * FROM [auth].[Roles];

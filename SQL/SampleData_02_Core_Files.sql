-- =============================================
-- Sample Data: Core Schema (Files)
-- Description: Insert sample files (avatars, resumes, logos, etc.)
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT 'Inserting Sample Files...';

-- User IDs
DECLARE @AdminUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Employer1UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Employer2UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';
DECLARE @Student1UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Student2UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @Student3UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';

-- File IDs (predefined for easy reference)
DECLARE @Avatar1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444441';
DECLARE @Avatar2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444442';
DECLARE @Avatar3FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444443';
DECLARE @Resume1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444451';
DECLARE @Resume2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444452';
DECLARE @Logo1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444461';
DECLARE @Logo2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444462';

-- Insert Avatar Files
IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Avatar1FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Avatar1FileId, 'avatar_student1.jpg', 'image/jpeg', 245678,
     '/uploads/avatars/avatar_student1.jpg', 'Local',
     'abc123def456', @Student1UserId, GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Avatar2FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Avatar2FileId, 'avatar_student2.jpg', 'image/jpeg', 198234,
     '/uploads/avatars/avatar_student2.jpg', 'Local',
     'def456ghi789', @Student2UserId, GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Avatar3FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Avatar3FileId, 'avatar_employer1.jpg', 'image/jpeg', 312456,
     '/uploads/avatars/avatar_employer1.jpg', 'Local',
     'ghi789jkl012', @Employer1UserId, GETUTCDATE(), 0);
END

-- Insert Resume Files
IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Resume1FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Resume1FileId, 'CV_PhamMinhTuan.pdf', 'application/pdf', 567890,
     '/uploads/resumes/CV_PhamMinhTuan.pdf', 'Local',
     'jkl012mno345', @Student1UserId, GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Resume2FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Resume2FileId, 'CV_NgoThiHuong.pdf', 'application/pdf', 489123,
     '/uploads/resumes/CV_NgoThiHuong.pdf', 'Local',
     'mno345pqr678', @Student2UserId, GETUTCDATE(), 0);
END

-- Insert Company Logo Files
IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Logo1FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Logo1FileId, 'logo_fpt_software.png', 'image/png', 128456,
     '/uploads/logos/logo_fpt_software.png', 'Local',
     'pqr678stu901', @Employer1UserId, GETUTCDATE(), 0);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Files] WHERE [FileId] = @Logo2FileId)
BEGIN
    INSERT INTO [core].[Files]
    ([FileId], [FileName], [ContentType], [ByteSize], [StorageUrl], [StorageProvider],
     [Checksum], [OwnerUserId], [CreatedAt], [IsDeleted])
    VALUES
    (@Logo2FileId, 'logo_vng_corporation.png', 'image/png', 156789,
     '/uploads/logos/logo_vng_corporation.png', 'Local',
     'stu901vwx234', @Employer2UserId, GETUTCDATE(), 0);
END

PRINT 'Sample Files inserted successfully.';
GO

PRINT '';
PRINT '========================================';
PRINT 'Core Sample Data Inserted Successfully!';
PRINT '========================================';
PRINT 'Files created: 7 (3 avatars, 2 resumes, 2 logos)';
PRINT '';

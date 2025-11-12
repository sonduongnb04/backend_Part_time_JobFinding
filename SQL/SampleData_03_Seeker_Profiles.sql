-- =============================================
-- Sample Data: Seeker Schema (Profiles)
-- Description: Insert sample student profiles with skills, experiences, education, certificates
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT 'Inserting Sample Student Profiles...';

-- User IDs
DECLARE @Student1UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Student2UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @Student3UserId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';

-- File IDs
DECLARE @Avatar1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444441';
DECLARE @Avatar2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444442';
DECLARE @Resume1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444451';
DECLARE @Resume2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444452';

-- Profile IDs
DECLARE @Profile1Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @Profile2Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';
DECLARE @Profile3Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555553';

-- =============================================
-- 1. Insert Profiles
-- =============================================

-- Profile 1: Phạm Minh Tuấn (IT Student)
IF NOT EXISTS (SELECT 1 FROM [seeker].[Profiles] WHERE [ProfileId] = @Profile1Id)
BEGIN
    INSERT INTO [seeker].[Profiles]
    ([ProfileId], [StudentUserId], [FirstName], [LastName], [DateOfBirth], [Gender],
     [Email], [PhoneNumber], [AddressLine1], [Ward], [District], [City], [Province],
     [University], [Major], [StudentId], [GraduationYear], [GPA],
     [AvatarFileId], [ResumeFileId], [Bio], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Profile1Id, @Student1UserId, N'Minh Tuấn', N'Phạm', '2002-03-15', N'Nam',
     'student1@gmail.com', '0945678901', N'123 Đường Láng', N'Láng Thượng', N'Đống Đa',
     N'Hà Nội', N'Hà Nội', N'Đại học Bách Khoa Hà Nội', N'Công nghệ thông tin',
     'SV20020123', 2024, 3.45, @Avatar1FileId, @Resume1FileId,
     N'Sinh viên năm 4 chuyên ngành Công nghệ phần mềm. Có kinh nghiệm làm việc với C#, .NET, và SQL Server.',
     GETUTCDATE(), GETUTCDATE(), 0);
END

-- Profile 2: Ngô Thị Hương (Business Student)
IF NOT EXISTS (SELECT 1 FROM [seeker].[Profiles] WHERE [ProfileId] = @Profile2Id)
BEGIN
    INSERT INTO [seeker].[Profiles]
    ([ProfileId], [StudentUserId], [FirstName], [LastName], [DateOfBirth], [Gender],
     [Email], [PhoneNumber], [AddressLine1], [Ward], [District], [City], [Province],
     [University], [Major], [StudentId], [GraduationYear], [GPA],
     [AvatarFileId], [ResumeFileId], [Bio], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Profile2Id, @Student2UserId, N'Thị Hương', N'Ngô', '2003-07-22', N'Nữ',
     'student2@gmail.com', '0956789012', N'456 Nguyễn Trãi', N'Thanh Xuân Nam', N'Thanh Xuân',
     N'Hà Nội', N'Hà Nội', N'Đại học Kinh tế Quốc dân', N'Quản trị kinh doanh',
     'SV20030456', 2025, 3.67, @Avatar2FileId, @Resume2FileId,
     N'Sinh viên năm 3 ngành Quản trị kinh doanh. Đam mê marketing và kinh doanh online.',
     GETUTCDATE(), GETUTCDATE(), 0);
END

-- Profile 3: Hoàng Văn Đức (Engineering Student)
IF NOT EXISTS (SELECT 1 FROM [seeker].[Profiles] WHERE [ProfileId] = @Profile3Id)
BEGIN
    INSERT INTO [seeker].[Profiles]
    ([ProfileId], [StudentUserId], [FirstName], [LastName], [DateOfBirth], [Gender],
     [Email], [PhoneNumber], [AddressLine1], [Ward], [District], [City], [Province],
     [University], [Major], [StudentId], [GraduationYear], [GPA],
     [AvatarFileId], [ResumeFileId], [Bio], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Profile3Id, @Student3UserId, N'Văn Đức', N'Hoàng', '2002-11-08', N'Nam',
     'student3@gmail.com', '0967890123', N'789 Lê Duẩn', N'Bến Nghé', N'Quận 1',
     N'TP. Hồ Chí Minh', N'TP. Hồ Chí Minh', N'Đại học Bách Khoa TP.HCM', N'Kỹ thuật cơ khí',
     'SV20020789', 2024, 3.21, NULL, NULL,
     N'Sinh viên năm 4 ngành Kỹ thuật cơ khí. Quan tâm đến thiết kế sản phẩm và AutoCAD.',
     GETUTCDATE(), GETUTCDATE(), 0);
END

PRINT 'Sample Profiles inserted successfully.';
GO

-- =============================================
-- 2. Insert Profile Skills
-- =============================================

PRINT 'Inserting Sample Profile Skills...';

DECLARE @Profile1Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @Profile2Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';
DECLARE @Profile3Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555553';

-- Skill IDs (generated)
DECLARE @Skill_CSharp UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_DotNet UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_SQL UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_Marketing UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_Excel UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_AutoCAD UNIQUEIDENTIFIER = NEWID();

-- Profile 1 Skills (IT Student)
INSERT INTO [seeker].[ProfileSkills] ([ProfileId], [SkillId], [ProficiencyLevel])
VALUES
    (@Profile1Id, @Skill_CSharp, 4),
    (@Profile1Id, @Skill_DotNet, 4),
    (@Profile1Id, @Skill_SQL, 3);

-- Profile 2 Skills (Business Student)
INSERT INTO [seeker].[ProfileSkills] ([ProfileId], [SkillId], [ProficiencyLevel])
VALUES
    (@Profile2Id, @Skill_Marketing, 4),
    (@Profile2Id, @Skill_Excel, 5);

-- Profile 3 Skills (Engineering Student)
INSERT INTO [seeker].[ProfileSkills] ([ProfileId], [SkillId], [ProficiencyLevel])
VALUES
    (@Profile3Id, @Skill_AutoCAD, 3);

PRINT 'Sample Profile Skills inserted successfully.';
GO

-- =============================================
-- 3. Insert Profile Experiences
-- =============================================

PRINT 'Inserting Sample Profile Experiences...';

DECLARE @Profile1Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @Profile2Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';

-- Profile 1 Experiences
INSERT INTO [seeker].[ProfileExperiences]
([ExperienceId], [ProfileId], [Title], [CompanyName], [StartDate], [EndDate], [IsCurrent], [Description], [CreatedAt])
VALUES
(NEWID(), @Profile1Id, N'Thực tập sinh lập trình viên', N'FPT Software', '2023-06-01', '2023-08-31', 0,
 N'Tham gia phát triển module quản lý nhân sự sử dụng ASP.NET Core và SQL Server.', GETUTCDATE()),
(NEWID(), @Profile1Id, N'Part-time Developer', N'Startup ABC', '2023-09-01', NULL, 1,
 N'Phát triển website bán hàng online sử dụng C# và React.', GETUTCDATE());

-- Profile 2 Experiences
INSERT INTO [seeker].[ProfileExperiences]
([ExperienceId], [ProfileId], [Title], [CompanyName], [StartDate], [EndDate], [IsCurrent], [Description], [CreatedAt])
VALUES
(NEWID(), @Profile2Id, N'Marketing Intern', N'VNG Corporation', '2024-01-01', '2024-03-31', 0,
 N'Hỗ trợ team marketing trong các chiến dịch quảng cáo trên Facebook và Google Ads.', GETUTCDATE());

PRINT 'Sample Profile Experiences inserted successfully.';
GO

-- =============================================
-- 4. Insert Profile Educations
-- =============================================

PRINT 'Inserting Sample Profile Educations...';

DECLARE @Profile1Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @Profile2Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';
DECLARE @Profile3Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555553';

INSERT INTO [seeker].[ProfileEducations]
([EducationId], [ProfileId], [School], [Degree], [FieldOfStudy], [StartDate], [EndDate], [GPA], [CertificateFileId], [CreatedAt])
VALUES
(NEWID(), @Profile1Id, N'Đại học Bách Khoa Hà Nội', N'Kỹ sư', N'Công nghệ thông tin', '2020-09-01', '2024-06-30', 3.45, NULL, GETUTCDATE()),
(NEWID(), @Profile2Id, N'Đại học Kinh tế Quốc dân', N'Cử nhân', N'Quản trị kinh doanh', '2021-09-01', '2025-06-30', 3.67, NULL, GETUTCDATE()),
(NEWID(), @Profile3Id, N'Đại học Bách Khoa TP.HCM', N'Kỹ sư', N'Kỹ thuật cơ khí', '2020-09-01', '2024-06-30', 3.21, NULL, GETUTCDATE());

PRINT 'Sample Profile Educations inserted successfully.';
GO

-- =============================================
-- 5. Insert Profile Certificates
-- =============================================

PRINT 'Inserting Sample Profile Certificates...';

DECLARE @Profile1Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @Profile2Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';

INSERT INTO [seeker].[ProfileCertificates]
([CertificateId], [ProfileId], [Name], [IssuedBy], [IssuedDate], [ExpiryDate], [CertificateFileId], [CreatedAt])
VALUES
(NEWID(), @Profile1Id, N'Microsoft Certified: Azure Fundamentals', N'Microsoft', '2023-05-15', NULL, NULL, GETUTCDATE()),
(NEWID(), @Profile1Id, N'TOEIC 850', N'ETS', '2023-03-20', '2025-03-20', NULL, GETUTCDATE()),
(NEWID(), @Profile2Id, N'Google Analytics Certification', N'Google', '2024-02-10', '2025-02-10', NULL, GETUTCDATE());

PRINT 'Sample Profile Certificates inserted successfully.';
GO

PRINT '';
PRINT '========================================';
PRINT 'Seeker Sample Data Inserted Successfully!';
PRINT '========================================';
PRINT 'Profiles: 3';
PRINT 'Skills: 6';
PRINT 'Experiences: 3';
PRINT 'Educations: 3';
PRINT 'Certificates: 3';
PRINT '';

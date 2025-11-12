-- =============================================
-- Sample Data: Jobs Schema (JobPosts, JobShifts, JobPostSkills)
-- Description: Insert sample job postings with shifts and required skills
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT 'Inserting Sample Job Posts...';

-- Company IDs
DECLARE @Company1Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666661'; -- FPT Software
DECLARE @Company2Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666662'; -- VNG Corporation
DECLARE @Company3Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666663'; -- Startup Tech Hub

-- User IDs (Employers who create posts)
DECLARE @Employer1UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Employer2UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';

-- Job Post IDs
DECLARE @Job1Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777771';
DECLARE @Job2Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777772';
DECLARE @Job3Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777773';
DECLARE @Job4Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777774';
DECLARE @Job5Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777775';

-- =============================================
-- 1. Insert Job Posts
-- =============================================

-- Job 1: Lập trình viên C# part-time
IF NOT EXISTS (SELECT 1 FROM [jobs].[JobPosts] WHERE [JobPostId] = @Job1Id)
BEGIN
    INSERT INTO [jobs].[JobPosts]
    ([JobPostId], [CompanyId], [Title], [CategoryId], [Description], [Requirements], [Benefits],
     [StatusId], [SalaryMin], [SalaryMax], [Currency], [SalaryUnitId], [ArrangementId],
     [AddressLine1], [Ward], [District], [City], [Province], [Latitude], [Longitude],
     [Slots], [PublishAt], [ExpireAt], [ViewCount], [CreatedBy], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Job1Id, @Company1Id, N'Lập trình viên C# Part-time', NULL,
     N'FPT Software tuyển dụng lập trình viên C# part-time để hỗ trợ phát triển các dự án nội bộ. Sinh viên năm 3, 4 ưu tiên.',
     N'- Có kiến thức về C#, .NET Framework/Core
- Biết sử dụng SQL Server
- Có khả năng làm việc nhóm
- Có thể làm việc 3-4 buổi/tuần',
     N'- Lương: 50-80k/giờ
- Môi trường làm việc chuyên nghiệp
- Được mentor bởi senior developers
- Cơ hội trở thành nhân viên chính thức',
     1, 50000, 80000, 'VND', 1, 1, -- StatusId: 1=Published, SalaryUnitId: 1=Hourly, ArrangementId: 1=Part-time
     N'Tòa nhà FPT, 17 Duy Tân', N'Dịch Vọng Hậu', N'Cầu Giấy', N'Hà Nội', N'Hà Nội',
     21.0285, 105.7821, 5, GETUTCDATE(), DATEADD(DAY, 30, GETUTCDATE()), 125,
     @Employer1UserId, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Job 2: Marketing Intern
IF NOT EXISTS (SELECT 1 FROM [jobs].[JobPosts] WHERE [JobPostId] = @Job2Id)
BEGIN
    INSERT INTO [jobs].[JobPosts]
    ([JobPostId], [CompanyId], [Title], [CategoryId], [Description], [Requirements], [Benefits],
     [StatusId], [SalaryMin], [SalaryMax], [Currency], [SalaryUnitId], [ArrangementId],
     [AddressLine1], [Ward], [District], [City], [Province], [Latitude], [Longitude],
     [Slots], [PublishAt], [ExpireAt], [ViewCount], [CreatedBy], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Job2Id, @Company2Id, N'Thực tập sinh Marketing', NULL,
     N'VNG tuyển dụng thực tập sinh Marketing để hỗ trợ các chiến dịch quảng bá sản phẩm Zalo và ZaloPay.',
     N'- Sinh viên năm 3, 4 ngành Marketing, Quản trị kinh doanh
- Có kiến thức về Digital Marketing
- Biết sử dụng Facebook Ads, Google Ads là lợi thế
- Chăm chỉ, ham học hỏi',
     N'- Trợ cấp: 3-4 triệu/tháng
- Làm việc tại công ty công nghệ hàng đầu VN
- Được đào tạo bài bản
- Cơ hội được nhận vào làm chính thức',
     1, 3000000, 4000000, 'VND', 2, 2, -- SalaryUnitId: 2=Monthly, ArrangementId: 2=Internship
     N'Z06, 13 Tân Trào', N'Tân Phú', N'Quận 7', N'TP. Hồ Chí Minh', N'TP. Hồ Chí Minh',
     10.7327, 106.7218, 3, GETUTCDATE(), DATEADD(DAY, 45, GETUTCDATE()), 87,
     @Employer2UserId, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Job 3: Nhân viên bán hàng part-time
IF NOT EXISTS (SELECT 1 FROM [jobs].[JobPosts] WHERE [JobPostId] = @Job3Id)
BEGIN
    INSERT INTO [jobs].[JobPosts]
    ([JobPostId], [CompanyId], [Title], [CategoryId], [Description], [Requirements], [Benefits],
     [StatusId], [SalaryMin], [SalaryMax], [Currency], [SalaryUnitId], [ArrangementId],
     [AddressLine1], [Ward], [District], [City], [Province], [Latitude], [Longitude],
     [Slots], [PublishAt], [ExpireAt], [ViewCount], [CreatedBy], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Job3Id, @Company3Id, N'Nhân viên bán hàng Online Part-time', NULL,
     N'Startup Tech Hub tuyển nhân viên bán hàng online để hỗ trợ chăm sóc khách hàng và tư vấn sản phẩm.',
     N'- Không yêu cầu kinh nghiệm
- Có khả năng giao tiếp tốt
- Biết sử dụng Facebook, Zalo
- Làm việc tại nhà (Remote)',
     N'- Lương cơ bản: 25k/giờ + hoa hồng
- Làm việc linh hoạt
- Được đào tạo kỹ năng bán hàng',
     1, 25000, 40000, 'VND', 1, 3, -- SalaryUnitId: 1=Hourly, ArrangementId: 3=Remote
     N'123 Nguyễn Thái Học', N'Điện Biên', N'Ba Đình', N'Hà Nội', N'Hà Nội',
     21.0352, 105.8346, 10, GETUTCDATE(), DATEADD(DAY, 60, GETUTCDATE()), 234,
     @Employer1UserId, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Job 4: Developer Fullstack (đã hết hạn - để test)
IF NOT EXISTS (SELECT 1 FROM [jobs].[JobPosts] WHERE [JobPostId] = @Job4Id)
BEGIN
    INSERT INTO [jobs].[JobPosts]
    ([JobPostId], [CompanyId], [Title], [CategoryId], [Description], [Requirements], [Benefits],
     [StatusId], [SalaryMin], [SalaryMax], [Currency], [SalaryUnitId], [ArrangementId],
     [AddressLine1], [Ward], [District], [City], [Province], [Latitude], [Longitude],
     [Slots], [PublishAt], [ExpireAt], [ViewCount], [CreatedBy], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Job4Id, @Company1Id, N'Fullstack Developer Part-time', NULL,
     N'Tuyển fullstack developer có kinh nghiệm React và .NET.',
     N'- Có kinh nghiệm React, .NET Core
- Biết SQL Server',
     N'- Lương: 60-100k/giờ',
     1, 60000, 100000, 'VND', 1, 1,
     N'Tòa nhà FPT, 17 Duy Tân', N'Dịch Vọng Hậu', N'Cầu Giấy', N'Hà Nội', N'Hà Nội',
     21.0285, 105.7821, 2, DATEADD(DAY, -30, GETUTCDATE()), DATEADD(DAY, -5, GETUTCDATE()), 45,
     @Employer1UserId, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Job 5: Content Writer
IF NOT EXISTS (SELECT 1 FROM [jobs].[JobPosts] WHERE [JobPostId] = @Job5Id)
BEGIN
    INSERT INTO [jobs].[JobPosts]
    ([JobPostId], [CompanyId], [Title], [CategoryId], [Description], [Requirements], [Benefits],
     [StatusId], [SalaryMin], [SalaryMax], [Currency], [SalaryUnitId], [ArrangementId],
     [AddressLine1], [Ward], [District], [City], [Province], [Latitude], [Longitude],
     [Slots], [PublishAt], [ExpireAt], [ViewCount], [CreatedBy], [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Job5Id, @Company2Id, N'Content Writer - Viết nội dung mạng xã hội', NULL,
     N'Tuyển content writer để viết nội dung cho các kênh social media của VNG.',
     N'- Có khả năng viết lách tốt
- Am hiểu về social media
- Sáng tạo, cập nhật xu hướng',
     N'- Lương: 30-50k/giờ
- Làm việc remote
- Thời gian linh hoạt',
     1, 30000, 50000, 'VND', 1, 3,
     N'Z06, 13 Tân Trào', N'Tân Phú', N'Quận 7', N'TP. Hồ Chí Minh', N'TP. Hồ Chí Minh',
     10.7327, 106.7218, 4, GETUTCDATE(), DATEADD(DAY, 20, GETUTCDATE()), 156,
     @Employer2UserId, GETUTCDATE(), GETUTCDATE(), 0);
END

PRINT 'Sample Job Posts inserted successfully.';
GO

-- =============================================
-- 2. Insert Job Shifts
-- =============================================

PRINT 'Inserting Sample Job Shifts...';

DECLARE @Job1Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777771';
DECLARE @Job2Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777772';
DECLARE @Job3Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777773';

-- Job 1 Shifts (C# Developer)
INSERT INTO [jobs].[JobShifts]
([JobShiftId], [JobPostId], [ShiftName], [DayOfWeek], [StartTime], [EndTime], [Note])
VALUES
(NEWID(), @Job1Id, N'Ca chiều Thứ 2, 4, 6', 2, '13:00', '17:00', N'Làm việc tại văn phòng'),
(NEWID(), @Job1Id, N'Ca chiều Thứ 2, 4, 6', 4, '13:00', '17:00', N'Làm việc tại văn phòng'),
(NEWID(), @Job1Id, N'Ca chiều Thứ 2, 4, 6', 6, '13:00', '17:00', N'Làm việc tại văn phòng');

-- Job 2 Shifts (Marketing Intern)
INSERT INTO [jobs].[JobShifts]
([JobShiftId], [JobPostId], [ShiftName], [DayOfWeek], [StartTime], [EndTime], [Note])
VALUES
(NEWID(), @Job2Id, N'Full-time Thứ 2-6', 2, '08:30', '17:30', N'Nghỉ trưa 12:00-13:30'),
(NEWID(), @Job2Id, N'Full-time Thứ 2-6', 3, '08:30', '17:30', N'Nghỉ trưa 12:00-13:30'),
(NEWID(), @Job2Id, N'Full-time Thứ 2-6', 4, '08:30', '17:30', N'Nghỉ trưa 12:00-13:30'),
(NEWID(), @Job2Id, N'Full-time Thứ 2-6', 5, '08:30', '17:30', N'Nghỉ trưa 12:00-13:30'),
(NEWID(), @Job2Id, N'Full-time Thứ 2-6', 6, '08:30', '17:30', N'Nghỉ trưa 12:00-13:30');

-- Job 3 Shifts (Sales - Remote, flexible)
INSERT INTO [jobs].[JobShifts]
([JobShiftId], [JobPostId], [ShiftName], [DayOfWeek], [StartTime], [EndTime], [Note])
VALUES
(NEWID(), @Job3Id, N'Linh hoạt', NULL, NULL, NULL, N'Làm việc online, tự sắp xếp thời gian');

PRINT 'Sample Job Shifts inserted successfully.';
GO

-- =============================================
-- 3. Insert Job Post Skills
-- =============================================

PRINT 'Inserting Sample Job Post Skills...';

DECLARE @Job1Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777771';
DECLARE @Job2Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777772';
DECLARE @Job5Id UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777775';

-- Skill IDs (reuse from Profile sample data or create new)
DECLARE @Skill_CSharp UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_DotNet UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_SQL UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_Marketing UNIQUEIDENTIFIER = NEWID();
DECLARE @Skill_Writing UNIQUEIDENTIFIER = NEWID();

-- Job 1 Required Skills (C# Developer)
INSERT INTO [jobs].[JobPostSkills] ([JobPostId], [SkillId])
VALUES
    (@Job1Id, @Skill_CSharp),
    (@Job1Id, @Skill_DotNet),
    (@Job1Id, @Skill_SQL);

-- Job 2 Required Skills (Marketing Intern)
INSERT INTO [jobs].[JobPostSkills] ([JobPostId], [SkillId])
VALUES
    (@Job2Id, @Skill_Marketing);

-- Job 5 Required Skills (Content Writer)
INSERT INTO [jobs].[JobPostSkills] ([JobPostId], [SkillId])
VALUES
    (@Job5Id, @Skill_Writing);

PRINT 'Sample Job Post Skills inserted successfully.';
GO

PRINT '';
PRINT '========================================';
PRINT 'Jobs Sample Data Inserted Successfully!';
PRINT '========================================';
PRINT 'Job Posts: 5';
PRINT '  - 3 Active jobs';
PRINT '  - 1 Expired job (for testing)';
PRINT 'Job Shifts: 9';
PRINT 'Job Skills: 5';
PRINT '';

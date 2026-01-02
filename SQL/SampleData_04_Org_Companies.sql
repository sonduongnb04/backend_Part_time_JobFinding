-- =============================================
-- Sample Data: Org Schema (Companies)
-- Description: Insert sample companies
-- =============================================

USE [PTJ_Database]; -- Replace with your database name
GO

PRINT 'Inserting Sample Companies...';

-- User IDs (Employers)
DECLARE @Employer1UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Employer2UserId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';

-- File IDs (Logos)
DECLARE @Logo1FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444461';
DECLARE @Logo2FileId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444462';

-- Company IDs
DECLARE @Company1Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666661';
DECLARE @Company2Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666662';
DECLARE @Company3Id UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666663';

-- =============================================
-- Insert Companies
-- =============================================

-- Company 1: FPT Software
IF NOT EXISTS (SELECT 1 FROM [org].[Companies] WHERE [CompanyId] = @Company1Id)
BEGIN
    INSERT INTO [org].[Companies]
    ([CompanyId], [OwnerUserId], [Name], [IndustryId], [Description], [WebsiteUrl],
     [LogoFileId], [EmailPublic], [PhonePublic], [AddressLine1], [Ward], [District],
     [City], [Province], [PostalCode], [Latitude], [Longitude], [Verification],
     [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Company1Id, @Employer1UserId, N'FPT Software', NULL,
     N'FPT Software là công ty phần mềm hàng đầu Việt Nam, chuyên cung cấp dịch vụ công nghệ thông tin và giải pháp phần mềm cho khách hàng toàn cầu.',
     'https://www.fpt-software.com', @Logo1FileId, 'contact@fpt-software.com', '024-7300-8866',
     N'Tòa nhà FPT, 17 Duy Tân', N'Dịch Vọng Hậu', N'Cầu Giấy', N'Hà Nội', N'Hà Nội', '100000',
     21.0285, 105.7821, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Company 2: VNG Corporation
IF NOT EXISTS (SELECT 1 FROM [org].[Companies] WHERE [CompanyId] = @Company2Id)
BEGIN
    INSERT INTO [org].[Companies]
    ([CompanyId], [OwnerUserId], [Name], [IndustryId], [Description], [WebsiteUrl],
     [LogoFileId], [EmailPublic], [PhonePublic], [AddressLine1], [Ward], [District],
     [City], [Province], [PostalCode], [Latitude], [Longitude], [Verification],
     [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Company2Id, @Employer2UserId, N'VNG Corporation', NULL,
     N'VNG là công ty công nghệ internet hàng đầu Việt Nam, sở hữu các sản phẩm nổi tiếng như Zalo, ZaloPay, Zing MP3.',
     'https://www.vng.com.vn', @Logo2FileId, 'hr@vng.com.vn', '028-7300-7888',
     N'Z06, 13 Tân Trào', N'Tân Phú', N'Quận 7', N'TP. Hồ Chí Minh', N'TP. Hồ Chí Minh', '700000',
     10.7327, 106.7218, 1, GETUTCDATE(), GETUTCDATE(), 0);
END

-- Company 3: Startup Tech Hub (Small company)
IF NOT EXISTS (SELECT 1 FROM [org].[Companies] WHERE [CompanyId] = @Company3Id)
BEGIN
    INSERT INTO [org].[Companies]
    ([CompanyId], [OwnerUserId], [Name], [IndustryId], [Description], [WebsiteUrl],
     [LogoFileId], [EmailPublic], [PhonePublic], [AddressLine1], [Ward], [District],
     [City], [Province], [PostalCode], [Latitude], [Longitude], [Verification],
     [CreatedAt], [UpdatedAt], [IsDeleted])
    VALUES
    (@Company3Id, @Employer1UserId, N'Startup Tech Hub', NULL,
     N'Công ty khởi nghiệp chuyên phát triển ứng dụng di động và website cho doanh nghiệp vừa và nhỏ.',
     'https://www.startuptechhub.vn', NULL, 'info@startuptechhub.vn', '0912345678',
     N'123 Nguyễn Thái Học', N'Điện Biên', N'Ba Đình', N'Hà Nội', N'Hà Nội', '100000',
     21.0352, 105.8346, 0, GETUTCDATE(), GETUTCDATE(), 0);
END

PRINT 'Sample Companies inserted successfully.';
GO

PRINT '';
PRINT '========================================';
PRINT 'Org Sample Data Inserted Successfully!';
PRINT '========================================';
PRINT 'Companies: 3';
PRINT '  - FPT Software (Verified)';
PRINT '  - VNG Corporation (Verified)';
PRINT '  - Startup Tech Hub (Not Verified)';
PRINT '';

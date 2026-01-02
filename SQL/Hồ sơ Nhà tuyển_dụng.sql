USE PartTimeJobs;
GO

-- Doanh nghiệp/cửa hàng
CREATE TABLE org.Companies (
    CompanyId     uniqueidentifier NOT NULL 
        CONSTRAINT PK_org_Companies PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    OwnerUserId   uniqueidentifier NOT NULL 
        CONSTRAINT FK_org_Companies_OwnerUser REFERENCES auth.Users(UserId),
    Name          nvarchar(200)    NOT NULL,
    IndustryId    uniqueidentifier NULL 
        CONSTRAINT FK_org_Companies_Industry REFERENCES jobs.Categories(CategoryId),
    Description   nvarchar(max)    NULL,
    WebsiteUrl    nvarchar(512)    NULL,
    LogoFileId    uniqueidentifier NULL 
        CONSTRAINT FK_org_Companies_Logo REFERENCES core.Files(FileId),
    EmailPublic   nvarchar(256)    NULL,
    PhonePublic   nvarchar(32)     NULL,
    AddressLine1  nvarchar(300)    NULL,
    Ward          nvarchar(100)    NULL,
    District      nvarchar(100)    NULL,
    City          nvarchar(100)    NULL,
    Province      nvarchar(100)    NULL,
    PostalCode    nvarchar(20)     NULL,
    Latitude      float            NULL CONSTRAINT CK_org_Companies_Lat CHECK (Latitude BETWEEN -90 AND 90),
    Longitude     float            NULL CONSTRAINT CK_org_Companies_Lon CHECK (Longitude BETWEEN -180 AND 180),
    Location      AS (CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL 
                      THEN geography::Point(Latitude, Longitude, 4326) END) PERSISTED,
    Verification  tinyint          NOT NULL DEFAULT 0, -- 0=Chưa xác minh,1=Đang xét,2=Đã xác minh,3=Từ chối
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted     bit              NOT NULL DEFAULT 0,
    RowVer        rowversion       NOT NULL
);
CREATE SPATIAL INDEX SIDX_org_Companies_Location ON org.Companies(Location);

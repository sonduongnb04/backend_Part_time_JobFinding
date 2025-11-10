CREATE TABLE org.CompanyRegistrationRequests (
    RequestId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RequestedByUserId UNIQUEIDENTIFIER NOT NULL, -- User gửi yêu cầu
    
    -- Thông tin company chờ duyệt (copy từ CreateCompanyRequest)
    CompanyName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    WebsiteUrl NVARCHAR(500),
    EmailPublic NVARCHAR(256),
    PhonePublic NVARCHAR(32),
    AddressLine1 NVARCHAR(500),
    Ward NVARCHAR(100),
    District NVARCHAR(100),
    City NVARCHAR(100),
    Province NVARCHAR(100),
    PostalCode NVARCHAR(20),
    Latitude FLOAT,
    Longitude FLOAT,
    
    -- Status workflow
    Status TINYINT NOT NULL DEFAULT 0, 
    -- 0: Pending, 1: Approved, 2: Rejected, 3: Cancelled
    
    -- Metadata
    RequestedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReviewedByUserId UNIQUEIDENTIFIER, -- Admin duyệt
    ReviewedAt DATETIME2,
    ReviewNote NVARCHAR(1000), -- Lý do approve/reject
    
    -- Kết quả sau khi approve
    CreatedCompanyId UNIQUEIDENTIFIER, -- ID company được tạo ra
    
    FOREIGN KEY (RequestedByUserId) REFERENCES auth.Users(UserId),
    FOREIGN KEY (ReviewedByUserId) REFERENCES auth.Users(UserId)
);
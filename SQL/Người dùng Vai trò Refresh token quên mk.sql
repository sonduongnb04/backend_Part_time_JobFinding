USE PartTimeJobs;
GO

-- Người dùng
CREATE TABLE auth.Users (
    UserId            uniqueidentifier NOT NULL 
        CONSTRAINT PK_auth_Users PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Email             nvarchar(256)    NOT NULL,
    NormalizedEmail   nvarchar(256)    NOT NULL,
    PhoneNumber       nvarchar(32)     NULL,
    FullName          nvarchar(200)    NULL,
    PasswordHash      nvarchar(255)    NOT NULL,  -- lưu chuỗi hash theo PHC (argon2/bcrypt)
    IsEmailVerified   bit              NOT NULL DEFAULT 0,
    IsPhoneVerified   bit              NOT NULL DEFAULT 0,
    IsActive          bit              NOT NULL DEFAULT 1,
    AvatarFileId      uniqueidentifier NULL,
    CreatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted         bit              NOT NULL DEFAULT 0,
    RowVer            rowversion       NOT NULL
);
CREATE UNIQUE INDEX UX_auth_Users_Email ON auth.Users(NormalizedEmail) WHERE IsDeleted = 0;

-- Vai trò & gán vai trò (RBAC đơn giản)
CREATE TABLE auth.Roles (
    RoleId        uniqueidentifier NOT NULL 
        CONSTRAINT PK_auth_Roles PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Code          varchar(32)      NOT NULL UNIQUE, -- ADMIN, EMPLOYER, STUDENT
    Name          nvarchar(64)     NOT NULL
);
INSERT INTO auth.Roles(Code,Name) VALUES ('ADMIN',N'Quản trị'),('EMPLOYER',N'Nhà tuyển dụng'),('STUDENT',N'Sinh viên');

CREATE TABLE auth.UserRoles (
    UserId        uniqueidentifier NOT NULL 
        CONSTRAINT FK_auth_UserRoles_User REFERENCES auth.Users(UserId),
    RoleId        uniqueidentifier NOT NULL 
        CONSTRAINT FK_auth_UserRoles_Role REFERENCES auth.Roles(RoleId),
    AssignedAt    datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_auth_UserRoles PRIMARY KEY(UserId, RoleId)
);

-- Refresh token (JWT)
CREATE TABLE auth.RefreshTokens (
    TokenId       uniqueidentifier NOT NULL 
        CONSTRAINT PK_auth_RefreshTokens PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId        uniqueidentifier NOT NULL 
        CONSTRAINT FK_auth_RefreshTokens_User REFERENCES auth.Users(UserId),
    Token         nvarchar(500)    NOT NULL,
    ExpiresAt     datetime2(0)     NOT NULL,
    RevokedAt     datetime2(0)     NULL,
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

-- OTP/Quên mật khẩu
CREATE TABLE auth.PasswordResetTokens (
    Id            uniqueidentifier NOT NULL 
        CONSTRAINT PK_auth_PasswordResetTokens PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId        uniqueidentifier NOT NULL 
        CONSTRAINT FK_auth_PasswordResetTokens_User REFERENCES auth.Users(UserId),
    Code          nvarchar(16)     NOT NULL,
    ExpiresAt     datetime2(0)     NOT NULL,
    UsedAt        datetime2(0)     NULL,
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_auth_PasswordResetTokens_User ON auth.PasswordResetTokens(UserId);

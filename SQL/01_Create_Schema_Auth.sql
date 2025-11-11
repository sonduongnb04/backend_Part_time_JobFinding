-- =============================================
-- Create Schema: auth
-- Description: Authentication and authorization tables
-- =============================================

-- Create schema auth if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'auth')
BEGIN
    EXEC('CREATE SCHEMA auth');
END
GO

-- =============================================
-- Table: auth.Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[auth].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [auth].[Users] (
        [UserId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Email] NVARCHAR(256) NOT NULL,
        [NormalizedEmail] NVARCHAR(256) NOT NULL,
        [PhoneNumber] NVARCHAR(32) NULL,
        [FullName] NVARCHAR(200) NULL,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        [IsPhoneVerified] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [AvatarFileId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVer] ROWVERSION NOT NULL,

        CONSTRAINT [PK_auth_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
    );

    -- Unique index on NormalizedEmail where IsDeleted = 0
    CREATE UNIQUE NONCLUSTERED INDEX [UX_auth_Users_Email]
        ON [auth].[Users]([NormalizedEmail] ASC)
        WHERE [IsDeleted] = 0;
END
GO

-- =============================================
-- Table: auth.Roles
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[auth].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [auth].[Roles] (
        [RoleId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Code] NVARCHAR(32) NOT NULL,
        [Name] NVARCHAR(64) NOT NULL,

        CONSTRAINT [PK_auth_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)
    );

    -- Unique index on Code
    CREATE UNIQUE NONCLUSTERED INDEX [UX_auth_Roles_Code]
        ON [auth].[Roles]([Code] ASC);
END
GO

-- =============================================
-- Table: auth.UserRoles
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[auth].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [auth].[UserRoles] (
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_auth_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId])
            REFERENCES [auth].[Roles]([RoleId]) ON DELETE CASCADE
    );
END
GO

-- =============================================
-- Table: auth.RefreshTokens
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[auth].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [auth].[RefreshTokens] (
        [TokenId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Token] NVARCHAR(500) NOT NULL,
        [ExpiresAt] DATETIME2 NOT NULL,
        [RevokedAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_auth_RefreshTokens] PRIMARY KEY CLUSTERED ([TokenId] ASC),
        CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION
    );

    -- Index on UserId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId]
        ON [auth].[RefreshTokens]([UserId] ASC);
END
GO

PRINT 'Schema auth and tables created successfully';

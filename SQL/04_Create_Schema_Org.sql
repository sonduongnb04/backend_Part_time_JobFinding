-- =============================================
-- Create Schema: org
-- Description: Organization/company related tables
-- =============================================

-- Create schema org if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'org')
BEGIN
    EXEC('CREATE SCHEMA org');
END
GO

-- =============================================
-- Table: org.Companies
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[org].[Companies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [org].[Companies] (
        [CompanyId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [OwnerUserId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [IndustryId] UNIQUEIDENTIFIER NULL,
        [Description] NVARCHAR(MAX) NULL,
        [WebsiteUrl] NVARCHAR(500) NULL,
        [LogoFileId] UNIQUEIDENTIFIER NULL,
        [EmailPublic] NVARCHAR(256) NULL,
        [PhonePublic] NVARCHAR(32) NULL,
        [AddressLine1] NVARCHAR(200) NULL,
        [Ward] NVARCHAR(100) NULL,
        [District] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Province] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [Latitude] FLOAT NULL,
        [Longitude] FLOAT NULL,
        [Verification] TINYINT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVer] ROWVERSION NOT NULL,

        CONSTRAINT [PK_org_Companies] PRIMARY KEY CLUSTERED ([CompanyId] ASC),
        CONSTRAINT [FK_Companies_Users] FOREIGN KEY ([OwnerUserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Companies_LogoFile] FOREIGN KEY ([LogoFileId])
            REFERENCES [core].[Files]([FileId]) ON DELETE NO ACTION
    );

    -- Index on OwnerUserId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_Companies_OwnerUserId]
        ON [org].[Companies]([OwnerUserId] ASC);

    -- Index on IsDeleted for filtering
    CREATE NONCLUSTERED INDEX [IX_Companies_IsDeleted]
        ON [org].[Companies]([IsDeleted] ASC);
END
GO

-- =============================================
-- Table: org.CompanyRegistrationRequests
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[org].[CompanyRegistrationRequests]') AND type in (N'U'))
BEGIN
    CREATE TABLE [org].[CompanyRegistrationRequests] (
        [RequestId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [RequestedByUserId] UNIQUEIDENTIFIER NOT NULL,

        -- Company information
        [CompanyName] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [WebsiteUrl] NVARCHAR(500) NULL,
        [EmailPublic] NVARCHAR(256) NULL,
        [PhonePublic] NVARCHAR(32) NULL,
        [AddressLine1] NVARCHAR(200) NULL,
        [Ward] NVARCHAR(100) NULL,
        [District] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Province] NVARCHAR(100) NULL,
        [PostalCode] NVARCHAR(20) NULL,
        [Latitude] FLOAT NULL,
        [Longitude] FLOAT NULL,

        -- Workflow
        [Status] TINYINT NOT NULL DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected
        [RequestedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ReviewedByUserId] UNIQUEIDENTIFIER NULL,
        [ReviewedAt] DATETIME2 NULL,
        [ReviewNote] NVARCHAR(MAX) NULL,
        [CreatedCompanyId] UNIQUEIDENTIFIER NULL,

        CONSTRAINT [PK_org_CompanyRegistrationRequests] PRIMARY KEY CLUSTERED ([RequestId] ASC),
        CONSTRAINT [FK_CompanyRequests_RequestedByUser] FOREIGN KEY ([RequestedByUserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompanyRequests_ReviewedByUser] FOREIGN KEY ([ReviewedByUserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompanyRequests_CreatedCompany] FOREIGN KEY ([CreatedCompanyId])
            REFERENCES [org].[Companies]([CompanyId]) ON DELETE NO ACTION
    );

    -- Index on RequestedByUserId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_CompanyRequests_RequestedByUserId]
        ON [org].[CompanyRegistrationRequests]([RequestedByUserId] ASC);

    -- Index on Status for filtering
    CREATE NONCLUSTERED INDEX [IX_CompanyRequests_Status]
        ON [org].[CompanyRegistrationRequests]([Status] ASC);
END
GO

PRINT 'Schema org and tables created successfully';

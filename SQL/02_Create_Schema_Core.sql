-- =============================================
-- Create Schema: core
-- Description: Core system tables (file storage, etc.)
-- =============================================

-- Create schema core if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'core')
BEGIN
    EXEC('CREATE SCHEMA core');
END
GO

-- =============================================
-- Table: core.Files
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[Files]') AND type in (N'U'))
BEGIN
    CREATE TABLE [core].[Files] (
        [FileId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [FileName] NVARCHAR(255) NOT NULL,
        [ContentType] NVARCHAR(100) NOT NULL,
        [ByteSize] BIGINT NOT NULL,
        [StorageUrl] NVARCHAR(500) NOT NULL,
        [StorageProvider] NVARCHAR(50) NOT NULL,
        [Checksum] NVARCHAR(100) NULL,
        [OwnerUserId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,

        CONSTRAINT [PK_core_Files] PRIMARY KEY CLUSTERED ([FileId] ASC),
        CONSTRAINT [FK_Files_Users] FOREIGN KEY ([OwnerUserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION
    );

    -- Index on OwnerUserId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_Files_OwnerUserId]
        ON [core].[Files]([OwnerUserId] ASC);

    -- Index on IsDeleted for filtering
    CREATE NONCLUSTERED INDEX [IX_Files_IsDeleted]
        ON [core].[Files]([IsDeleted] ASC);
END
GO

PRINT 'Schema core and table Files created successfully';

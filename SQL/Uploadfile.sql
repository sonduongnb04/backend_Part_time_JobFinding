USE PartTimeJobs;
GO

CREATE TABLE core.Files (
    FileId        uniqueidentifier NOT NULL 
        CONSTRAINT PK_core_Files PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    FileName      nvarchar(260)    NOT NULL,
    ContentType   nvarchar(128)    NOT NULL,
    ByteSize      bigint           NOT NULL,
    StorageUrl    nvarchar(1000)   NOT NULL,   -- URL trong Object Storage
    StorageProvider nvarchar(64)   NOT NULL,   -- e.g., 'AzureBlob','S3','Local'
    Checksum      varchar(128)     NULL,
    OwnerUserId   uniqueidentifier NULL
        CONSTRAINT FK_core_Files_OwnerUser REFERENCES auth.Users(UserId),
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted     bit              NOT NULL DEFAULT 0
);

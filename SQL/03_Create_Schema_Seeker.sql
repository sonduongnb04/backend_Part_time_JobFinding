-- =============================================
-- Create Schema: seeker
-- Description: Student/job seeker profile tables
-- =============================================

-- Create schema seeker if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'seeker')
BEGIN
    EXEC('CREATE SCHEMA seeker');
END
GO

-- =============================================
-- Table: seeker.Profiles
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[seeker].[Profiles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [seeker].[Profiles] (
        [ProfileId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [StudentUserId] UNIQUEIDENTIFIER NOT NULL,
        [FirstName] NVARCHAR(100) NULL,
        [LastName] NVARCHAR(100) NULL,
        [DateOfBirth] DATE NULL,
        [Gender] NVARCHAR(20) NULL,
        [Email] NVARCHAR(256) NULL,
        [PhoneNumber] NVARCHAR(32) NULL,
        [AddressLine1] NVARCHAR(200) NULL,
        [Ward] NVARCHAR(100) NULL,
        [District] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Province] NVARCHAR(100) NULL,
        [University] NVARCHAR(200) NULL,
        [Major] NVARCHAR(200) NULL,
        [StudentId] NVARCHAR(50) NULL,
        [GraduationYear] INT NULL,
        [GPA] DECIMAL(3, 2) NULL,
        [AvatarFileId] UNIQUEIDENTIFIER NULL,
        [ResumeFileId] UNIQUEIDENTIFIER NULL,
        [Bio] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVer] ROWVERSION NOT NULL,

        CONSTRAINT [PK_seeker_Profiles] PRIMARY KEY CLUSTERED ([ProfileId] ASC),
        CONSTRAINT [FK_Profiles_Users] FOREIGN KEY ([StudentUserId])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Profiles_AvatarFile] FOREIGN KEY ([AvatarFileId])
            REFERENCES [core].[Files]([FileId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Profiles_ResumeFile] FOREIGN KEY ([ResumeFileId])
            REFERENCES [core].[Files]([FileId]) ON DELETE NO ACTION
    );

    -- Unique index on StudentUserId
    CREATE UNIQUE NONCLUSTERED INDEX [UX_Profiles_StudentUserId]
        ON [seeker].[Profiles]([StudentUserId] ASC);
END
GO

-- =============================================
-- Table: seeker.ProfileSkills
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[seeker].[ProfileSkills]') AND type in (N'U'))
BEGIN
    CREATE TABLE [seeker].[ProfileSkills] (
        [ProfileId] UNIQUEIDENTIFIER NOT NULL,
        [SkillId] UNIQUEIDENTIFIER NOT NULL,
        [ProficiencyLevel] TINYINT NULL,

        CONSTRAINT [PK_seeker_ProfileSkills] PRIMARY KEY CLUSTERED ([ProfileId] ASC, [SkillId] ASC),
        CONSTRAINT [FK_ProfileSkills_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [seeker].[Profiles]([ProfileId]) ON DELETE CASCADE
    );
END
GO

-- =============================================
-- Table: seeker.ProfileExperiences
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[seeker].[ProfileExperiences]') AND type in (N'U'))
BEGIN
    CREATE TABLE [seeker].[ProfileExperiences] (
        [ExperienceId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [ProfileId] UNIQUEIDENTIFIER NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [CompanyName] NVARCHAR(200) NULL,
        [StartDate] DATE NULL,
        [EndDate] DATE NULL,
        [IsCurrent] BIT NOT NULL DEFAULT 0,
        [Description] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_seeker_ProfileExperiences] PRIMARY KEY CLUSTERED ([ExperienceId] ASC),
        CONSTRAINT [FK_ProfileExperiences_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [seeker].[Profiles]([ProfileId]) ON DELETE CASCADE
    );

    -- Index on ProfileId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_ProfileExperiences_ProfileId]
        ON [seeker].[ProfileExperiences]([ProfileId] ASC);
END
GO

-- =============================================
-- Table: seeker.ProfileEducations
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[seeker].[ProfileEducations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [seeker].[ProfileEducations] (
        [EducationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [ProfileId] UNIQUEIDENTIFIER NOT NULL,
        [School] NVARCHAR(200) NOT NULL,
        [Degree] NVARCHAR(100) NULL,
        [FieldOfStudy] NVARCHAR(200) NULL,
        [StartDate] DATE NULL,
        [EndDate] DATE NULL,
        [GPA] DECIMAL(3, 2) NULL,
        [CertificateFileId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_seeker_ProfileEducations] PRIMARY KEY CLUSTERED ([EducationId] ASC),
        CONSTRAINT [FK_ProfileEducations_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [seeker].[Profiles]([ProfileId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProfileEducations_CertificateFile] FOREIGN KEY ([CertificateFileId])
            REFERENCES [core].[Files]([FileId]) ON DELETE NO ACTION
    );

    -- Index on ProfileId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_ProfileEducations_ProfileId]
        ON [seeker].[ProfileEducations]([ProfileId] ASC);
END
GO

-- =============================================
-- Table: seeker.ProfileCertificates
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[seeker].[ProfileCertificates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [seeker].[ProfileCertificates] (
        [CertificateId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [ProfileId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [IssuedBy] NVARCHAR(200) NULL,
        [IssuedDate] DATE NULL,
        [ExpiryDate] DATE NULL,
        [CertificateFileId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_seeker_ProfileCertificates] PRIMARY KEY CLUSTERED ([CertificateId] ASC),
        CONSTRAINT [FK_ProfileCertificates_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [seeker].[Profiles]([ProfileId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProfileCertificates_CertificateFile] FOREIGN KEY ([CertificateFileId])
            REFERENCES [core].[Files]([FileId]) ON DELETE NO ACTION
    );

    -- Index on ProfileId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_ProfileCertificates_ProfileId]
        ON [seeker].[ProfileCertificates]([ProfileId] ASC);
END
GO

PRINT 'Schema seeker and tables created successfully';

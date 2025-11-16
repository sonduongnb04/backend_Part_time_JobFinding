-- =============================================
-- Create Schema: jobs
-- Description: Job posting and related tables
-- =============================================

-- Create schema jobs if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'jobs')
BEGIN
    EXEC('CREATE SCHEMA jobs');
END
GO

-- =============================================
-- Table: jobs.JobPosts
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[jobs].[JobPosts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [jobs].[JobPosts] (
        [JobPostId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [CompanyId] UNIQUEIDENTIFIER NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [CategoryId] UNIQUEIDENTIFIER NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Requirements] NVARCHAR(MAX) NULL,
        [Benefits] NVARCHAR(MAX) NULL,
        [StatusId] TINYINT NOT NULL,
        [SalaryMin] DECIMAL(12, 2) NULL,
        [SalaryMax] DECIMAL(12, 2) NULL,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'VND',
        [SalaryUnitId] TINYINT NOT NULL,
        [ArrangementId] TINYINT NOT NULL,
        [AddressLine1] NVARCHAR(200) NULL,
        [Ward] NVARCHAR(100) NULL,
        [District] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Province] NVARCHAR(100) NULL,
        [Latitude] FLOAT NULL,
        [Longitude] FLOAT NULL,
        [Slots] INT NULL,
        [PublishAt] DATETIME2 NULL,
        [ExpireAt] DATETIME2 NULL,
        [ViewCount] INT NOT NULL DEFAULT 0,
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [RowVer] ROWVERSION NOT NULL,

        CONSTRAINT [PK_jobs_JobPosts] PRIMARY KEY CLUSTERED ([JobPostId] ASC),
        CONSTRAINT [FK_JobPosts_Companies] FOREIGN KEY ([CompanyId])
            REFERENCES [org].[Companies]([CompanyId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JobPosts_Creator] FOREIGN KEY ([CreatedBy])
            REFERENCES [auth].[Users]([UserId]) ON DELETE NO ACTION
    );

    -- Index on CompanyId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_JobPosts_CompanyId]
        ON [jobs].[JobPosts]([CompanyId] ASC);

    -- Index on StatusId for filtering
    CREATE NONCLUSTERED INDEX [IX_JobPosts_StatusId]
        ON [jobs].[JobPosts]([StatusId] ASC);

    -- Index on IsDeleted for filtering
    CREATE NONCLUSTERED INDEX [IX_JobPosts_IsDeleted]
        ON [jobs].[JobPosts]([IsDeleted] ASC);

    -- Index on PublishAt and ExpireAt for filtering active jobs
    CREATE NONCLUSTERED INDEX [IX_JobPosts_PublishExpire]
        ON [jobs].[JobPosts]([PublishAt] ASC, [ExpireAt] ASC);
END
GO

-- =============================================
-- Table: jobs.JobShifts
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[jobs].[JobShifts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [jobs].[JobShifts] (
        [JobShiftId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [JobPostId] UNIQUEIDENTIFIER NOT NULL,
        [ShiftName] NVARCHAR(100) NULL,
        [DayOfWeek] TINYINT NULL,
        [StartTime] TIME NULL,
        [EndTime] TIME NULL,
        [Note] NVARCHAR(500) NULL,

        CONSTRAINT [PK_jobs_JobShifts] PRIMARY KEY CLUSTERED ([JobShiftId] ASC),
        CONSTRAINT [FK_JobShifts_JobPosts] FOREIGN KEY ([JobPostId])
            REFERENCES [jobs].[JobPosts]([JobPostId]) ON DELETE CASCADE
    );

    -- Index on JobPostId for faster lookups
    CREATE NONCLUSTERED INDEX [IX_JobShifts_JobPostId]
        ON [jobs].[JobShifts]([JobPostId] ASC);
END
GO

-- =============================================
-- Table: jobs.JobPostSkills
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[jobs].[JobPostSkills]') AND type in (N'U'))
BEGIN
    CREATE TABLE [jobs].[JobPostSkills] (
        [JobPostId] UNIQUEIDENTIFIER NOT NULL,
        [SkillId] UNIQUEIDENTIFIER NOT NULL,

        CONSTRAINT [PK_jobs_JobPostSkills] PRIMARY KEY CLUSTERED ([JobPostId] ASC, [SkillId] ASC),
        CONSTRAINT [FK_JobPostSkills_JobPosts] FOREIGN KEY ([JobPostId])
            REFERENCES [jobs].[JobPosts]([JobPostId]) ON DELETE CASCADE
    );
END
GO

PRINT 'Schema jobs and tables created successfully';

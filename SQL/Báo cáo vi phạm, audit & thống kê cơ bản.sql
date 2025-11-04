USE PartTimeJobs;
GO

-- Báo cáo vi phạm
CREATE TABLE ops.Reports (
    ReportId      uniqueidentifier NOT NULL 
        CONSTRAINT PK_ops_Reports PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ReporterUserId uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_Reports_Reporter REFERENCES auth.Users(UserId),
    TargetType     varchar(32)     NOT NULL,  -- JOB_POST / USER / MESSAGE
    TargetId       uniqueidentifier NOT NULL, -- Id thực thể bị báo cáo
    ReasonId       tinyint         NOT NULL 
        CONSTRAINT FK_ops_Reports_Reason REFERENCES ops.ReportReason(ReasonId),
    Description    nvarchar(1000)  NULL,
    StatusId       tinyint         NOT NULL 
        CONSTRAINT FK_ops_Reports_Status REFERENCES ops.ReportStatus(StatusId),
    HandledBy      uniqueidentifier NULL 
        CONSTRAINT FK_ops_Reports_Admin REFERENCES auth.Users(UserId),
    HandledAt      datetime2(0)    NULL,
    ResolutionNote nvarchar(1000)  NULL,
    CreatedAt      datetime2(0)    NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_ops_Reports_Status ON ops.Reports(StatusId, CreatedAt DESC);

-- Log hành động (audit)
CREATE TABLE ops.AuditLogs (
    AuditId       uniqueidentifier NOT NULL 
        CONSTRAINT PK_ops_AuditLogs PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId        uniqueidentifier NULL 
        CONSTRAINT FK_ops_AuditLogs_User REFERENCES auth.Users(UserId),
    Action        nvarchar(128)    NOT NULL,  -- e.g., 'JOB_POST_CREATE'
    EntityType    nvarchar(64)     NULL,
    EntityId      uniqueidentifier NULL,
    MetaData      nvarchar(max)    NULL,      -- JSON
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Lượt xem tin (để thống kê)
CREATE TABLE ops.JobPostViews (
    ViewId        bigint IDENTITY(1,1) NOT NULL 
        CONSTRAINT PK_ops_JobPostViews PRIMARY KEY,
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_JobPostViews_JobPost REFERENCES jobs.JobPosts(JobPostId),
    UserId        uniqueidentifier NULL 
        CONSTRAINT FK_ops_JobPostViews_User REFERENCES auth.Users(UserId),
    Source        varchar(16)      NULL,      -- web/mobile
    ViewedAt      datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_ops_JobPostViews_JobPost ON ops.JobPostViews(JobPostId, ViewedAt DESC);

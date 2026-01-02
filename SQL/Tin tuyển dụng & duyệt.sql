USE PartTimeJobs;
GO

-- Tin tuyển dụng
CREATE TABLE jobs.JobPosts (
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_JobPosts PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    CompanyId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_JobPosts_Company REFERENCES org.Companies(CompanyId),
    Title         nvarchar(250)    NOT NULL,
    CategoryId    uniqueidentifier NULL 
        CONSTRAINT FK_jobs_JobPosts_Category REFERENCES jobs.Categories(CategoryId),
    Description   nvarchar(max)    NOT NULL,   -- JD chi tiết (có thể lưu HTML/markdown)
    Requirements  nvarchar(max)    NULL,
    Benefits      nvarchar(max)    NULL,
    StatusId      tinyint          NOT NULL 
        CONSTRAINT FK_jobs_JobPosts_Status REFERENCES ops.JobPostStatus(StatusId),
    SalaryMin     decimal(12,2)    NULL,
    SalaryMax     decimal(12,2)    NULL,
    Currency      char(3)          NOT NULL DEFAULT 'VND',
    SalaryUnitId  tinyint          NOT NULL 
        CONSTRAINT FK_jobs_JobPosts_SalaryUnit REFERENCES jobs.SalaryUnit(UnitId),
    ArrangementId tinyint          NOT NULL 
        CONSTRAINT FK_jobs_JobPosts_Arrangement REFERENCES jobs.WorkArrangement(ArrangementId),
    AddressLine1  nvarchar(300)    NULL,
    Ward          nvarchar(100)    NULL,
    District      nvarchar(100)    NULL,
    City          nvarchar(100)    NULL,
    Province      nvarchar(100)    NULL,
    Latitude      float            NULL CONSTRAINT CK_jobs_JobPosts_Lat CHECK (Latitude BETWEEN -90 AND 90),
    Longitude     float            NULL CONSTRAINT CK_jobs_JobPosts_Lon CHECK (Longitude BETWEEN -180 AND 180),
    Location      AS (CASE WHEN Latitude IS NOT NULL AND Longitude IS NOT NULL 
                      THEN geography::Point(Latitude, Longitude, 4326) END) PERSISTED,
    Slots         int              NULL,      -- số lượng cần tuyển
    PublishAt     datetime2(0)     NULL,
    ExpireAt      datetime2(0)     NULL,
    ViewCount     int              NOT NULL DEFAULT 0,
    CreatedBy     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_JobPosts_CreatedBy REFERENCES auth.Users(UserId),
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted     bit              NOT NULL DEFAULT 0,
    RowVer        rowversion       NOT NULL,
    CONSTRAINT CK_jobs_JobPosts_SalaryRange CHECK (SalaryMin IS NULL OR SalaryMax IS NULL OR SalaryMin <= SalaryMax)
);
CREATE INDEX IX_jobs_JobPosts_Status_Created ON jobs.JobPosts(StatusId, CreatedAt DESC);
CREATE SPATIAL INDEX SIDX_jobs_JobPosts_Location ON jobs.JobPosts(Location);

-- Ca làm việc cho tin tuyển dụng
CREATE TABLE jobs.JobShifts (
    JobShiftId    uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_JobShifts PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_JobShifts_JobPost REFERENCES jobs.JobPosts(JobPostId),
    ShiftName     nvarchar(100)    NULL,      -- ví dụ: Ca tối
    DayOfWeek     tinyint          NULL,      -- 0=CN ... 6=Thứ 7; NULL = linh hoạt
    StartTime     time(0)          NULL,
    EndTime       time(0)          NULL,
    Note          nvarchar(200)    NULL,
    CONSTRAINT CK_jobs_JobShifts_DOW CHECK (DayOfWeek IS NULL OR DayOfWeek BETWEEN 0 AND 6)
);

-- Kỹ năng yêu cầu cho tin
CREATE TABLE jobs.JobPostSkills (
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_JobPostSkills_JobPost REFERENCES jobs.JobPosts(JobPostId),
    SkillId       uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_JobPostSkills_Skill REFERENCES core.Skills(SkillId),
    PRIMARY KEY (JobPostId, SkillId)
);

-- Lịch sử kiểm duyệt tin
CREATE TABLE ops.JobModerationHistory (
    ModerationId  uniqueidentifier NOT NULL 
        CONSTRAINT PK_ops_JobModerationHistory PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_JobModerationHistory_JobPost REFERENCES jobs.JobPosts(JobPostId),
    OldStatusId   tinyint          NOT NULL 
        CONSTRAINT FK_ops_JobModerationHistory_OldStatus REFERENCES ops.JobPostStatus(StatusId),
    NewStatusId   tinyint          NOT NULL 
        CONSTRAINT FK_ops_JobModerationHistory_NewStatus REFERENCES ops.JobPostStatus(StatusId),
    Reason        nvarchar(500)    NULL,
    ActionBy      uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_JobModerationHistory_Admin REFERENCES auth.Users(UserId),
    ActionAt      datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

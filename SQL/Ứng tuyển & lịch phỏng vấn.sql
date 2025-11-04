USE PartTimeJobs;
GO

-- Ứng tuyển
CREATE TABLE jobs.Applications (
    ApplicationId uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_Applications PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    JobPostId     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_Applications_JobPost REFERENCES jobs.JobPosts(JobPostId),
    StudentUserId uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_Applications_Student REFERENCES auth.Users(UserId),
    ProfileId     uniqueidentifier NULL 
        CONSTRAINT FK_jobs_Applications_Profile REFERENCES seeker.Profiles(ProfileId),
    CVFileId      uniqueidentifier NULL 
        CONSTRAINT FK_jobs_Applications_CVFile REFERENCES core.Files(FileId),
    CoverLetter   nvarchar(max)    NULL,
    StatusId      tinyint          NOT NULL 
        CONSTRAINT FK_jobs_Applications_Status REFERENCES jobs.ApplicationStatus(StatusId),
    AppliedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UNIQUE (JobPostId, StudentUserId)  -- 1 SV chỉ ứng tuyển 1 lần / 1 tin
);

-- Lịch sử trạng thái ứng tuyển
CREATE TABLE jobs.ApplicationHistory (
    HistoryId     uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_ApplicationHistory PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ApplicationId uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_ApplicationHistory_Application REFERENCES jobs.Applications(ApplicationId),
    OldStatusId   tinyint          NOT NULL 
        CONSTRAINT FK_jobs_ApplicationHistory_OldStatus REFERENCES jobs.ApplicationStatus(StatusId),
    NewStatusId   tinyint          NOT NULL 
        CONSTRAINT FK_jobs_ApplicationHistory_NewStatus REFERENCES jobs.ApplicationStatus(StatusId),
    Note          nvarchar(500)    NULL,
    ChangedBy     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_ApplicationHistory_User REFERENCES auth.Users(UserId),
    ChangedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Lịch phỏng vấn
CREATE TABLE jobs.Interviews (
    InterviewId   uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_Interviews PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ApplicationId uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_Interviews_Application REFERENCES jobs.Applications(ApplicationId),
    InterviewTime datetime2(0)     NOT NULL,
    Method        nvarchar(50)     NULL,       -- Trực tiếp/Online/Điện thoại...
    LocationText  nvarchar(300)    NULL,       -- Địa điểm (nếu trực tiếp)
    Note          nvarchar(500)    NULL,
    CreatedBy     uniqueidentifier NOT NULL 
        CONSTRAINT FK_jobs_Interviews_CreatedBy REFERENCES auth.Users(UserId),
    CreatedAt     datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

USE PartTimeJobs;
GO

-- Gợi ý công việc cho SV (AI)
CREATE TABLE ops.AI_JobRecommendations (
    RecommendationId uniqueidentifier NOT NULL 
        CONSTRAINT PK_ops_AI_JobRecommendations PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId           uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_AI_JobRecommendations_User REFERENCES auth.Users(UserId),
    GeneratedAt      datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    Context          nvarchar(max)    NULL,  -- JSON: profile snapshot, location, filters
    Result           nvarchar(max)    NULL   -- JSON: danh sách JobPostId + điểm phù hợp
);

-- Soạn thảo JD (AI) cho NTD
CREATE TABLE ops.AI_JDGenerations (
    GenerationId   uniqueidentifier NOT NULL 
        CONSTRAINT PK_ops_AI_JDGenerations PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    CompanyId      uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_AI_JDGenerations_Company REFERENCES org.Companies(CompanyId),
    Prompt         nvarchar(max)    NULL,
    OutputJD       nvarchar(max)    NULL,
    GeneratedAt    datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy      uniqueidentifier NOT NULL 
        CONSTRAINT FK_ops_AI_JDGenerations_User REFERENCES auth.Users(UserId)
);

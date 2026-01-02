USE PartTimeJobs;
GO

-- Hồ sơ sinh viên
CREATE TABLE seeker.Profiles (
    ProfileId         uniqueidentifier NOT NULL
        CONSTRAINT PK_seeker_Profiles PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    StudentUserId     uniqueidentifier NOT NULL UNIQUE
        CONSTRAINT FK_seeker_Profiles_Student REFERENCES auth.Users(UserId),

    -- Thông tin cá nhân
    FirstName         nvarchar(100)    NULL,
    LastName          nvarchar(100)    NULL,
    DateOfBirth       date             NULL,
    Gender            nvarchar(20)     NULL,

    -- Liên hệ
    Email             nvarchar(256)    NULL,
    PhoneNumber       nvarchar(32)     NULL,

    -- Địa chỉ
    AddressLine1      nvarchar(300)    NULL,
    Ward              nvarchar(100)    NULL,
    District          nvarchar(100)    NULL,
    City              nvarchar(100)    NULL,
    Province          nvarchar(100)    NULL,

    -- Học vấn
    University        nvarchar(200)    NULL,
    Major             nvarchar(200)    NULL,
    StudentId         nvarchar(50)     NULL,
    GraduationYear    int              NULL,
    GPA               decimal(3,2)     NULL,

    -- Upload Files
    AvatarFileId      uniqueidentifier NULL
        CONSTRAINT FK_seeker_Profiles_Avatar REFERENCES core.Files(FileId),
    ResumeFileId      uniqueidentifier NULL
        CONSTRAINT FK_seeker_Profiles_Resume REFERENCES core.Files(FileId),

    -- Giới thiệu bản thân
    Bio               nvarchar(max)    NULL,

    -- Metadata
    CreatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted         bit              NOT NULL DEFAULT 0,
    RowVer            rowversion       NOT NULL
);

-- Kỹ năng của sinh viên
CREATE TABLE seeker.ProfileSkills (
    ProfileId         uniqueidentifier NOT NULL
        CONSTRAINT FK_seeker_ProfileSkills_Profile REFERENCES seeker.Profiles(ProfileId),
    SkillId           uniqueidentifier NOT NULL
        CONSTRAINT FK_seeker_ProfileSkills_Skill REFERENCES core.Skills(SkillId),
    ProficiencyLevel  tinyint          NULL, -- 1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert
    CONSTRAINT PK_seeker_ProfileSkills PRIMARY KEY (ProfileId, SkillId)
);

-- Kinh nghiệm làm việc
CREATE TABLE seeker.ProfileExperiences (
    ExperienceId      uniqueidentifier NOT NULL
        CONSTRAINT PK_seeker_ProfileExperiences PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ProfileId         uniqueidentifier NOT NULL
        CONSTRAINT FK_seeker_ProfileExperiences_Profile REFERENCES seeker.Profiles(ProfileId),
    Title             nvarchar(200)    NOT NULL,
    CompanyName       nvarchar(200)    NULL,
    StartDate         date             NULL,
    EndDate           date             NULL,
    IsCurrent         bit              NOT NULL DEFAULT 0,
    Description       nvarchar(max)    NULL,
    CreatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Học vấn chi tiết
CREATE TABLE seeker.ProfileEducations (
    EducationId       uniqueidentifier NOT NULL
        CONSTRAINT PK_seeker_ProfileEducations PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ProfileId         uniqueidentifier NOT NULL
        CONSTRAINT FK_seeker_ProfileEducations_Profile REFERENCES seeker.Profiles(ProfileId),
    School            nvarchar(200)    NOT NULL,
    Degree            nvarchar(100)    NULL,
    FieldOfStudy      nvarchar(200)    NULL,
    StartDate         date             NULL,
    EndDate           date             NULL,
    GPA               decimal(3,2)     NULL,
    CertificateFileId uniqueidentifier NULL
        CONSTRAINT FK_seeker_ProfileEducations_Certificate REFERENCES core.Files(FileId),
    CreatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Chứng chỉ
CREATE TABLE seeker.ProfileCertificates (
    CertificateId     uniqueidentifier NOT NULL
        CONSTRAINT PK_seeker_ProfileCertificates PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ProfileId         uniqueidentifier NOT NULL
        CONSTRAINT FK_seeker_ProfileCertificates_Profile REFERENCES seeker.Profiles(ProfileId),
    Name              nvarchar(200)    NOT NULL,
    IssuedBy          nvarchar(200)    NULL,
    IssuedDate        date             NULL,
    ExpiryDate        date             NULL,
    CertificateFileId uniqueidentifier NULL
        CONSTRAINT FK_seeker_ProfileCertificates_File REFERENCES core.Files(FileId),
    CreatedAt         datetime2(0)     NOT NULL DEFAULT SYSUTCDATETIME()
);

USE PartTimeJobs;
GO

-- Trạng thái tin tuyển dụng
CREATE TABLE ops.JobPostStatus (
    StatusId      tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(32)   NOT NULL UNIQUE, -- DRAFT, PENDING, APPROVED, REJECTED, CLOSED
    Name          nvarchar(64)  NOT NULL
);
INSERT INTO ops.JobPostStatus(StatusId, Code, Name) VALUES
(0,'DRAFT',N'Nháp'),
(1,'PENDING',N'Chờ duyệt'),
(2,'APPROVED',N'Đã duyệt'),
(3,'REJECTED',N'Từ chối'),
(4,'CLOSED',N'Đã đóng');

-- Trạng thái hồ sơ ứng tuyển
CREATE TABLE jobs.ApplicationStatus (
    StatusId      tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(32)   NOT NULL UNIQUE, -- APPLIED, SHORTLISTED, INTERVIEW, HIRED, REJECTED, WITHDRAWN
    Name          nvarchar(64)  NOT NULL
);
INSERT INTO jobs.ApplicationStatus(StatusId, Code, Name) VALUES
(0,'APPLIED',N'Đã ứng tuyển'),
(1,'SHORTLISTED',N'Vào danh sách rút gọn'),
(2,'INTERVIEW',N'Hẹn phỏng vấn'),
(3,'HIRED',N'Nhận việc'),
(4,'REJECTED',N'Từ chối'),
(5,'WITHDRAWN',N'Ứng viên rút hồ sơ');

-- Đơn vị lương
CREATE TABLE jobs.SalaryUnit (
    UnitId        tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(16)   NOT NULL UNIQUE, -- HOUR, SHIFT, DAY, WEEK, MONTH
    Name          nvarchar(32)  NOT NULL
);
INSERT INTO jobs.SalaryUnit(UnitId, Code, Name) VALUES
(1,'HOUR',N'Theo giờ'),
(2,'SHIFT',N'Theo ca'),
(3,'DAY',N'Theo ngày'),
(4,'WEEK',N'Theo tuần'),
(5,'MONTH',N'Theo tháng');

-- Kiểu làm việc (onsite/hybrid/remote)
CREATE TABLE jobs.WorkArrangement (
    ArrangementId tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(16)   NOT NULL UNIQUE, -- ONSITE, HYBRID, REMOTE
    Name          nvarchar(32)  NOT NULL
);
INSERT INTO jobs.WorkArrangement(ArrangementId, Code, Name) VALUES
(1,'ONSITE',N'Tại chỗ'),
(2,'HYBRID',N'Kết hợp'),
(3,'REMOTE',N'Từ xa');

-- Lý do báo cáo vi phạm
CREATE TABLE ops.ReportReason (
    ReasonId      tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(32)   NOT NULL UNIQUE, -- SPAM, SCAM, INAPPROPRIATE, FAKE, OTHER
    Name          nvarchar(64)  NOT NULL
);
INSERT INTO ops.ReportReason(ReasonId, Code, Name) VALUES
(1,'SPAM',N'Spam'),
(2,'SCAM',N'Lừa đảo'),
(3,'INAPPROPRIATE',N'Nội dung không phù hợp'),
(4,'FAKE',N'Thông tin giả'),
(5,'OTHER',N'Khác');

-- Trạng thái xử lý báo cáo
CREATE TABLE ops.ReportStatus (
    StatusId      tinyint       NOT NULL PRIMARY KEY,
    Code          varchar(32)   NOT NULL UNIQUE, -- PENDING, RESOLVED, REJECTED
    Name          nvarchar(64)  NOT NULL
);
INSERT INTO ops.ReportStatus(StatusId, Code, Name) VALUES
(0,'PENDING',N'Chờ xử lý'),
(1,'RESOLVED',N'Đã xử lý'),
(2,'REJECTED',N'Bác báo cáo');

-- Ngành nghề / danh mục (có thể phân cấp)
CREATE TABLE jobs.Categories (
    CategoryId    uniqueidentifier NOT NULL 
        CONSTRAINT PK_jobs_Categories PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ParentId      uniqueidentifier NULL 
        CONSTRAINT FK_jobs_Categories_Parent REFERENCES jobs.Categories(CategoryId),
    Code          varchar(64)      NOT NULL UNIQUE,
    Name          nvarchar(128)    NOT NULL,
    IsActive      bit              NOT NULL DEFAULT 1
);

-- Kỹ năng
CREATE TABLE core.Skills (
    SkillId       uniqueidentifier NOT NULL 
        CONSTRAINT PK_core_Skills PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name          nvarchar(128)    NOT NULL UNIQUE
);

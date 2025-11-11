using PTJ.Auth;
using PTJ.Core;
using PTJ.Seeker;

namespace PTJ.Jobs;

/// Đơn ứng tuyển - Sinh viên nộp CV vào tin tuyển dụng
public class Application
{
    public Guid ApplicationId { get; set; }
    public Guid JobPostId { get; set; }
    public Guid StudentUserId { get; set; }
    public Guid? ProfileId { get; set; }
    public Guid? CVFileId { get; set; }
    public string? CoverLetter { get; set; }
    public byte StatusId { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public JobPost JobPost { get; set; } = default!;
    public User Student { get; set; } = default!;
    public Profile? Profile { get; set; }
    public FileEntity? CVFile { get; set; }
    public ApplicationStatus Status { get; set; } = default!;
    public ICollection<ApplicationHistory> ApplicationHistories { get; set; } = new List<ApplicationHistory>();
}

/// Lịch sử thay đổi trạng thái đơn ứng tuyển
public class ApplicationHistory
{
    public Guid HistoryId { get; set; }
    public Guid ApplicationId { get; set; }
    public byte OldStatusId { get; set; }
    public byte NewStatusId { get; set; }
    public string? Note { get; set; }
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }

    // Navigation properties
    public Application Application { get; set; } = default!;
    public ApplicationStatus OldStatus { get; set; } = default!;
    public ApplicationStatus NewStatus { get; set; } = default!;
    public User ChangedByUser { get; set; } = default!;
}

/// Trạng thái đơn ứng tuyển (lookup table)
public class ApplicationStatus
{
    public byte StatusId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}

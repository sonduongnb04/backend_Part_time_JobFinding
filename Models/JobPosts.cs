using PTJ.Auth;
using PTJ.Org;

namespace PTJ.Jobs;

public class JobPost
{
    public Guid JobPostId { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string Description { get; set; } = default!;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public byte StatusId { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Currency { get; set; } = "VND";
    public byte SalaryUnitId { get; set; }
    public byte ArrangementId { get; set; }
    public string? AddressLine1 { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? Slots { get; set; }
    public DateTime? PublishAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public int ViewCount { get; set; } = 0;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public byte[] RowVer { get; set; } = default!;

    // Navigation properties
    public Company Company { get; set; } = default!;
    public User Creator { get; set; } = default!;
    public ICollection<JobShift> JobShifts { get; set; } = new List<JobShift>();
    public ICollection<JobPostSkill> JobPostSkills { get; set; } = new List<JobPostSkill>();
}

public class JobShift
{
    public Guid JobShiftId { get; set; }
    public Guid JobPostId { get; set; }
    public string? ShiftName { get; set; }
    public byte? DayOfWeek { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Note { get; set; }

    // Navigation
    public JobPost JobPost { get; set; } = default!;
}

public class JobPostSkill
{
    public Guid JobPostId { get; set; }
    public Guid SkillId { get; set; }

    public JobPost JobPost { get; set; } = default!;
}
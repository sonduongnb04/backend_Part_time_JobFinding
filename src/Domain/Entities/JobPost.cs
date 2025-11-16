using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTJ.Domain.Entities;

[Table("JobPosts", Schema = "jobs")]
public class JobPost
{
    [Key] public Guid JobPostId { get; set; }
    public Guid CompanyId { get; set; }
    [MaxLength(500)] public string Title { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string Description { get; set; } = default!;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public byte StatusId { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SalaryMin { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SalaryMax { get; set; }
    [MaxLength(10)] public string Currency { get; set; } = "VND";
    public byte SalaryUnitId { get; set; }
    public byte ArrangementId { get; set; }
    [MaxLength(500)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? Ward { get; set; }
    [MaxLength(100)] public string? District { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Province { get; set; }
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

[Table("JobShifts", Schema = "jobs")]
public class JobShift
{
    [Key] public Guid JobShiftId { get; set; }
    public Guid JobPostId { get; set; }
    [MaxLength(100)] public string? ShiftName { get; set; }
    public byte? DayOfWeek { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    [MaxLength(500)] public string? Note { get; set; }

    // Navigation properties
    public JobPost JobPost { get; set; } = default!;
}

[Table("JobPostSkills", Schema = "jobs")]
public class JobPostSkill
{
    public Guid JobPostId { get; set; }
    public Guid SkillId { get; set; }

    // Navigation properties
    public JobPost JobPost { get; set; } = default!;
}

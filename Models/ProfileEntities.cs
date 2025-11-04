using PTJ.Auth;
using PTJ.Core;

namespace PTJ.Seeker;

public class Profile
{
    public Guid ProfileId { get; set; }
    public Guid StudentUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? University { get; set; }
    public string? Major { get; set; }
    public string? StudentId { get; set; }
    public int? GraduationYear { get; set; }
    public decimal? GPA { get; set; }
    public Guid? AvatarFileId { get; set; }
    public Guid? ResumeFileId { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVer { get; set; } = default!;

    // Navigation
    public User Student { get; set; } = default!;
    public FileEntity? AvatarFile { get; set; }
    public FileEntity? ResumeFile { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = new List<ProfileSkill>();
    public ICollection<ProfileExperience> ProfileExperiences { get; set; } = new List<ProfileExperience>();
    public ICollection<ProfileEducation> ProfileEducations { get; set; } = new List<ProfileEducation>();
    public ICollection<ProfileCertificate> ProfileCertificates { get; set; } = new List<ProfileCertificate>();
}

public class ProfileSkill
{
    public Guid ProfileId { get; set; }
    public Guid SkillId { get; set; }
    public byte? ProficiencyLevel { get; set; }

    public Profile Profile { get; set; } = default!;
}

public class ProfileExperience
{
    public Guid ExperienceId { get; set; }
    public Guid ProfileId { get; set; }
    public string Title { get; set; } = default!;
    public string? CompanyName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Profile Profile { get; set; } = default!;
}

public class ProfileEducation
{
    public Guid EducationId { get; set; }
    public Guid ProfileId { get; set; }
    public string School { get; set; } = default!;
    public string? Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? GPA { get; set; }
    public Guid? CertificateFileId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Profile Profile { get; set; } = default!;
    public FileEntity? CertificateFile { get; set; }
}

public class ProfileCertificate
{
    public Guid CertificateId { get; set; }
    public Guid ProfileId { get; set; }
    public string Name { get; set; } = default!;
    public string? IssuedBy { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? CertificateFileId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Profile Profile { get; set; } = default!;
    public FileEntity? CertificateFile { get; set; }
}

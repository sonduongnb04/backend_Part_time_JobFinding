using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTJ.Domain.Entities;

[Table("Profiles", Schema = "seeker")]
public class Profile
{
    [Key] public Guid ProfileId { get; set; }
    public Guid StudentUserId { get; set; }
    [MaxLength(100)] public string? FirstName { get; set; }
    [MaxLength(100)] public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(20)] public string? Gender { get; set; }
    [MaxLength(256)] public string? Email { get; set; }
    [MaxLength(32)] public string? PhoneNumber { get; set; }
    [MaxLength(500)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? Ward { get; set; }
    [MaxLength(100)] public string? District { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Province { get; set; }
    [MaxLength(256)] public string? University { get; set; }
    [MaxLength(256)] public string? Major { get; set; }
    [MaxLength(50)] public string? StudentId { get; set; }
    public int? GraduationYear { get; set; }
    [Column(TypeName = "decimal(3, 2)")]
    public decimal? GPA { get; set; }
    public Guid? AvatarFileId { get; set; }
    public Guid? ResumeFileId { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVer { get; set; } = default!;

    // Navigation properties
    public User Student { get; set; } = default!;
    public FileEntity? AvatarFile { get; set; }
    public FileEntity? ResumeFile { get; set; }
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = new List<ProfileSkill>();
    public ICollection<ProfileExperience> ProfileExperiences { get; set; } = new List<ProfileExperience>();
    public ICollection<ProfileEducation> ProfileEducations { get; set; } = new List<ProfileEducation>();
    public ICollection<ProfileCertificate> ProfileCertificates { get; set; } = new List<ProfileCertificate>();
}

[Table("ProfileSkills", Schema = "seeker")]
public class ProfileSkill
{
    public Guid ProfileId { get; set; }
    public Guid SkillId { get; set; }
    public byte? ProficiencyLevel { get; set; }

    // Navigation properties
    public Profile Profile { get; set; } = default!;
}

[Table("ProfileExperiences", Schema = "seeker")]
public class ProfileExperience
{
    [Key] public Guid ExperienceId { get; set; }
    public Guid ProfileId { get; set; }
    [MaxLength(256)] public string Title { get; set; } = default!;
    [MaxLength(256)] public string? CompanyName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Profile Profile { get; set; } = default!;
}

[Table("ProfileEducations", Schema = "seeker")]
public class ProfileEducation
{
    [Key] public Guid EducationId { get; set; }
    public Guid ProfileId { get; set; }
    [MaxLength(256)] public string School { get; set; } = default!;
    [MaxLength(256)] public string? Degree { get; set; }
    [MaxLength(256)] public string? FieldOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [Column(TypeName = "decimal(3, 2)")]
    public decimal? GPA { get; set; }
    public Guid? CertificateFileId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Profile Profile { get; set; } = default!;
    public FileEntity? CertificateFile { get; set; }
}

[Table("ProfileCertificates", Schema = "seeker")]
public class ProfileCertificate
{
    [Key] public Guid CertificateId { get; set; }
    public Guid ProfileId { get; set; }
    [MaxLength(256)] public string Name { get; set; } = default!;
    [MaxLength(256)] public string? IssuedBy { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? CertificateFileId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Profile Profile { get; set; } = default!;
    public FileEntity? CertificateFile { get; set; }
}

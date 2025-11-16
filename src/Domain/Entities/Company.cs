using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTJ.Domain.Entities;

[Table("Companies", Schema = "org")]
public class Company
{
    [Key] public Guid CompanyId { get; set; }
    public Guid OwnerUserId { get; set; }
    [MaxLength(256)] public string Name { get; set; } = default!;
    public Guid? IndustryId { get; set; }
    public string? Description { get; set; }
    [MaxLength(500)] public string? WebsiteUrl { get; set; }
    public Guid? LogoFileId { get; set; }
    [MaxLength(256)] public string? EmailPublic { get; set; }
    [MaxLength(32)] public string? PhonePublic { get; set; }
    [MaxLength(500)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? Ward { get; set; }
    [MaxLength(100)] public string? District { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Province { get; set; }
    [MaxLength(20)] public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public byte Verification { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVer { get; set; } = default!;

    // Navigation properties
    public User OwnerUser { get; set; } = default!;
    public FileEntity? LogoFile { get; set; }
}

[Table("CompanyRegistrationRequests", Schema = "org")]
public class CompanyRegistrationRequest
{
    [Key] public Guid RequestId { get; set; }
    public Guid RequestedByUserId { get; set; }

    // Company info
    [MaxLength(256)] public string CompanyName { get; set; } = default!;
    public string? Description { get; set; }
    [MaxLength(500)] public string? WebsiteUrl { get; set; }
    [MaxLength(256)] public string? EmailPublic { get; set; }
    [MaxLength(32)] public string? PhonePublic { get; set; }
    [MaxLength(500)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? Ward { get; set; }
    [MaxLength(100)] public string? District { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Province { get; set; }
    [MaxLength(20)] public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Workflow
    public byte Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public Guid? CreatedCompanyId { get; set; }

    // Navigation properties
    public User RequestedByUser { get; set; } = default!;
    public User? ReviewedByUser { get; set; }
    public Company? CreatedCompany { get; set; }
}

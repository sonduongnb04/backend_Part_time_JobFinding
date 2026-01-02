using PTJ.Auth;
using PTJ.Core;

namespace PTJ.Org;

public class Company
{
    public Guid CompanyId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = default!;
    public Guid? IndustryId { get; set; }
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public Guid? LogoFileId { get; set; }
    public string? EmailPublic { get; set; }
    public string? PhonePublic { get; set; }
    public string? AddressLine1 { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public byte Verification { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVer { get; set; } = default!;

    // Navigation
    public User OwnerUser { get; set; } = default!;
    public FileEntity? LogoFile { get; set; }
}

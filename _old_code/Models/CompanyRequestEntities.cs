// Models/CompanyRequestEntities.cs
namespace PTJ.Org;

public class CompanyRegistrationRequest
{
    public Guid RequestId { get; set; }
    public Guid RequestedByUserId { get; set; }

    // Company info
    public string CompanyName { get; set; } = default!;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
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

    // Workflow
    public byte Status { get; set; } // 0=Pending, 1=Approved, 2=Rejected
    public DateTime RequestedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public Guid? CreatedCompanyId { get; set; }

    // Navigation
    public PTJ.Auth.User RequestedByUser { get; set; } = default!;
    public PTJ.Auth.User? ReviewedByUser { get; set; }
    public Company? CreatedCompany { get; set; }
}
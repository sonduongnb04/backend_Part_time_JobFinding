using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Core;
using PTJ.Data;
using PTJ.Org;
using PTJ.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "EMPLOYER")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public CompaniesController(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    // GET /api/companies/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyCompanies()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var companies = await _db.Companies
            .Include(c => c.LogoFile)
            .Where(c => c.OwnerUserId == userId && !c.IsDeleted)
            .ToListAsync();

        return Ok(companies);
    }

    // GET /api/companies/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await _db.Companies
            .Include(c => c.LogoFile)
            .Where(c => c.CompanyId == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound(new { message = "Company not found" });

        return Ok(company);
    }

    // POST /api/companies
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = new Company
        {
            CompanyId = Guid.NewGuid(),
            OwnerUserId = userId,
            Name = req.Name,
            Description = req.Description,
            WebsiteUrl = req.WebsiteUrl,
            EmailPublic = req.EmailPublic,
            PhonePublic = req.PhonePublic,
            AddressLine1 = req.AddressLine1,
            Ward = req.Ward,
            District = req.District,
            City = req.City,
            Province = req.Province,
            PostalCode = req.PostalCode,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            Verification = 0, // Chưa xác minh
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.Companies.Add(company);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Company created successfully",
            companyId = company.CompanyId
        });
    }

    // PUT /api/companies/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = await _db.Companies
            .Where(c => c.CompanyId == id && c.OwnerUserId == userId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound(new { message = "Company not found or you don't have permission" });

        company.Name = req.Name ?? company.Name;
        company.Description = req.Description ?? company.Description;
        company.WebsiteUrl = req.WebsiteUrl ?? company.WebsiteUrl;
        company.EmailPublic = req.EmailPublic ?? company.EmailPublic;
        company.PhonePublic = req.PhonePublic ?? company.PhonePublic;
        company.AddressLine1 = req.AddressLine1 ?? company.AddressLine1;
        company.Ward = req.Ward ?? company.Ward;
        company.District = req.District ?? company.District;
        company.City = req.City ?? company.City;
        company.Province = req.Province ?? company.Province;
        company.PostalCode = req.PostalCode ?? company.PostalCode;
        company.Latitude = req.Latitude ?? company.Latitude;
        company.Longitude = req.Longitude ?? company.Longitude;
        company.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Company updated successfully" });
    }

    // POST /api/companies/{id}/logo
    [HttpPost("{id}/logo")]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = await _db.Companies
            .Where(c => c.CompanyId == id && c.OwnerUserId == userId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound(new { message = "Company not found or you don't have permission" });

        try
        {
            // Upload file
            var uploadResult = await _fileStorage.UploadFileAsync(file, "logos", userId);

            // Save to database
            var fileEntity = new FileEntity
            {
                FileId = Guid.NewGuid(),
                FileName = uploadResult.FileName,
                ContentType = uploadResult.ContentType,
                ByteSize = uploadResult.ByteSize,
                StorageUrl = uploadResult.StorageUrl,
                StorageProvider = "Local",
                Checksum = uploadResult.Checksum,
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _db.Files.Add(fileEntity);

            // Update company
            company.LogoFileId = fileEntity.FileId;
            company.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                fileId = fileEntity.FileId,
                url = _fileStorage.GetFileUrl(uploadResult.StorageUrl),
                fileName = uploadResult.FileName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/companies/{id}/logo
    [HttpDelete("{id}/logo")]
    public async Task<IActionResult> DeleteLogo(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = await _db.Companies
            .Where(c => c.CompanyId == id && c.OwnerUserId == userId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound(new { message = "Company not found or you don't have permission" });

        if (company.LogoFileId == null)
            return NotFound(new { message = "Logo not found" });

        company.LogoFileId = null;
        company.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Logo deleted successfully" });
    }

    // DELETE /api/companies/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = await _db.Companies
            .Where(c => c.CompanyId == id && c.OwnerUserId == userId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound(new { message = "Company not found or you don't have permission" });

        company.IsDeleted = true;
        company.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Company deleted successfully" });
    }
}

// DTOs
public class CreateCompanyRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

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
}

public class UpdateCompanyRequest
{
    public string? Name { get; set; }
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
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Core;
using PTJ.Data;
using PTJ.Seeker;
using PTJ.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "STUDENT")]
public class ProfilesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public ProfilesController(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    // GET /api/profiles/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var profile = await _db.Profiles
            .Include(p => p.AvatarFile)
            .Include(p => p.ResumeFile)
            .Include(p => p.ProfileExperiences)
            .Include(p => p.ProfileEducations)
            .Include(p => p.ProfileCertificates)
            .Where(p => p.StudentUserId == userId && !p.IsDeleted)
            .FirstOrDefaultAsync();

        if (profile == null)
            return NotFound(new { message = "Profile not found" });

        return Ok(profile);
    }

    // POST /api/profiles/me
    [HttpPost("me")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Check if profile already exists
        var existing = await _db.Profiles
            .Where(p => p.StudentUserId == userId)
            .FirstOrDefaultAsync();

        if (existing != null)
            return BadRequest(new { message = "Profile already exists" });

        var profile = new Profile
        {
            ProfileId = Guid.NewGuid(),
            StudentUserId = userId,
            FirstName = req.FirstName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth,
            Gender = req.Gender,
            Email = req.Email,
            PhoneNumber = req.PhoneNumber,
            AddressLine1 = req.AddressLine1,
            Ward = req.Ward,
            District = req.District,
            City = req.City,
            Province = req.Province,
            University = req.University,
            Major = req.Major,
            StudentId = req.StudentId,
            GraduationYear = req.GraduationYear,
            GPA = req.GPA,
            Bio = req.Bio,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Profile created successfully", profileId = profile.ProfileId });
    }

    // PUT /api/profiles/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var profile = await _db.Profiles
            .Where(p => p.StudentUserId == userId && !p.IsDeleted)
            .FirstOrDefaultAsync();

        if (profile == null)
            return NotFound(new { message = "Profile not found" });

        profile.FirstName = req.FirstName ?? profile.FirstName;
        profile.LastName = req.LastName ?? profile.LastName;
        profile.DateOfBirth = req.DateOfBirth ?? profile.DateOfBirth;
        profile.Gender = req.Gender ?? profile.Gender;
        profile.Email = req.Email ?? profile.Email;
        profile.PhoneNumber = req.PhoneNumber ?? profile.PhoneNumber;
        profile.AddressLine1 = req.AddressLine1 ?? profile.AddressLine1;
        profile.Ward = req.Ward ?? profile.Ward;
        profile.District = req.District ?? profile.District;
        profile.City = req.City ?? profile.City;
        profile.Province = req.Province ?? profile.Province;
        profile.University = req.University ?? profile.University;
        profile.Major = req.Major ?? profile.Major;
        profile.StudentId = req.StudentId ?? profile.StudentId;
        profile.GraduationYear = req.GraduationYear ?? profile.GraduationYear;
        profile.GPA = req.GPA ?? profile.GPA;
        profile.Bio = req.Bio ?? profile.Bio;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully" });
    }

    // POST /api/profiles/me/avatar
    [HttpPost("me/avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            // Upload file
            var uploadResult = await _fileStorage.UploadFileAsync(file, "avatars", userId);

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

            // Update or create profile
            var profile = await _db.Profiles
                .Where(p => p.StudentUserId == userId)
                .FirstOrDefaultAsync();

            if (profile != null)
            {
                profile.AvatarFileId = fileEntity.FileId;
                profile.UpdatedAt = DateTime.UtcNow;
            }

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

    // POST /api/profiles/me/resume
    [HttpPost("me/resume")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            // Upload file
            var uploadResult = await _fileStorage.UploadFileAsync(file, "resumes", userId);

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

            // Update or create profile
            var profile = await _db.Profiles
                .Where(p => p.StudentUserId == userId)
                .FirstOrDefaultAsync();

            if (profile != null)
            {
                profile.ResumeFileId = fileEntity.FileId;
                profile.UpdatedAt = DateTime.UtcNow;
            }

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

    // DELETE /api/profiles/me/avatar
    [HttpDelete("me/avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var profile = await _db.Profiles
            .Where(p => p.StudentUserId == userId)
            .FirstOrDefaultAsync();

        if (profile == null || profile.AvatarFileId == null)
            return NotFound(new { message = "Avatar not found" });

        profile.AvatarFileId = null;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Avatar deleted successfully" });
    }

    // DELETE /api/profiles/me/resume
    [HttpDelete("me/resume")]
    public async Task<IActionResult> DeleteResume()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var profile = await _db.Profiles
            .Where(p => p.StudentUserId == userId)
            .FirstOrDefaultAsync();

        if (profile == null || profile.ResumeFileId == null)
            return NotFound(new { message = "Resume not found" });

        profile.ResumeFileId = null;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Resume deleted successfully" });
    }
}

// DTOs
public class CreateProfileRequest
{
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
    public string? Bio { get; set; }
}

public class UpdateProfileRequest
{
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
    public string? Bio { get; set; }
}

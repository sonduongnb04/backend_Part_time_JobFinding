using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Data;
using PTJ.Jobs;
using PTJ.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobPostsController : ControllerBase
{
    private readonly IJobPostService _jobPostService;
    private readonly AppDbContext _db;

    public JobPostsController(IJobPostService jobPostService, AppDbContext db)
    {
        _jobPostService = jobPostService;
        _db = db;
    }

    // GET /api/jobposts
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllJobPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var jobPosts = await _db.JobPosts
            .Include(j => j.Company)
            .Include(j => j.JobShifts)
            .Where(j => !j.IsDeleted && j.StatusId == 2) // Status 2 = Published
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(jobPosts);
    }

    // GET /api/jobposts/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetJobPost(Guid id)
    {
        var jobPost = await _db.JobPosts
            .Include(j => j.Company)
            .Include(j => j.JobShifts)
            .Include(j => j.JobPostSkills)
            .Where(j => j.JobPostId == id && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Job post not found" });

        // Increase view count
        jobPost.ViewCount++;
        await _db.SaveChangesAsync();

        return Ok(jobPost);
    }

    // GET /api/jobposts/my
    [HttpGet("my")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> GetMyJobPosts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var jobPosts = await _db.JobPosts
            .Include(j => j.Company)
            .Include(j => j.JobShifts)
            .Where(j => j.CreatedBy == userId && !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return Ok(jobPosts);
    }

    // GET /api/jobposts/company/{companyId}
    [HttpGet("company/{companyId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetJobPostsByCompany(Guid companyId)
    {
        var jobPosts = await _db.JobPosts
            .Include(j => j.Company)
            .Include(j => j.JobShifts)
            .Where(j => j.CompanyId == companyId && !j.IsDeleted && j.StatusId == 2)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return Ok(jobPosts);
    }

    // POST /api/jobposts
    [HttpPost]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> CreateJobPost([FromBody] CreateJobPostRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Verify company ownership
        var company = await _db.Companies
            .Where(c => c.CompanyId == req.CompanyId && c.OwnerUserId == userId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (company == null)
            return BadRequest(new { message = "Company not found or you don't have permission" });

        var jobPost = new JobPost
        {
            JobPostId = Guid.NewGuid(),
            CompanyId = req.CompanyId,
            Title = req.Title,
            CategoryId = req.CategoryId,
            Description = req.Description,
            Requirements = req.Requirements,
            Benefits = req.Benefits,
            StatusId = 0, // Draft
            SalaryMin = req.SalaryMin,
            SalaryMax = req.SalaryMax,
            Currency = req.Currency ?? "VND",
            SalaryUnitId = req.SalaryUnitId,
            ArrangementId = req.ArrangementId,
            AddressLine1 = req.AddressLine1,
            Ward = req.Ward,
            District = req.District,
            City = req.City,
            Province = req.Province,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            Slots = req.Slots,
            PublishAt = req.PublishAt,
            ExpireAt = req.ExpireAt,
            ViewCount = 0,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.JobPosts.Add(jobPost);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Job post created successfully",
            jobPostId = jobPost.JobPostId
        });
    }

    // PUT /api/jobposts/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> UpdateJobPost(Guid id, [FromBody] UpdateJobPostRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var jobPost = await _db.JobPosts
            .Include(j => j.Company)
            .Where(j => j.JobPostId == id && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Job post not found or you don't have permission" });

        // Update fields
        if (req.Title != null) jobPost.Title = req.Title;
        if (req.CategoryId != null) jobPost.CategoryId = req.CategoryId;
        if (req.Description != null) jobPost.Description = req.Description;
        if (req.Requirements != null) jobPost.Requirements = req.Requirements;
        if (req.Benefits != null) jobPost.Benefits = req.Benefits;
        if (req.SalaryMin != null) jobPost.SalaryMin = req.SalaryMin;
        if (req.SalaryMax != null) jobPost.SalaryMax = req.SalaryMax;
        if (req.Currency != null) jobPost.Currency = req.Currency;
        if (req.SalaryUnitId != null) jobPost.SalaryUnitId = req.SalaryUnitId.Value;
        if (req.ArrangementId != null) jobPost.ArrangementId = req.ArrangementId.Value;
        if (req.AddressLine1 != null) jobPost.AddressLine1 = req.AddressLine1;
        if (req.Ward != null) jobPost.Ward = req.Ward;
        if (req.District != null) jobPost.District = req.District;
        if (req.City != null) jobPost.City = req.City;
        if (req.Province != null) jobPost.Province = req.Province;
        if (req.Latitude != null) jobPost.Latitude = req.Latitude;
        if (req.Longitude != null) jobPost.Longitude = req.Longitude;
        if (req.Slots != null) jobPost.Slots = req.Slots;
        if (req.PublishAt != null) jobPost.PublishAt = req.PublishAt;
        if (req.ExpireAt != null) jobPost.ExpireAt = req.ExpireAt;

        jobPost.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Job post updated successfully" });
    }

    // DELETE /api/jobposts/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> DeleteJobPost(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var jobPost = await _db.JobPosts
            .Where(j => j.JobPostId == id && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Job post not found or you don't have permission" });

        jobPost.IsDeleted = true;
        jobPost.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Job post deleted successfully" });
    }

    // PUT /api/jobposts/{id}/status
    [HttpPut("{id}/status")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var jobPost = await _db.JobPosts
            .Where(j => j.JobPostId == id && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Job post not found or you don't have permission" });

        jobPost.StatusId = req.StatusId;
        jobPost.UpdatedAt = DateTime.UtcNow;

        // If publishing, set PublishAt if not set
        if (req.StatusId == 2 && jobPost.PublishAt == null)
        {
            jobPost.PublishAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return Ok(new { message = "Job post status updated successfully" });
    }

    // POST /api/jobposts/{id}/shifts
    [HttpPost("{id}/shifts")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> AddShift(Guid id, [FromBody] CreateShiftRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var jobPost = await _db.JobPosts
            .Where(j => j.JobPostId == id && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Job post not found or you don't have permission" });

        var shift = new JobShift
        {
            JobShiftId = Guid.NewGuid(),
            JobPostId = id,
            ShiftName = req.ShiftName,
            DayOfWeek = req.DayOfWeek,
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            Note = req.Note
        };

        _db.JobShifts.Add(shift);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Shift added successfully",
            shiftId = shift.JobShiftId
        });
    }

    // PUT /api/jobposts/shifts/{shiftId}
    [HttpPut("shifts/{shiftId}")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> UpdateShift(Guid shiftId, [FromBody] UpdateShiftRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var shift = await _db.JobShifts
            .Include(s => s.JobPost)
            .Where(s => s.JobShiftId == shiftId && s.JobPost.CreatedBy == userId)
            .FirstOrDefaultAsync();

        if (shift == null)
            return NotFound(new { message = "Shift not found or you don't have permission" });

        if (req.ShiftName != null) shift.ShiftName = req.ShiftName;
        if (req.DayOfWeek != null) shift.DayOfWeek = req.DayOfWeek;
        if (req.StartTime != null) shift.StartTime = req.StartTime;
        if (req.EndTime != null) shift.EndTime = req.EndTime;
        if (req.Note != null) shift.Note = req.Note;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Shift updated successfully" });
    }

    // DELETE /api/jobposts/shifts/{shiftId}
    [HttpDelete("shifts/{shiftId}")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> DeleteShift(Guid shiftId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var shift = await _db.JobShifts
            .Include(s => s.JobPost)
            .Where(s => s.JobShiftId == shiftId && s.JobPost.CreatedBy == userId)
            .FirstOrDefaultAsync();

        if (shift == null)
            return NotFound(new { message = "Shift not found or you don't have permission" });

        _db.JobShifts.Remove(shift);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Shift deleted successfully" });
    }

    // GET /api/jobposts/{id}/shifts
    [HttpGet("{id}/shifts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetShifts(Guid id)
    {
        var shifts = await _db.JobShifts
            .Where(s => s.JobPostId == id)
            .ToListAsync();

        return Ok(shifts);
    }
}

// DTOs
public class CreateJobPostRequest
{
    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Title { get; set; } = default!;

    public Guid? CategoryId { get; set; }

    [Required]
    public string Description { get; set; } = default!;

    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Currency { get; set; }
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
}

public class UpdateJobPostRequest
{
    public string? Title { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Currency { get; set; }
    public byte? SalaryUnitId { get; set; }
    public byte? ArrangementId { get; set; }
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
}

public class ChangeStatusRequest
{
    [Required]
    public byte StatusId { get; set; }
}

public class CreateShiftRequest
{
    public string? ShiftName { get; set; }
    public byte? DayOfWeek { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Note { get; set; }
}

public class UpdateShiftRequest
{
    public string? ShiftName { get; set; }
    public byte? DayOfWeek { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Note { get; set; }
}

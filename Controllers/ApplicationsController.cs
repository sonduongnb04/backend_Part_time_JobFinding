using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Core;
using PTJ.Data;
using PTJ.Jobs;
using PTJ.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public ApplicationsController(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    // STUDENT APIs - Nộp CV và quản lý đơn ứng tuyển

    /// Nộp CV vào tin tuyển dụng

    [HttpPost]
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> ApplyForJob([FromBody] ApplyJobRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 1. Kiểm tra tin tuyển dụng có tồn tại và đang mở
        var jobPost = await _db.JobPosts
            .Include(j => j.Company)
            .Where(j => j.JobPostId == req.JobPostId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Tin tuyển dụng không tồn tại" });

        if (jobPost.StatusId != 2) // 2 = APPROVED (đang mở)
            return BadRequest(new { message = "Tin tuyển dụng không còn nhận hồ sơ" });

        // Kiểm tra hạn nộp
        if (jobPost.ExpireAt.HasValue && jobPost.ExpireAt.Value < DateTime.UtcNow)
            return BadRequest(new { message = "Tin tuyển dụng đã hết hạn nộp" });

        // 2. Kiểm tra đã nộp chưa (1 sinh viên chỉ nộp 1 lần/tin)
        var existingApp = await _db.Applications
            .Where(a => a.JobPostId == req.JobPostId && a.StudentUserId == userId)
            .FirstOrDefaultAsync();

        if (existingApp != null)
            return BadRequest(new { message = "Bạn đã nộp hồ sơ vào tin này rồi" });

        // 3. Lấy profile của sinh viên (nếu có)
        var profile = await _db.Profiles
            .Where(p => p.StudentUserId == userId && !p.IsDeleted)
            .FirstOrDefaultAsync();

        // 4. Kiểm tra CV file
        Guid? cvFileId = null;
        if (req.CVFileId.HasValue)
        {
            // Sử dụng CV từ profile hoặc đã upload trước
            var cvFile = await _db.Files
                .Where(f => f.FileId == req.CVFileId.Value && f.OwnerUserId == userId && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (cvFile == null)
                return BadRequest(new { message = "File CV không tồn tại hoặc không thuộc về bạn" });

            cvFileId = cvFile.FileId;
        }
        else if (profile?.ResumeFileId != null)
        {
            // Tự động dùng CV từ profile
            cvFileId = profile.ResumeFileId;
        }
        else
        {
            return BadRequest(new { message = "Vui lòng tải CV lên trước khi nộp" });
        }

        // 5. Tạo đơn ứng tuyển
        var application = new Application
        {
            ApplicationId = Guid.NewGuid(),
            JobPostId = req.JobPostId,
            StudentUserId = userId,
            ProfileId = profile?.ProfileId,
            CVFileId = cvFileId,
            CoverLetter = req.CoverLetter,
            StatusId = 0, // 0 = APPLIED
            AppliedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Applications.Add(application);

        // 6. Ghi lịch sử thay đổi trạng thái
        var history = new ApplicationHistory
        {
            HistoryId = Guid.NewGuid(),
            ApplicationId = application.ApplicationId,
            OldStatusId = 0,
            NewStatusId = 0,
            Note = "Đã nộp hồ sơ",
            ChangedBy = userId,
            ChangedAt = DateTime.UtcNow
        };

        _db.ApplicationHistory.Add(history);

        await _db.SaveChangesAsync();

        // 7. Trả về thông báo thành công
        return Ok(new
        {
            success = true,
            message = "Nộp CV thành công! Nhà tuyển dụng sẽ xem xét hồ sơ của bạn.",
            data = new
            {
                applicationId = application.ApplicationId,
                jobTitle = jobPost.Title,
                companyName = jobPost.Company.Name,
                appliedAt = application.AppliedAt,
                status = "Đã nộp"
            }
        });
    }

    /// Nộp CV với file upload trực tiếp
    [HttpPost("with-file")]
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> ApplyWithFileUpload([FromForm] ApplyJobWithFileRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 1. Upload CV file trước
        Guid cvFileId;
        try
        {
            var uploadResult = await _fileStorage.UploadFileAsync(req.CVFile, "applications", userId);

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
            await _db.SaveChangesAsync();

            cvFileId = fileEntity.FileId;
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi upload file: " + ex.Message });
        }

        // 2. Gọi lại logic apply với CV vừa upload
        var applyRequest = new ApplyJobRequest
        {
            JobPostId = req.JobPostId,
            CVFileId = cvFileId,
            CoverLetter = req.CoverLetter
        };

        return await ApplyForJob(applyRequest);
    }

    /// Xem danh sách đơn ứng tuyển của mình
    [HttpGet("my")]
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> GetMyApplications(
        [FromQuery] byte? statusId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = _db.Applications
            .Include(a => a.JobPost)
                .ThenInclude(j => j.Company)
            .Include(a => a.CVFile)
            .Include(a => a.Status)
            .Where(a => a.StudentUserId == userId);

        if (statusId.HasValue)
            query = query.Where(a => a.StatusId == statusId.Value);

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                applicationId = a.ApplicationId,
                jobPost = new
                {
                    jobPostId = a.JobPost.JobPostId,
                    title = a.JobPost.Title,
                    company = new
                    {
                        companyId = a.JobPost.Company.CompanyId,
                        name = a.JobPost.Company.Name,
                        logoFileId = a.JobPost.Company.LogoFileId
                    },
                    salaryMin = a.JobPost.SalaryMin,
                    salaryMax = a.JobPost.SalaryMax,
                    currency = a.JobPost.Currency
                },
                cvFile = a.CVFile != null ? new
                {
                    fileId = a.CVFile.FileId,
                    fileName = a.CVFile.FileName
                } : null,
                coverLetter = a.CoverLetter,
                status = new
                {
                    statusId = a.StatusId,
                    statusName = a.Status.Name,
                    statusCode = a.Status.Code
                },
                appliedAt = a.AppliedAt,
                updatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            total = await query.CountAsync(),
            page,
            pageSize,
            data = applications
        });
    }

    /// Xem chi tiết đơn ứng tuyển
    [HttpGet("{id}")]
    [Authorize(Roles = "STUDENT,EMPLOYER")]
    public async Task<IActionResult> GetApplicationDetail(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        var application = await _db.Applications
            .Include(a => a.JobPost)
                .ThenInclude(j => j.Company)
            .Include(a => a.Student)
            .Include(a => a.Profile)
            .Include(a => a.CVFile)
            .Include(a => a.Status)
            .Include(a => a.ApplicationHistories.OrderByDescending(h => h.ChangedAt))
            .Where(a => a.ApplicationId == id)
            .FirstOrDefaultAsync();

        if (application == null)
            return NotFound(new { message = "Đơn ứng tuyển không tồn tại" });

        // Kiểm tra quyền xem
        var isStudent = application.StudentUserId == userId;
        var isEmployer = roles.Contains("EMPLOYER") && application.JobPost.CreatedBy == userId;

        if (!isStudent && !isEmployer)
            return Forbid();

        return Ok(application);
    }

    /// Rút hồ sơ ứng tuyển
    [HttpPost("{id}/withdraw")]
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> WithdrawApplication(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var application = await _db.Applications
            .Where(a => a.ApplicationId == id && a.StudentUserId == userId)
            .FirstOrDefaultAsync();

        if (application == null)
            return NotFound(new { message = "Đơn ứng tuyển không tồn tại" });

        // Chỉ cho phép rút nếu chưa được xử lý
        if (application.StatusId >= 3) // 3 = HIRED, 4 = REJECTED
            return BadRequest(new { message = "Không thể rút hồ sơ khi đã được xử lý" });

        var oldStatus = application.StatusId;
        application.StatusId = 5; // 5 = WITHDRAWN
        application.UpdatedAt = DateTime.UtcNow;

        // Ghi lịch sử
        var history = new ApplicationHistory
        {
            HistoryId = Guid.NewGuid(),
            ApplicationId = application.ApplicationId,
            OldStatusId = oldStatus,
            NewStatusId = 5,
            Note = "Ứng viên rút hồ sơ",
            ChangedBy = userId,
            ChangedAt = DateTime.UtcNow
        };

        _db.ApplicationHistory.Add(history);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã rút hồ sơ thành công" });
    }

    // EMPLOYER APIs - Xem và quản lý đơn ứng tuyển

    /// Xem danh sách ứng viên nộp vào tin của mình
    [HttpGet("job/{jobPostId}")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> GetApplicationsByJob(
        Guid jobPostId,
        [FromQuery] byte? statusId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Kiểm tra quyền sở hữu job post
        var jobPost = await _db.JobPosts
            .Where(j => j.JobPostId == jobPostId && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Tin tuyển dụng không tồn tại hoặc bạn không có quyền" });

        var query = _db.Applications
            .Include(a => a.Student)
            .Include(a => a.Profile)
            .Include(a => a.CVFile)
            .Include(a => a.Status)
            .Where(a => a.JobPostId == jobPostId);

        if (statusId.HasValue)
            query = query.Where(a => a.StatusId == statusId.Value);

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                applicationId = a.ApplicationId,
                student = new
                {
                    userId = a.Student.UserId,
                    fullName = a.Student.FullName,
                    email = a.Student.Email,
                    phoneNumber = a.Student.PhoneNumber,
                    avatarFileId = a.Student.AvatarFileId
                },
                profile = a.Profile != null ? new
                {
                    profileId = a.Profile.ProfileId,
                    university = a.Profile.University,
                    major = a.Profile.Major,
                    gpa = a.Profile.GPA,
                    graduationYear = a.Profile.GraduationYear
                } : null,
                cvFile = a.CVFile != null ? new
                {
                    fileId = a.CVFile.FileId,
                    fileName = a.CVFile.FileName,
                    contentType = a.CVFile.ContentType
                } : null,
                coverLetter = a.CoverLetter,
                status = new
                {
                    statusId = a.StatusId,
                    statusName = a.Status.Name,
                    statusCode = a.Status.Code
                },
                appliedAt = a.AppliedAt,
                updatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            jobPost = new
            {
                jobPostId = jobPost.JobPostId,
                title = jobPost.Title
            },
            total = await query.CountAsync(),
            page,
            pageSize,
            data = applications
        });
    }

    /// Thay đổi trạng thái đơn ứng tuyển (Shortlist, Interview, Hire, Reject)
    [HttpPut("{id}/status")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> ChangeApplicationStatus(
        Guid id,
        [FromBody] ChangeApplicationStatusRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var application = await _db.Applications
            .Include(a => a.JobPost)
            .Where(a => a.ApplicationId == id)
            .FirstOrDefaultAsync();

        if (application == null)
            return NotFound(new { message = "Đơn ứng tuyển không tồn tại" });

        // Kiểm tra quyền
        if (application.JobPost.CreatedBy != userId)
            return Forbid();

        // Không cho đổi status nếu ứng viên đã rút
        if (application.StatusId == 5) // WITHDRAWN
            return BadRequest(new { message = "Ứng viên đã rút hồ sơ" });

        var oldStatus = application.StatusId;
        application.StatusId = req.StatusId;
        application.UpdatedAt = DateTime.UtcNow;

        // Ghi lịch sử
        var history = new ApplicationHistory
        {
            HistoryId = Guid.NewGuid(),
            ApplicationId = application.ApplicationId,
            OldStatusId = oldStatus,
            NewStatusId = req.StatusId,
            Note = req.Note ?? GetStatusChangeName(oldStatus, req.StatusId),
            ChangedBy = userId,
            ChangedAt = DateTime.UtcNow
        };

        _db.ApplicationHistory.Add(history);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Đã cập nhật trạng thái đơn ứng tuyển",
            newStatus = req.StatusId
        });
    }

    /// Thống kê số lượng đơn theo trạng thái
    [HttpGet("job/{jobPostId}/stats")]
    [Authorize(Roles = "EMPLOYER")]
    public async Task<IActionResult> GetApplicationStats(Guid jobPostId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Kiểm tra quyền
        var jobPost = await _db.JobPosts
            .Where(j => j.JobPostId == jobPostId && j.CreatedBy == userId && !j.IsDeleted)
            .FirstOrDefaultAsync();

        if (jobPost == null)
            return NotFound(new { message = "Tin tuyển dụng không tồn tại" });

        var stats = await _db.Applications
            .Where(a => a.JobPostId == jobPostId)
            .GroupBy(a => a.StatusId)
            .Select(g => new
            {
                statusId = g.Key,
                count = g.Count()
            })
            .ToListAsync();

        var total = await _db.Applications
            .Where(a => a.JobPostId == jobPostId)
            .CountAsync();

        return Ok(new
        {
            total,
            byStatus = stats
        });
    }

    // HELPER METHODS

    private string GetStatusChangeName(byte oldStatus, byte newStatus)
    {
        return newStatus switch
        {
            1 => "Chuyển vào danh sách rút gọn",
            2 => "Mời phỏng vấn",
            3 => "Nhận vào làm việc",
            4 => "Từ chối hồ sơ",
            _ => "Thay đổi trạng thái"
        };
    }
}

// DTOs (Data Transfer Objects)

/// Request để nộp CV (sử dụng CV đã có)
public class ApplyJobRequest
{
    [Required(ErrorMessage = "JobPostId là bắt buộc")]
    public Guid JobPostId { get; set; }

    /// ID của file CV (từ profile hoặc đã upload trước). Nếu null sẽ dùng CV từ profile
    public Guid? CVFileId { get; set; }

    /// Thư xin việc (cover letter)
    [MaxLength(2000, ErrorMessage = "Cover letter không được quá 2000 ký tự")]
    public string? CoverLetter { get; set; }
}


/// Request để nộp CV kèm upload file mới
public class ApplyJobWithFileRequest
{
    [Required(ErrorMessage = "JobPostId là bắt buộc")]
    public Guid JobPostId { get; set; }

    [Required(ErrorMessage = "File CV là bắt buộc")]
    public IFormFile CVFile { get; set; } = default!;

    [MaxLength(2000)]
    public string? CoverLetter { get; set; }
}

/// Request để thay đổi trạng thái đơn ứng tuyển

public class ChangeApplicationStatusRequest
{
    [Required]
    [Range(0, 5, ErrorMessage = "StatusId phải từ 0-5")]
    public byte StatusId { get; set; }

    /// Ghi chú khi thay đổi trạng thái
    [MaxLength(500)]
    public string? Note { get; set; }
}

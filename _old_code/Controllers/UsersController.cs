// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Auth;
using PTJ.Data;
using PTJ.Org;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/users/me - Lấy thông tin user hiện tại
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.UserId == userId && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new { message = "User not found" });

        var roles = user.UserRoles.Select(ur => ur.Role.Code).ToList();

        return Ok(new
        {
            userId = user.UserId,
            email = user.Email,
            fullName = user.FullName,
            phoneNumber = user.PhoneNumber,
            isEmailVerified = user.IsEmailVerified,
            isPhoneVerified = user.IsPhoneVerified,
            avatarFileId = user.AvatarFileId,
            roles,
            createdAt = user.CreatedAt
        });
    }

    // GET /api/users/me/roles - Lấy roles từ token (nhanh, không cần query DB)
    [HttpGet("me/roles")]
    public IActionResult GetMyRoles()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new
        {
            userId,
            email,
            roles
        });
    }

    // GET /api/users/{id} - Lấy thông tin user khác (public info)
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _db.Users
            .Where(u => u.UserId == id && !u.IsDeleted && u.IsActive)
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new { message = "User not found" });

        // Chỉ trả thông tin public
        return Ok(new
        {
            userId = user.UserId,
            fullName = user.FullName,
            avatarFileId = user.AvatarFileId,
            createdAt = user.CreatedAt
        });
    }

    // PUT /api/users/me - Update thông tin cá nhân
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _db.Users
            .Where(u => u.UserId == userId && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new { message = "User not found" });

        user.FullName = req.FullName ?? user.FullName;
        user.PhoneNumber = req.PhoneNumber ?? user.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully" });
    }

    // GET /api/users - List users (chỉ ADMIN)
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var users = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(u => new
            {
                userId = u.UserId,
                email = u.Email,
                fullName = u.FullName,
                isActive = u.IsActive,
                roles = u.UserRoles.Select(ur => ur.Role.Code).ToList(),
                createdAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // PUT /api/users/{id}/toggle-active - Khóa/Mở khóa user (chỉ ADMIN)
    [HttpPut("{id}/toggle-active")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ToggleUserActive(Guid id)
    {
        var user = await _db.Users
            .Where(u => u.UserId == id && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new { message = "User not found" });

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = user.IsActive ? "User activated" : "User deactivated",
            isActive = user.IsActive
        });
    }
    // GET /api/admin/company-requests - Danh sách request chờ duyệt
    [HttpGet("company-requests")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetPendingCompanyRequests(
        [FromQuery] byte? status = null, // null=all, 0=Pending, 1=Approved, 2=Rejected
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        var query = _db.CompanyRegistrationRequests
            .Include(r => r.RequestedByUser)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var requests = await query
            .OrderByDescending(r => r.RequestedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(r => new
            {
                requestId = r.RequestId,
                companyName = r.CompanyName,
                description = r.Description,
                requester = new
                {
                    userId = r.RequestedByUser.UserId,
                    email = r.RequestedByUser.Email,
                    fullName = r.RequestedByUser.FullName
                },
                status = r.Status,
                requestedAt = r.RequestedAt,
                reviewedAt = r.ReviewedAt,
                reviewNote = r.ReviewNote
            })
            .ToListAsync();

        return Ok(requests);
    }
    // GET /api/admin/company-requests/{id} - Chi tiết request
    [HttpGet("company-requests/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetCompanyRequest(Guid id)
    {
        var request = await _db.CompanyRegistrationRequests
            .Include(r => r.RequestedByUser)
            .Where(r => r.RequestId == id)
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(new { message = "Request not found" });

        return Ok(request);
    }

    // POST /api/admin/company-requests/{id}/approve - Duyệt request
    [HttpPost("company-requests/{id}/approve")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ApproveCompanyRequest(
        Guid id,
        [FromBody] ReviewRequest req)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = await _db.CompanyRegistrationRequests
            .Include(r => r.RequestedByUser)
            .Where(r => r.RequestId == id && r.Status == 0) // Chỉ approve Pending
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(new { message = "Request not found or already processed" });

        // Tạo Company vào DB
        var company = new Company
        {
            CompanyId = Guid.NewGuid(),
            OwnerUserId = request.RequestedByUserId,
            Name = request.CompanyName,
            Description = request.Description,
            WebsiteUrl = request.WebsiteUrl,
            EmailPublic = request.EmailPublic,
            PhonePublic = request.PhonePublic,
            AddressLine1 = request.AddressLine1,
            Ward = request.Ward,
            District = request.District,
            City = request.City,
            Province = request.Province,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Verification = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.Companies.Add(company);

        // Gán EMPLOYER role cho user
        var employerRole = await _db.Roles
            .Where(r => r.Code == "EMPLOYER")
            .FirstOrDefaultAsync();

        if (employerRole != null)
        {
            var hasRole = await _db.UserRoles
                .AnyAsync(ur => ur.UserId == request.RequestedByUserId
                    && ur.RoleId == employerRole.RoleId);

            if (!hasRole)
            {
                _db.UserRoles.Add(new UserRole
                {
                    UserId = request.RequestedByUserId,
                    RoleId = employerRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        // Cập nhật request status
        request.Status = 1; // Approved
        request.ReviewedByUserId = adminId;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewNote = req.Note;
        request.CreatedCompanyId = company.CompanyId;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Company request approved successfully",
            companyId = company.CompanyId,
            requestId = request.RequestId
        });
    }
    // POST /api/admin/company-requests/{id}/reject - Từ chối request
    [HttpPost("company-requests/{id}/reject")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> RejectCompanyRequest(
        Guid id,
        [FromBody] ReviewRequest req)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = await _db.CompanyRegistrationRequests
            .Where(r => r.RequestId == id && r.Status == 0)
            .FirstOrDefaultAsync();

        if (request == null)
            return NotFound(new { message = "Request not found or already processed" });

        request.Status = 2; // Rejected
        request.ReviewedByUserId = adminId;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewNote = req.Note ?? "Request rejected";

        await _db.SaveChangesAsync();

        return Ok(new { message = "Company request rejected" });
    }

}
// DTOs
public class UpdateUserRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [MaxLength(32)]
    public string? PhoneNumber { get; set; }
}
public class ReviewRequest
{
    public string? Note { get; set; }
}

// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Auth;
using PTJ.Data;
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
}

// DTOs
public class UpdateUserRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [MaxLength(32)]
    public string? PhoneNumber { get; set; }
}
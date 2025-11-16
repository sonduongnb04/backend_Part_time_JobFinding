using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Auth;
using PTJ.Data;
using PTJ.Services;
using System.ComponentModel.DataAnnotations;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext db, JwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        // Kiểm tra email đã tồn tại
        var normalizedEmail = req.Email.ToUpperInvariant();
        var existing = await _db.Users
            .Where(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            return BadRequest(new { message = "Email đã được sử dụng" });
        }

        // Tìm role STUDENT
        var studentRole = await _db.Roles
            .Where(r => r.Code == "STUDENT")
            .FirstOrDefaultAsync();

        if (studentRole == null)
        {
            return BadRequest(new { message = "Role STUDENT chưa được tạo trong hệ thống" });
        }

        // Tạo user mới
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = req.Email,
            NormalizedEmail = normalizedEmail,
            FullName = req.FullName,
            PhoneNumber = req.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };

        _db.Users.Add(user);

        // Gán role STUDENT
        var userRole = new UserRole
        {
            UserId = user.UserId,
            RoleId = studentRole.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        _db.UserRoles.Add(userRole);

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Đăng ký thành công",
            userId = user.UserId,
            email = user.Email,
            fullName = user.FullName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var normalizedEmail = req.Email.ToUpperInvariant();
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
        }

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Tài khoản đã bị khóa" });
        }

        // Lấy danh sách roles
        var roles = user.UserRoles.Select(ur => ur.Role.Code).ToList();

        // Tạo access token
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        // Tạo refresh token
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            TokenId = Guid.NewGuid(),
            UserId = user.UserId,
            Token = refreshTokenString,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            refreshToken = refreshTokenString,
            expiresIn = 3600, // 60 phút
            user = new
            {
                userId = user.UserId,
                email = user.Email,
                fullName = user.FullName,
                phoneNumber = user.PhoneNumber,
                roles
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
    {
        var refreshToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(rt => rt.Token == req.RefreshToken && rt.RevokedAt == null)
            .FirstOrDefaultAsync();

        if (refreshToken == null)
        {
            return Unauthorized(new { message = "Refresh token không hợp lệ" });
        }

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Refresh token đã hết hạn" });
        }

        var user = refreshToken.User;
        if (!user.IsActive || user.IsDeleted)
        {
            return Unauthorized(new { message = "Tài khoản không hợp lệ" });
        }

        // Revoke refresh token cũ
        refreshToken.RevokedAt = DateTime.UtcNow;

        // Tạo access token mới
        var roles = user.UserRoles.Select(ur => ur.Role.Code).ToList();
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);

        // Tạo refresh token mới
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            TokenId = Guid.NewGuid(),
            UserId = user.UserId,
            Token = newRefreshTokenString,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshTokenString,
            expiresIn = 3600
        });
    }
}

// DTOs
public class RegisterRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
    public string Password { get; set; } = default!;

    [MaxLength(200)]
    public string? FullName { get; set; }

    [MaxLength(32)]
    public string? PhoneNumber { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = default!;
}

//rolebase asset control
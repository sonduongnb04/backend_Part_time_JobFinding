using Microsoft.EntityFrameworkCore;
using PTJ.Application.DTOs.Requests.Auth;
using PTJ.Application.DTOs.Responses.Auth;
using PTJ.Application.Interfaces;
using PTJ.Domain.Constants;
using PTJ.Domain.Entities;
using PTJ.Domain.Interfaces;

namespace PTJ.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // Kiểm tra email đã tồn tại
        var normalizedEmail = request.Email.ToUpperInvariant();
        var existing = await _unitOfWork.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted);

        if (existing != null)
        {
            throw new InvalidOperationException("Email đã được sử dụng");
        }

        // Tìm role STUDENT
        var studentRole = await _unitOfWork.Roles
            .FirstOrDefaultAsync(r => r.Code == RoleConstants.Student);

        if (studentRole == null)
        {
            throw new InvalidOperationException("Role STUDENT chưa được tạo trong hệ thống");
        }

        // Tạo user mới
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.Users.AddAsync(user);

        // Gán role STUDENT
        var userRole = new UserRole
        {
            UserId = user.UserId,
            RoleId = studentRole.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRoles.AddAsync(userRole);

        await _unitOfWork.SaveChangesAsync();

        return new RegisterResponse
        {
            Message = "Đăng ký thành công",
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();

        // Tìm user và load roles
        var users = await _unitOfWork.Users.FindAsync(u =>
            u.NormalizedEmail == normalizedEmail && !u.IsDeleted);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
        }

        // Lấy danh sách roles
        var userRoles = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == user.UserId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var allRoles = await _unitOfWork.Roles.GetAllAsync();
        var roles = allRoles
            .Where(r => roleIds.Contains(r.RoleId))
            .Select(r => r.Code)
            .ToList();

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

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresIn = 3600, // 60 phút
            User = new UserInfoDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            }
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshTokens = await _unitOfWork.RefreshTokens
            .FindAsync(rt => rt.Token == request.RefreshToken && rt.RevokedAt == null);
        var refreshToken = refreshTokens.FirstOrDefault();

        if (refreshToken == null)
        {
            throw new UnauthorizedAccessException("Refresh token không hợp lệ");
        }

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token đã hết hạn");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Tài khoản không hợp lệ");
        }

        // Lấy roles
        var userRoles = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == user.UserId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var allRoles = await _unitOfWork.Roles.GetAllAsync();
        var roles = allRoles
            .Where(r => roleIds.Contains(r.RoleId))
            .Select(r => r.Code)
            .ToList();

        // Revoke refresh token cũ
        refreshToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(refreshToken);

        // Tạo access token mới
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

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString,
            ExpiresIn = 3600
        };
    }
}

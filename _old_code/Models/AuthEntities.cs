// Models/AuthEntities.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTJ.Auth;

[Table("Users", Schema = "auth")]
public class User
{
    [Key] public Guid UserId { get; set; }
    [MaxLength(256)] public string Email { get; set; } = default!;
    [MaxLength(256)] public string NormalizedEmail { get; set; } = default!;
    [MaxLength(32)] public string? PhoneNumber { get; set; }
    [MaxLength(200)] public string? FullName { get; set; }
    [MaxLength(255)] public string PasswordHash { get; set; } = default!;
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPhoneVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public Guid? AvatarFileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public byte[] RowVer { get; set; } = Array.Empty<byte>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

[Table("Roles", Schema = "auth")]
public class Role
{
    [Key] public Guid RoleId { get; set; }
    [MaxLength(32)] public string Code { get; set; } = default!; // ADMIN/EMPLOYER/STUDENT
    [MaxLength(64)] public string Name { get; set; } = default!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

[Table("UserRoles", Schema = "auth")]
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}

[Table("RefreshTokens", Schema = "auth")]
public class RefreshToken
{
    [Key] public Guid TokenId { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(500)] public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = default!;
}

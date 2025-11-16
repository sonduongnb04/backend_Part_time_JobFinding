namespace PTJ.Application.DTOs.Responses.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
    public UserInfoDto User { get; set; } = default!;
}

public class UserInfoDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();
}

namespace PTJ.Application.DTOs.Responses.Auth;

public class RegisterResponse
{
    public string Message { get; set; } = default!;
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
}

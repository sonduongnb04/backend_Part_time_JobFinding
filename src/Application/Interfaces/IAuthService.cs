using PTJ.Application.DTOs.Requests.Auth;
using PTJ.Application.DTOs.Responses.Auth;

namespace PTJ.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}

using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
}

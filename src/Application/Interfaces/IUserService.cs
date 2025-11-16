using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(User user);
    Task<bool> AssignRoleAsync(Guid userId, string roleCode);
    Task<bool> RemoveRoleAsync(Guid userId, string roleCode);
    Task<bool> ActivateUserAsync(Guid userId);
    Task<bool> DeactivateUserAsync(Guid userId);
}

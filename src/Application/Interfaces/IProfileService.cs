using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface IProfileService
{
    Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task<Profile> CreateProfileAsync(Profile profile);
    Task<Profile> UpdateProfileAsync(Profile profile);
    Task DeleteProfileAsync(Guid profileId);
}

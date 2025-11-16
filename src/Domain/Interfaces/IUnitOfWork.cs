using PTJ.Domain.Entities;

namespace PTJ.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<UserRole> UserRoles { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<Company> Companies { get; }
    IRepository<CompanyRegistrationRequest> CompanyRegistrationRequests { get; }
    IRepository<Profile> Profiles { get; }
    IRepository<ProfileSkill> ProfileSkills { get; }
    IRepository<ProfileExperience> ProfileExperiences { get; }
    IRepository<ProfileEducation> ProfileEducations { get; }
    IRepository<ProfileCertificate> ProfileCertificates { get; }
    IRepository<JobPost> JobPosts { get; }
    IRepository<JobShift> JobShifts { get; }
    IRepository<JobPostSkill> JobPostSkills { get; }
    IRepository<FileEntity> Files { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

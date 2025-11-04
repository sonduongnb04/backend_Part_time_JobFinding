using PTJ.Auth;
using PTJ.Org;
using PTJ.Seeker;
using PTJ.Jobs;

namespace PTJ.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Company> Companies { get; }
    IRepository<Profile> Profiles { get; }
    IRepository<JobPost> JobPosts { get; }
    IRepository<JobShift> JobShifts { get; }

    Task<int> SaveChangesAsync();
}

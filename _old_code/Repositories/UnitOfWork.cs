using PTJ.Auth;
using PTJ.Data;
using PTJ.Org;
using PTJ.Seeker;
using PTJ.Jobs;

namespace PTJ.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<User>? _users;
    private IRepository<Role>? _roles;
    private IRepository<Company>? _companies;
    private IRepository<Profile>? _profiles;
    private IRepository<JobPost>? _jobPosts;
    private IRepository<JobShift>? _jobShifts;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new GenericRepository<User>(_context);
    public IRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);
    public IRepository<Company> Companies => _companies ??= new GenericRepository<Company>(_context);
    public IRepository<Profile> Profiles => _profiles ??= new GenericRepository<Profile>(_context);
    public IRepository<JobPost> JobPosts => _jobPosts ??= new GenericRepository<JobPost>(_context);
    public IRepository<JobShift> JobShifts => _jobShifts ??= new GenericRepository<JobShift>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

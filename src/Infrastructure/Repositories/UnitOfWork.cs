using Microsoft.EntityFrameworkCore.Storage;
using PTJ.Domain.Entities;
using PTJ.Domain.Interfaces;
using PTJ.Infrastructure.Data;

namespace PTJ.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Users = new GenericRepository<User>(_context);
        Roles = new GenericRepository<Role>(_context);
        UserRoles = new GenericRepository<UserRole>(_context);
        RefreshTokens = new GenericRepository<RefreshToken>(_context);
        Companies = new GenericRepository<Company>(_context);
        CompanyRegistrationRequests = new GenericRepository<CompanyRegistrationRequest>(_context);
        Profiles = new GenericRepository<Profile>(_context);
        ProfileSkills = new GenericRepository<ProfileSkill>(_context);
        ProfileExperiences = new GenericRepository<ProfileExperience>(_context);
        ProfileEducations = new GenericRepository<ProfileEducation>(_context);
        ProfileCertificates = new GenericRepository<ProfileCertificate>(_context);
        JobPosts = new GenericRepository<JobPost>(_context);
        JobShifts = new GenericRepository<JobShift>(_context);
        JobPostSkills = new GenericRepository<JobPostSkill>(_context);
        Files = new GenericRepository<FileEntity>(_context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Role> Roles { get; }
    public IRepository<UserRole> UserRoles { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }
    public IRepository<Company> Companies { get; }
    public IRepository<CompanyRegistrationRequest> CompanyRegistrationRequests { get; }
    public IRepository<Profile> Profiles { get; }
    public IRepository<ProfileSkill> ProfileSkills { get; }
    public IRepository<ProfileExperience> ProfileExperiences { get; }
    public IRepository<ProfileEducation> ProfileEducations { get; }
    public IRepository<ProfileCertificate> ProfileCertificates { get; }
    public IRepository<JobPost> JobPosts { get; }
    public IRepository<JobShift> JobShifts { get; }
    public IRepository<JobPostSkill> JobPostSkills { get; }
    public IRepository<FileEntity> Files { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

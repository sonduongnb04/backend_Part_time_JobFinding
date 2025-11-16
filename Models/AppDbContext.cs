using Microsoft.EntityFrameworkCore;
using PTJ.Auth;
using PTJ.Core;
using PTJ.Seeker;
using PTJ.Org;
using PTJ.Jobs;

namespace PTJ.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    // Auth
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Files
    public DbSet<FileEntity> Files => Set<FileEntity>();

    // Profiles
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileSkill> ProfileSkills => Set<ProfileSkill>();
    public DbSet<ProfileExperience> ProfileExperiences => Set<ProfileExperience>();
    public DbSet<ProfileEducation> ProfileEducations => Set<ProfileEducation>();
    public DbSet<ProfileCertificate> ProfileCertificates => Set<ProfileCertificate>();

    // Companies
    public DbSet<Company> Companies => Set<Company>();

    //Jobs
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<JobShift> JobShifts => Set<JobShift>();
    public DbSet<JobPostSkill> JobPostSkills => Set<JobPostSkill>();

    // Applications
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<ApplicationHistory> ApplicationHistory => Set<ApplicationHistory>();
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();

    public DbSet<CompanyRegistrationRequest> CompanyRegistrationRequests => Set<CompanyRegistrationRequest>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // auth.Users
        b.Entity<User>(e =>
        {
            e.ToTable("Users", "auth");
            e.HasKey(x => x.UserId);
            e.Property(x => x.UserId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.RowVer).IsRowVersion();

            // UX_auth_Users_Email WHERE IsDeleted=0
            e.HasIndex(x => x.NormalizedEmail)
             .HasDatabaseName("UX_auth_Users_Email")
             .IsUnique()
             .HasFilter("[IsDeleted] = 0"); // khớp script SQL
        });

        // auth.Roles
        b.Entity<Role>(e =>
        {
            e.ToTable("Roles", "auth");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.RoleId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasIndex(x => x.Code).IsUnique();
        });

        // auth.UserRoles (PK (UserId, RoleId))
        b.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles", "auth");
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany(r => r.UserRoles).HasForeignKey(x => x.RoleId);
        });

        // auth.RefreshTokens
        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens", "auth");
            e.HasKey(x => x.TokenId);
            e.Property(x => x.TokenId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // core.Files
        b.Entity<FileEntity>(e =>
        {
            e.ToTable("Files", "core");
            e.HasKey(x => x.FileId);
            e.Property(x => x.FileId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId);
        });

        // seeker.Profiles
        b.Entity<Profile>(e =>
        {
            e.ToTable("Profiles", "seeker");
            e.HasKey(x => x.ProfileId);
            e.Property(x => x.ProfileId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.RowVer).IsRowVersion();
            e.Property(x => x.GPA).HasPrecision(3, 2);
            e.HasIndex(x => x.StudentUserId).IsUnique();
            e.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.AvatarFile).WithMany().HasForeignKey(x => x.AvatarFileId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ResumeFile).WithMany().HasForeignKey(x => x.ResumeFileId).OnDelete(DeleteBehavior.Restrict);
        });

        // seeker.ProfileSkills
        b.Entity<ProfileSkill>(e =>
        {
            e.ToTable("ProfileSkills", "seeker");
            e.HasKey(x => new { x.ProfileId, x.SkillId });
            e.HasOne(x => x.Profile).WithMany(p => p.ProfileSkills).HasForeignKey(x => x.ProfileId);
        });

        // seeker.ProfileExperiences
        b.Entity<ProfileExperience>(e =>
        {
            e.ToTable("ProfileExperiences", "seeker");
            e.HasKey(x => x.ExperienceId);
            e.Property(x => x.ExperienceId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.Profile).WithMany(p => p.ProfileExperiences).HasForeignKey(x => x.ProfileId);
        });

        // seeker.ProfileEducations
        b.Entity<ProfileEducation>(e =>
        {
            e.ToTable("ProfileEducations", "seeker");
            e.HasKey(x => x.EducationId);
            e.Property(x => x.EducationId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.GPA).HasPrecision(3, 2);
            e.HasOne(x => x.Profile).WithMany(p => p.ProfileEducations).HasForeignKey(x => x.ProfileId);
            e.HasOne(x => x.CertificateFile).WithMany().HasForeignKey(x => x.CertificateFileId).OnDelete(DeleteBehavior.Restrict);
        });

        // seeker.ProfileCertificates
        b.Entity<ProfileCertificate>(e =>
        {
            e.ToTable("ProfileCertificates", "seeker");
            e.HasKey(x => x.CertificateId);
            e.Property(x => x.CertificateId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.Profile).WithMany(p => p.ProfileCertificates).HasForeignKey(x => x.ProfileId);
            e.HasOne(x => x.CertificateFile).WithMany().HasForeignKey(x => x.CertificateFileId).OnDelete(DeleteBehavior.Restrict);
        });

        // org.Companies
        b.Entity<Company>(e =>
        {
            e.ToTable("Companies", "org");
            e.HasKey(x => x.CompanyId);
            e.Property(x => x.CompanyId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.RowVer).IsRowVersion();
            e.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.LogoFile).WithMany().HasForeignKey(x => x.LogoFileId).OnDelete(DeleteBehavior.Restrict);
        });

        // jobs.JobPosts
        b.Entity<JobPost>(e =>
        {
            e.ToTable("JobPosts", "jobs");
            e.HasKey(x => x.JobPostId);
            e.Property(x => x.JobPostId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.Property(x => x.RowVer).IsRowVersion();
            e.Property(x => x.SalaryMin).HasPrecision(12, 2);
            e.Property(x => x.SalaryMax).HasPrecision(12, 2);
            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // jobs.JobShifts
        b.Entity<JobShift>(e =>
        {
            e.ToTable("JobShifts", "jobs");
            e.HasKey(x => x.JobShiftId);
            e.Property(x => x.JobShiftId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.JobPost).WithMany(p => p.JobShifts).HasForeignKey(x => x.JobPostId);
        });

        // jobs.JobPostSkills
        b.Entity<JobPostSkill>(e =>
        {
            e.ToTable("JobPostSkills", "jobs");
            e.HasKey(x => new { x.JobPostId, x.SkillId });
            e.HasOne(x => x.JobPost).WithMany(p => p.JobPostSkills).HasForeignKey(x => x.JobPostId);
        });

        // jobs.Applications
        b.Entity<Application>(e =>
        {
            e.ToTable("Applications", "jobs");
            e.HasKey(x => x.ApplicationId);
            e.Property(x => x.ApplicationId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasIndex(x => new { x.JobPostId, x.StudentUserId }).IsUnique();
            e.HasOne(x => x.JobPost).WithMany().HasForeignKey(x => x.JobPostId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.CVFile).WithMany().HasForeignKey(x => x.CVFileId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
        });

        // jobs.ApplicationHistory
        b.Entity<ApplicationHistory>(e =>
        {
            e.ToTable("ApplicationHistory", "jobs");
            e.HasKey(x => x.HistoryId);
            e.Property(x => x.HistoryId).HasDefaultValueSql("NEWSEQUENTIALID()");
            e.HasOne(x => x.Application).WithMany(a => a.ApplicationHistories).HasForeignKey(x => x.ApplicationId);
            e.HasOne(x => x.OldStatus).WithMany().HasForeignKey(x => x.OldStatusId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NewStatus).WithMany().HasForeignKey(x => x.NewStatusId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ChangedByUser).WithMany().HasForeignKey(x => x.ChangedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // jobs.ApplicationStatus
        b.Entity<ApplicationStatus>(e =>
        {
            e.ToTable("ApplicationStatus", "jobs");
            e.HasKey(x => x.StatusId);
            e.HasIndex(x => x.Code).IsUnique();
        });

        b.Entity<CompanyRegistrationRequest>(e =>
    {
        e.ToTable("CompanyRegistrationRequests", "org");
        e.HasKey(x => x.RequestId);
        e.Property(x => x.RequestId).HasDefaultValueSql("NEWSEQUENTIALID()");

        e.HasOne(x => x.RequestedByUser)
            .WithMany()
            .HasForeignKey(x => x.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.CreatedCompany)
            .WithMany()
            .HasForeignKey(x => x.CreatedCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    });
    }
}

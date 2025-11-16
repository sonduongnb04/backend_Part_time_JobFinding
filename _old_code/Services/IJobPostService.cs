using PTJ.Jobs;

namespace PTJ.Services;

public interface IJobPostService
{
    Task<JobPost?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<IEnumerable<JobPost>> GetAllAsync(bool includeDeleted = false);
    Task<IEnumerable<JobPost>> GetByCompanyIdAsync(Guid companyId, bool includeDeleted = false);
    Task<IEnumerable<JobPost>> GetByCreatorIdAsync(Guid userId, bool includeDeleted = false);
    Task<JobPost> CreateAsync(JobPost jobPost);
    Task UpdateAsync(JobPost jobPost);
    Task DeleteAsync(Guid id, bool hardDelete = false);
    Task<bool> ChangeStatusAsync(Guid id, byte newStatus);
    Task AddShiftAsync(JobShift shift);
    Task UpdateShiftAsync(JobShift shift);
    Task DeleteShiftAsync(Guid shiftId);
    Task<IEnumerable<JobShift>> GetShiftsByJobPostIdAsync(Guid jobPostId);
}

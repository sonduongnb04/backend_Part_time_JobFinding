using Microsoft.EntityFrameworkCore;
using PTJ.Jobs;
using PTJ.Repositories;

namespace PTJ.Services;

public class JobPostService : IJobPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobPostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<JobPost?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        if (includeDeleted)
        {
            return await _unitOfWork.JobPosts.FirstOrDefaultAsync(j => j.JobPostId == id);
        }
        return await _unitOfWork.JobPosts.FirstOrDefaultAsync(j => j.JobPostId == id && !j.IsDeleted);
    }

    public async Task<IEnumerable<JobPost>> GetAllAsync(bool includeDeleted = false)
    {
        if (includeDeleted)
        {
            return await _unitOfWork.JobPosts.GetAllAsync();
        }
        return await _unitOfWork.JobPosts.FindAsync(j => !j.IsDeleted);
    }

    public async Task<IEnumerable<JobPost>> GetByCompanyIdAsync(Guid companyId, bool includeDeleted = false)
    {
        if (includeDeleted)
        {
            return await _unitOfWork.JobPosts.FindAsync(j => j.CompanyId == companyId);
        }
        return await _unitOfWork.JobPosts.FindAsync(j => j.CompanyId == companyId && !j.IsDeleted);
    }

    public async Task<IEnumerable<JobPost>> GetByCreatorIdAsync(Guid userId, bool includeDeleted = false)
    {
        if (includeDeleted)
        {
            return await _unitOfWork.JobPosts.FindAsync(j => j.CreatedBy == userId);
        }
        return await _unitOfWork.JobPosts.FindAsync(j => j.CreatedBy == userId && !j.IsDeleted);
    }

    public async Task<JobPost> CreateAsync(JobPost jobPost)
    {
        jobPost.JobPostId = Guid.NewGuid();
        jobPost.CreatedAt = DateTime.UtcNow;
        jobPost.UpdatedAt = DateTime.UtcNow;
        jobPost.IsDeleted = false;
        jobPost.ViewCount = 0;

        await _unitOfWork.JobPosts.AddAsync(jobPost);
        await _unitOfWork.SaveChangesAsync();

        return jobPost;
    }

    public async Task UpdateAsync(JobPost jobPost)
    {
        jobPost.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.JobPosts.Update(jobPost);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, bool hardDelete = false)
    {
        var jobPost = await _unitOfWork.JobPosts.FirstOrDefaultAsync(j => j.JobPostId == id);
        if (jobPost == null)
            throw new KeyNotFoundException($"JobPost with ID {id} not found");

        if (hardDelete)
        {
            _unitOfWork.JobPosts.Remove(jobPost);
        }
        else
        {
            jobPost.IsDeleted = true;
            jobPost.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.JobPosts.Update(jobPost);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ChangeStatusAsync(Guid id, byte newStatus)
    {
        var jobPost = await _unitOfWork.JobPosts.FirstOrDefaultAsync(j => j.JobPostId == id);
        if (jobPost == null)
            return false;

        jobPost.StatusId = newStatus;
        jobPost.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.JobPosts.Update(jobPost);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task AddShiftAsync(JobShift shift)
    {
        shift.JobShiftId = Guid.NewGuid();
        await _unitOfWork.JobShifts.AddAsync(shift);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateShiftAsync(JobShift shift)
    {
        _unitOfWork.JobShifts.Update(shift);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteShiftAsync(Guid shiftId)
    {
        var shift = await _unitOfWork.JobShifts.FirstOrDefaultAsync(s => s.JobShiftId == shiftId);
        if (shift == null)
            throw new KeyNotFoundException($"JobShift with ID {shiftId} not found");

        _unitOfWork.JobShifts.Remove(shift);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobShift>> GetShiftsByJobPostIdAsync(Guid jobPostId)
    {
        return await _unitOfWork.JobShifts.FindAsync(s => s.JobPostId == jobPostId);
    }
}

using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface IJobPostService
{
    Task<JobPost?> GetJobPostByIdAsync(Guid jobPostId);
    Task<IEnumerable<JobPost>> GetAllJobPostsAsync(int page, int pageSize);
    Task<IEnumerable<JobPost>> GetJobPostsByCompanyAsync(Guid companyId);
    Task<JobPost> CreateJobPostAsync(JobPost jobPost);
    Task<JobPost> UpdateJobPostAsync(JobPost jobPost);
    Task DeleteJobPostAsync(Guid jobPostId);
    Task<bool> PublishJobPostAsync(Guid jobPostId);
    Task<bool> CloseJobPostAsync(Guid jobPostId);
}

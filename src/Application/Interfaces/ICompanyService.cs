using PTJ.Domain.Entities;

namespace PTJ.Application.Interfaces;

public interface ICompanyService
{
    Task<Company?> GetCompanyByIdAsync(Guid companyId);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<CompanyRegistrationRequest> CreateRegistrationRequestAsync(CompanyRegistrationRequest request);
    Task<CompanyRegistrationRequest> ApproveRequestAsync(Guid requestId, Guid reviewerUserId, string? note);
    Task<CompanyRegistrationRequest> RejectRequestAsync(Guid requestId, Guid reviewerUserId, string? note);
}

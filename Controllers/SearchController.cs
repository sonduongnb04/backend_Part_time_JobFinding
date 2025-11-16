// Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTJ.Data;
using PTJ.Services;

namespace PTJ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISearchService _searchService;

    public SearchController(AppDbContext db, ISearchService searchService)
    {
        _db = db;
        _searchService = searchService;
    }

    /// GET /api/search/jobs?q=keyword&page=1&size=20
    /// Search for job posts by title
    [HttpGet("jobs")]
    public async Task<IActionResult> SearchJobs(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 20;

        // Base query: only active, published, non-deleted job posts
        var query = _db.JobPosts
            .Include(j => j.Company)
            .Where(j => !j.IsDeleted
                && j.PublishAt <= DateTime.UtcNow
                && (j.ExpireAt == null || j.ExpireAt > DateTime.UtcNow));

        // Apply search using SearchService
        var result = await _searchService.SearchAsync(
            query,
            q ?? string.Empty,
            job => job.Title,
            page,
            size);

        // Map to DTO
        var jobDtos = result.Items.Select(j => new
        {
            jobPostId = j.JobPostId,
            title = j.Title,
            company = new
            {
                companyId = j.Company.CompanyId,
                name = j.Company.Name,
                logoFileId = j.Company.LogoFileId
            },
            salaryMin = j.SalaryMin,
            salaryMax = j.SalaryMax,
            currency = j.Currency,
            city = j.City,
            province = j.Province,
            publishAt = j.PublishAt,
            expireAt = j.ExpireAt,
            viewCount = j.ViewCount
        }).ToList();

        return Ok(new
        {
            items = jobDtos,
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
            hasNextPage = result.HasNextPage,
            hasPreviousPage = result.HasPreviousPage
        });
    }


    /// GET /api/search/companies?q=keyword&page=1&size=20
    /// Search for companies by name

    [HttpGet("companies")]
    public async Task<IActionResult> SearchCompanies(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 20;

        // Base query: only active, non-deleted companies
        var query = _db.Companies
            .Where(c => !c.IsDeleted);

        // Apply search using SearchService
        var result = await _searchService.SearchAsync(
            query,
            q ?? string.Empty,
            company => company.Name,
            page,
            size);

        // Map to DTO
        var companyDtos = result.Items.Select(c => new
        {
            companyId = c.CompanyId,
            name = c.Name,
            description = c.Description,
            websiteUrl = c.WebsiteUrl,
            logoFileId = c.LogoFileId,
            emailPublic = c.EmailPublic,
            phonePublic = c.PhonePublic,
            city = c.City,
            province = c.Province,
            verification = c.Verification,
            createdAt = c.CreatedAt
        }).ToList();

        return Ok(new
        {
            items = companyDtos,
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
            hasNextPage = result.HasNextPage,
            hasPreviousPage = result.HasPreviousPage
        });
    }
}

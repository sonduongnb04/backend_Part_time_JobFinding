# Kế Hoạch Refactoring & Clean Code

## Tổng Quan

Tài liệu này mô tả kế hoạch chi tiết để cải thiện cấu trúc và chất lượng code của dự án Backend Part-Time Job Finding. Mục tiêu là áp dụng các best practices, cải thiện maintainability, và tổ chức code theo Clean Architecture.

---

## Phân Tích Vấn Đề Hiện Tại

### 1. Vấn Đề Về Cấu Trúc

#### 1.1 DTOs Nằm Cùng Controllers
**Hiện tại:**
- Các Request/Response classes được định nghĩa ở cuối mỗi file Controller
- Ví dụ: `CreateJobPostRequest`, `UpdateJobPostRequest` trong `JobPostsController.cs`

**Vấn đề:**
- Khó tái sử dụng DTOs
- File Controllers quá dài (hơn 400 dòng)
- Vi phạm Single Responsibility Principle

#### 1.2 Thiếu Tổ Chức Folder
**Hiện tại:**
```
backend_Part_time_JobFinding/
├── Controllers/
├── Models/
├── Services/
├── Repositories/
└── Migrations/
```

**Vấn đề:**
- Không có folder cho DTOs, Constants, Enums, Validators
- Không có folder Extensions, Middleware
- Khó tìm kiếm và quản lý code

#### 1.3 Namespaces Không Consistent
**Hiện tại:**
- Models: `PTJ.Auth`, `PTJ.Core`, `PTJ.Seeker`, `PTJ.Org`, `PTJ.Jobs`
- Controllers: `PTJ.Api.Controllers`
- Services: `PTJ.Services`
- Repositories: `PTJ.Repositories`

**Vấn đề:**
- Một số dùng domain-based (Auth, Jobs), một số dùng layer-based (Services, Repositories)
- Gây khó hiểu về tổ chức code

### 2. Vấn Đề Về Business Logic

#### 2.1 Controllers Quá Dày (Fat Controllers)
**Ví dụ từ JobPostsController.cs:28-93:**
```csharp
[HttpGet]
public async Task<IActionResult> GetAllJobPosts(...)
{
    // Validation logic
    if (page <= 0) page = 1;
    if (pageSize <= 0) pageSize = 20;

    // Query building logic
    var query = _db.JobPosts.Include(...).Where(...)

    // Filtering logic
    if (categoryId != null) { ... }
    if (minSalary != null) { ... }

    // Pagination logic
    var total = await query.CountAsync();
    var items = await query.Skip(...).Take(...).ToListAsync();

    return Ok(new { items, total, page, pageSize });
}
```

**Vấn đề:**
- Business logic nằm trong Controller
- Query logic nằm trong Controller
- Khó test
- Khó tái sử dụng

#### 2.2 Services Chưa Đầy Đủ
**Hiện tại:** Chỉ có `JobPostService`

**Thiếu:**
- `AuthService` - Authentication logic
- `CompanyService` - Company management
- `ProfileService` - Profile management
- `FileStorageService` (có interface nhưng chưa dùng hết)

### 3. Vấn Đề Về Code Quality

#### 3.1 Magic Numbers/Strings
**Ví dụ:**
```csharp
// JobPostsController.cs:44
.Where(j => !j.IsDeleted && j.StatusId == 2) // 2 = Published?

// AuthController.cs:39
.Where(r => r.Code == "STUDENT") // Magic string

// CompaniesController.cs:89
Status = 0, // Pending
```

**Vấn đề:**
- Không rõ ý nghĩa
- Dễ nhầm lẫn
- Khó maintain

#### 3.2 Thiếu Validation Layer
**Hiện tại:** Chỉ có DataAnnotations
```csharp
public class CreateJobPostRequest
{
    [Required]
    [MaxLength(250)]
    public string Title { get; set; }
}
```

**Vấn đề:**
- Validation đơn giản, không có business rules
- Không có custom validators
- Không có cross-field validation

#### 3.3 Không Có Error Handling Middleware
**Hiện tại:** Try-catch rải rác trong controllers
```csharp
try
{
    var uploadResult = await _fileStorage.UploadFileAsync(...);
    ...
}
catch (Exception ex)
{
    return BadRequest(new { message = ex.Message });
}
```

**Vấn đề:**
- Error handling không consistent
- Exception details leak ra client
- Không có logging

#### 3.4 Response Wrapping Không Consistent
**Ví dụ:**
```csharp
// Có khi return entity trực tiếp
return Ok(jobPost);

// Có khi return anonymous object
return Ok(new { message = "Success", jobPostId = id });

// Có khi return custom object
return Ok(new { items, total, page, pageSize });
```

**Vấn đề:**
- Client phải handle nhiều format
- Khó standardize

### 4. Vấn Đề Về Database Access

#### 4.1 Query Logic Trong Controllers
**Ví dụ JobPostsController.cs:41-84:**
```csharp
var query = _db.JobPosts
    .Include(j => j.Company)
    .Include(j => j.JobShifts)
    .Where(j => !j.IsDeleted && j.StatusId == 2);

if (categoryId != null) { query = query.Where(...); }
// ... more filtering
```

**Vấn đề:**
- Vi phạm separation of concerns
- Khó test
- Duplicate query logic

#### 4.2 Repository Pattern Chưa Tận Dụng
**Hiện tại:**
- Có GenericRepository nhưng controllers vẫn inject `AppDbContext` trực tiếp
- UnitOfWork chưa được dùng ở nhiều nơi

**Vấn đề:**
- Không consistent trong cách access data
- Khó mock để test

### 5. Vấn Đề Về Configuration

#### 5.1 Program.cs Quá Dài
**Hiện tại:** Program.cs có 122 dòng với tất cả configuration

**Vấn đề:**
- Khó đọc
- Khó maintain
- Không modular

---

## Cấu Trúc Mới Đề Xuất

### Folder Structure

```
backend_Part_time_JobFinding/
│
├── PTJ.Api/                          # Web API Layer
│   ├── Controllers/                  # API Controllers (thin)
│   │   ├── V1/                       # API versioning
│   │   │   ├── AuthController.cs
│   │   │   ├── JobPostsController.cs
│   │   │   ├── CompaniesController.cs
│   │   │   ├── ProfilesController.cs
│   │   │   └── FilesController.cs
│   │   └── Base/
│   │       └── ApiControllerBase.cs  # Base controller
│   │
│   ├── Filters/                      # Action filters
│   │   ├── ValidateModelStateFilter.cs
│   │   └── ApiExceptionFilter.cs
│   │
│   ├── Middlewares/                  # Custom middlewares
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── PerformanceMonitoringMiddleware.cs
│   │
│   ├── Extensions/                   # Service extensions
│   │   ├── ServiceCollectionExtensions.cs
│   │   ├── SwaggerExtensions.cs
│   │   ├── AuthenticationExtensions.cs
│   │   └── CorsExtensions.cs
│   │
│   ├── Program.cs                    # Minimal entry point
│   └── appsettings.json
│
├── PTJ.Application/                  # Application Layer (Use Cases)
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Auth/
│   │   │   ├── Requests/
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   ├── RegisterRequest.cs
│   │   │   │   └── RefreshTokenRequest.cs
│   │   │   └── Responses/
│   │   │       ├── LoginResponse.cs
│   │   │       └── UserResponse.cs
│   │   │
│   │   ├── JobPosts/
│   │   │   ├── Requests/
│   │   │   │   ├── CreateJobPostRequest.cs
│   │   │   │   ├── UpdateJobPostRequest.cs
│   │   │   │   └── JobPostFilterRequest.cs
│   │   │   └── Responses/
│   │   │       ├── JobPostResponse.cs
│   │   │       └── JobPostDetailResponse.cs
│   │   │
│   │   ├── Companies/
│   │   ├── Profiles/
│   │   └── Common/
│   │       ├── PagedResult.cs
│   │       └── ApiResponse.cs
│   │
│   ├── Services/                     # Application Services
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IJobPostService.cs
│   │   │   ├── ICompanyService.cs
│   │   │   ├── IProfileService.cs
│   │   │   └── IFileStorageService.cs
│   │   │
│   │   └── Implementations/
│   │       ├── AuthService.cs
│   │       ├── JobPostService.cs
│   │       ├── CompanyService.cs
│   │       ├── ProfileService.cs
│   │       └── LocalFileStorageService.cs
│   │
│   ├── Validators/                   # FluentValidation validators
│   │   ├── Auth/
│   │   │   ├── LoginRequestValidator.cs
│   │   │   └── RegisterRequestValidator.cs
│   │   ├── JobPosts/
│   │   └── Companies/
│   │
│   ├── Mappings/                     # AutoMapper profiles
│   │   ├── AuthMappingProfile.cs
│   │   ├── JobPostMappingProfile.cs
│   │   └── CompanyMappingProfile.cs
│   │
│   └── Exceptions/                   # Custom exceptions
│       ├── BusinessException.cs
│       ├── NotFoundException.cs
│       ├── ValidationException.cs
│       └── UnauthorizedException.cs
│
├── PTJ.Domain/                       # Domain Layer (Core)
│   ├── Entities/                     # Domain entities
│   │   ├── Auth/
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   ├── UserRole.cs
│   │   │   └── RefreshToken.cs
│   │   │
│   │   ├── Jobs/
│   │   │   ├── JobPost.cs
│   │   │   ├── JobShift.cs
│   │   │   └── JobPostSkill.cs
│   │   │
│   │   ├── Organizations/
│   │   │   ├── Company.cs
│   │   │   └── CompanyRegistrationRequest.cs
│   │   │
│   │   ├── Seekers/
│   │   │   ├── Profile.cs
│   │   │   ├── ProfileSkill.cs
│   │   │   ├── ProfileExperience.cs
│   │   │   ├── ProfileEducation.cs
│   │   │   └── ProfileCertificate.cs
│   │   │
│   │   └── Core/
│   │       └── FileEntity.cs
│   │
│   ├── Enums/                        # Domain enums
│   │   ├── JobPostStatus.cs
│   │   ├── UserRole.cs
│   │   ├── RequestStatus.cs
│   │   ├── SalaryUnit.cs
│   │   └── WorkArrangement.cs
│   │
│   ├── Constants/                    # Domain constants
│   │   ├── RoleCodes.cs
│   │   ├── ErrorMessages.cs
│   │   └── ValidationMessages.cs
│   │
│   └── Interfaces/                   # Repository interfaces
│       ├── IRepository.cs
│       ├── IUnitOfWork.cs
│       └── Specifications/           # Specification pattern
│           └── ISpecification.cs
│
├── PTJ.Infrastructure/               # Infrastructure Layer
│   ├── Data/                         # EF Core
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/           # Entity configurations
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── JobPostConfiguration.cs
│   │   │   └── CompanyConfiguration.cs
│   │   │
│   │   └── Migrations/
│   │
│   ├── Repositories/                 # Repository implementations
│   │   ├── GenericRepository.cs
│   │   ├── UnitOfWork.cs
│   │   └── Specialized/              # Domain-specific repos
│   │       ├── JobPostRepository.cs
│   │       └── UserRepository.cs
│   │
│   ├── Services/                     # Infrastructure services
│   │   ├── JwtService.cs
│   │   ├── EmailService.cs
│   │   └── FileStorage/
│   │       ├── LocalFileStorageService.cs
│   │       └── AzureBlobStorageService.cs
│   │
│   └── Options/                      # Configuration options
│       ├── JwtOptions.cs
│       ├── FileStorageOptions.cs
│       └── EmailOptions.cs
│
└── PTJ.Shared/                       # Shared utilities
    ├── Extensions/
    │   ├── StringExtensions.cs
    │   └── DateTimeExtensions.cs
    │
    ├── Helpers/
    │   ├── PasswordHelper.cs
    │   └── FileHelper.cs
    │
    └── Utilities/
        └── PaginationHelper.cs
```

### Namespace Convention

**Mới:**
- API Layer: `PTJ.Api.*`
- Application Layer: `PTJ.Application.*`
- Domain Layer: `PTJ.Domain.*`
- Infrastructure Layer: `PTJ.Infrastructure.*`
- Shared: `PTJ.Shared.*`

---

## Kế Hoạch Refactoring Chi Tiết

### Phase 1: Tổ Chức Lại Cấu Trúc (Foundation)

#### Task 1.1: Tạo Enums và Constants
**Ưu tiên:** HIGH
**Thời gian ước tính:** 2-3 giờ

**Các file cần tạo:**

1. **PTJ.Domain/Enums/JobPostStatus.cs**
```csharp
namespace PTJ.Domain.Enums;

public enum JobPostStatus : byte
{
    Draft = 0,
    UnderReview = 1,
    Published = 2,
    Closed = 3
}
```

2. **PTJ.Domain/Enums/UserRoleCode.cs**
```csharp
namespace PTJ.Domain.Enums;

public enum UserRoleCode
{
    Admin,
    Employer,
    Student
}
```

3. **PTJ.Domain/Enums/CompanyRequestStatus.cs**
```csharp
namespace PTJ.Domain.Enums;

public enum CompanyRequestStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
```

4. **PTJ.Domain/Constants/RoleCodes.cs**
```csharp
namespace PTJ.Domain.Constants;

public static class RoleCodes
{
    public const string Admin = "ADMIN";
    public const string Employer = "EMPLOYER";
    public const string Student = "STUDENT";
}
```

5. **PTJ.Domain/Constants/ErrorMessages.cs**
```csharp
namespace PTJ.Domain.Constants;

public static class ErrorMessages
{
    // Auth
    public const string InvalidCredentials = "Email hoặc mật khẩu không đúng";
    public const string EmailAlreadyExists = "Email đã được sử dụng";
    public const string AccountInactive = "Tài khoản đã bị khóa";

    // Generic
    public const string NotFound = "{0} không tìm thấy";
    public const string NoPermission = "Bạn không có quyền thực hiện hành động này";

    // Company
    public const string PendingRequestExists = "Bạn đã có yêu cầu đang chờ duyệt";
}
```

**Các thay đổi:**
- Replace tất cả magic numbers/strings bằng enums/constants
- Update entity properties để dùng enums
- Update controllers để dùng constants

#### Task 1.2: Tách DTOs Ra Khỏi Controllers
**Ưu tiên:** HIGH
**Thời gian ước tính:** 3-4 giờ

**Các file cần tạo:**

```
PTJ.Application/DTOs/
├── Auth/
│   ├── Requests/
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   └── RefreshTokenRequest.cs
│   └── Responses/
│       └── LoginResponse.cs
│
├── JobPosts/
│   ├── Requests/
│   │   ├── CreateJobPostRequest.cs
│   │   ├── UpdateJobPostRequest.cs
│   │   ├── ChangeJobPostStatusRequest.cs
│   │   ├── CreateShiftRequest.cs
│   │   ├── UpdateShiftRequest.cs
│   │   └── JobPostFilterRequest.cs (NEW)
│   └── Responses/
│       ├── JobPostResponse.cs
│       └── JobPostDetailResponse.cs
│
├── Companies/
│   ├── Requests/
│   │   ├── CreateCompanyRequest.cs
│   │   └── UpdateCompanyRequest.cs
│   └── Responses/
│       └── CompanyResponse.cs
│
├── Profiles/
│   ├── Requests/
│   │   ├── CreateProfileRequest.cs
│   │   └── UpdateProfileRequest.cs
│   └── Responses/
│       └── ProfileResponse.cs
│
└── Common/
    ├── PagedResult.cs (NEW)
    └── ApiResponse.cs (NEW)
```

**Ví dụ PagedResult.cs:**
```csharp
namespace PTJ.Application.DTOs.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

**Ví dụ ApiResponse.cs:**
```csharp
namespace PTJ.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string> errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
```

#### Task 1.3: Tạo Services Đầy Đủ
**Ưu tiên:** HIGH
**Thời gian ước tính:** 6-8 giờ

**Services cần tạo:**

1. **IAuthService & AuthService**
   - Register
   - Login
   - RefreshToken
   - Logout
   - ChangePassword

2. **ICompanyService & CompanyService**
   - GetMyCompanies
   - GetCompanyById
   - CreateCompanyRequest
   - GetMyRequests
   - UpdateCompany
   - DeleteCompany

3. **IProfileService & ProfileService**
   - GetMyProfile
   - CreateProfile
   - UpdateProfile
   - UploadAvatar
   - UploadResume

4. **IJobPostService & JobPostService** (Refactor existing)
   - GetJobPosts (with filtering & pagination)
   - GetJobPostById
   - GetMyJobPosts
   - CreateJobPost
   - UpdateJobPost
   - DeleteJobPost
   - ChangeStatus
   - ManageShifts

**Ví dụ IJobPostService:**
```csharp
namespace PTJ.Application.Services.Interfaces;

public interface IJobPostService
{
    Task<PagedResult<JobPostResponse>> GetJobPostsAsync(JobPostFilterRequest filter);
    Task<JobPostDetailResponse> GetJobPostByIdAsync(Guid id);
    Task<IEnumerable<JobPostResponse>> GetMyJobPostsAsync(Guid userId);
    Task<IEnumerable<JobPostResponse>> GetJobPostsByCompanyAsync(Guid companyId);
    Task<Guid> CreateJobPostAsync(CreateJobPostRequest request, Guid userId);
    Task UpdateJobPostAsync(Guid id, UpdateJobPostRequest request, Guid userId);
    Task DeleteJobPostAsync(Guid id, Guid userId);
    Task ChangeStatusAsync(Guid id, JobPostStatus newStatus, Guid userId);
}
```

### Phase 2: Error Handling & Validation

#### Task 2.1: Tạo Custom Exceptions
**Ưu tiên:** HIGH
**Thời gian ước tính:** 1-2 giờ

**Files:**

```csharp
// PTJ.Application/Exceptions/BusinessException.cs
namespace PTJ.Application.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

// PTJ.Application/Exceptions/NotFoundException.cs
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} với ID '{key}' không tìm thấy") { }
}

// PTJ.Application/Exceptions/UnauthorizedException.cs
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

// PTJ.Application/Exceptions/ForbiddenException.cs
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Bạn không có quyền thực hiện hành động này")
        : base(message) { }
}
```

#### Task 2.2: Tạo Exception Handling Middleware
**Ưu tiên:** HIGH
**Thời gian ước tính:** 2-3 giờ

```csharp
// PTJ.Api/Middlewares/ExceptionHandlingMiddleware.cs
namespace PTJ.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = exception switch
        {
            NotFoundException notFound => new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = notFound.Message
            },
            UnauthorizedException unauthorized => new
            {
                statusCode = StatusCodes.Status401Unauthorized,
                message = unauthorized.Message
            },
            ForbiddenException forbidden => new
            {
                statusCode = StatusCodes.Status403Forbidden,
                message = forbidden.Message
            },
            BusinessException business => new
            {
                statusCode = StatusCodes.Status400BadRequest,
                message = business.Message
            },
            _ => new
            {
                statusCode = StatusCodes.Status500InternalServerError,
                message = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau."
            }
        };

        response.StatusCode = apiResponse.statusCode;
        await response.WriteAsJsonAsync(apiResponse);
    }
}
```

#### Task 2.3: Thêm FluentValidation
**Ưu tiên:** MEDIUM
**Thời gian ước tính:** 4-5 giờ

**Cài đặt package:**
```bash
dotnet add package FluentValidation.AspNetCore
```

**Ví dụ Validator:**
```csharp
// PTJ.Application/Validators/Auth/RegisterRequestValidator.cs
namespace PTJ.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email là bắt buộc")
            .EmailAddress().WithMessage("Email không hợp lệ")
            .MaximumLength(256).WithMessage("Email không được vượt quá 256 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường")
            .Matches(@"[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 số");

        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("Họ tên không được vượt quá 200 ký tự");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^0[0-9]{9}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Số điện thoại không hợp lệ");
    }
}
```

**Configuration trong Program.cs:**
```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
```

### Phase 3: AutoMapper Integration

#### Task 3.1: Cài Đặt AutoMapper
**Ưu tiên:** MEDIUM
**Thời gian ước tính:** 3-4 giờ

```bash
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

**Ví dụ Mapping Profile:**
```csharp
// PTJ.Application/Mappings/JobPostMappingProfile.cs
namespace PTJ.Application.Mappings;

public class JobPostMappingProfile : Profile
{
    public JobPostMappingProfile()
    {
        // Entity -> Response
        CreateMap<JobPost, JobPostResponse>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusId.ToString()));

        CreateMap<JobPost, JobPostDetailResponse>()
            .IncludeMembers(src => src.Company);

        // Request -> Entity
        CreateMap<CreateJobPostRequest, JobPost>()
            .ForMember(dest => dest.JobPostId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateJobPostRequest, JobPost>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Phase 4: Refactor Controllers

#### Task 4.1: Tạo Base Controller
**Ưu tiên:** MEDIUM
**Thời gian ước tính:** 1 giờ

```csharp
// PTJ.Api/Controllers/Base/ApiControllerBase.cs
namespace PTJ.Api.Controllers.Base;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email)!;

    protected bool IsInRole(string role) => User.IsInRole(role);

    protected IActionResult Success<T>(T data, string message = null)
    {
        return Ok(ApiResponse<T>.SuccessResult(data, message));
    }

    protected IActionResult Error(string message, List<string> errors = null)
    {
        return BadRequest(ApiResponse<object>.ErrorResult(message, errors));
    }
}
```

#### Task 4.2: Refactor JobPostsController
**Ưu tiên:** HIGH
**Thời gian ước tính:** 2-3 giờ

**Trước:**
```csharp
[HttpGet]
public async Task<IActionResult> GetAllJobPosts(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] Guid? categoryId = null,
    ...)
{
    // 50+ lines of query building, filtering, pagination
    var query = _db.JobPosts.Include(...).Where(...);
    // ...
    return Ok(new { items, total, page, pageSize });
}
```

**Sau:**
```csharp
[HttpGet]
public async Task<IActionResult> GetAllJobPosts([FromQuery] JobPostFilterRequest filter)
{
    var result = await _jobPostService.GetJobPostsAsync(filter);
    return Success(result);
}
```

### Phase 5: Configuration & Extensions

#### Task 5.1: Tách Configuration Extensions
**Ưu tiên:** MEDIUM
**Thời gian ước tính:** 2-3 giờ

```csharp
// PTJ.Api/Extensions/ServiceCollectionExtensions.cs
namespace PTJ.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJobPostService, JobPostService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IProfileService, ProfileService>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

// PTJ.Api/Extensions/AuthenticationExtensions.cs
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtOptions>(jwtSection);
        services.AddSingleton<JwtService>();

        var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key not found");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
```

**Program.cs mới (simplified):**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Custom extensions
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddFileStorage(builder.Configuration);
builder.Services.AddAutoMapperProfiles();
builder.Services.AddFluentValidation();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Phase 6: Repository Pattern Enhancement

#### Task 6.1: Thêm Specification Pattern
**Ưu tiên:** LOW
**Thời gian ước tính:** 3-4 giờ

```csharp
// PTJ.Domain/Interfaces/Specifications/ISpecification.cs
namespace PTJ.Domain.Interfaces.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}

// PTJ.Application/Specifications/JobPostSpecification.cs
public class PublishedJobPostsSpecification : BaseSpecification<JobPost>
{
    public PublishedJobPostsSpecification(JobPostFilterRequest filter)
        : base(j => !j.IsDeleted && j.StatusId == JobPostStatus.Published)
    {
        AddInclude(j => j.Company);
        AddInclude(j => j.JobShifts);

        if (filter.CategoryId.HasValue)
        {
            And(j => j.CategoryId == filter.CategoryId);
        }

        if (filter.MinSalary.HasValue)
        {
            And(j => j.SalaryMax == null || j.SalaryMax >= filter.MinSalary);
        }

        ApplyPaging(filter.PageSize * (filter.Page - 1), filter.PageSize);
        ApplyOrderByDescending(j => j.CreatedAt);
    }
}
```

### Phase 7: Logging & Monitoring

#### Task 7.1: Thêm Structured Logging
**Ưu tiên:** MEDIUM
**Thời gian ước tính:** 2-3 giờ

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

```csharp
// Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

#### Task 7.2: Request Logging Middleware
**Ưu tiên:** LOW
**Thời gian ước tính:** 1-2 giờ

```csharp
// PTJ.Api/Middlewares/RequestLoggingMiddleware.cs
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        await _next(context);

        var duration = DateTime.UtcNow - startTime;

        _logger.LogInformation(
            "Request {Method} {Path} completed in {Duration}ms with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            duration.TotalMilliseconds,
            context.Response.StatusCode
        );
    }
}
```

---

## Ưu Tiên Thực Hiện

### Sprint 1 (Week 1): Foundation
1. Tạo Enums và Constants (Task 1.1)
2. Tách DTOs (Task 1.2)
3. Tạo Custom Exceptions (Task 2.1)
4. Tạo Exception Handling Middleware (Task 2.2)

### Sprint 2 (Week 2): Services
1. Tạo AuthService (Task 1.3.1)
2. Tạo CompanyService (Task 1.3.2)
3. Tạo ProfileService (Task 1.3.3)
4. Refactor JobPostService (Task 1.3.4)

### Sprint 3 (Week 3): Controllers & Validation
1. Tạo Base Controller (Task 4.1)
2. Refactor tất cả Controllers (Task 4.2)
3. Thêm FluentValidation (Task 2.3)

### Sprint 4 (Week 4): Polish & Enhancement
1. Thêm AutoMapper (Task 3.1)
2. Tách Configuration Extensions (Task 5.1)
3. Thêm Logging (Task 7.1, 7.2)
4. Testing & Bug fixes

---

## Checklist

### Phase 1: Foundation
- [ ] Tạo folder structure mới
- [ ] Tạo enums cho JobPostStatus, UserRoleCode, CompanyRequestStatus
- [ ] Tạo constants classes (RoleCodes, ErrorMessages, ValidationMessages)
- [ ] Tách tất cả DTOs ra khỏi controllers
- [ ] Tạo PagedResult và ApiResponse generic classes
- [ ] Update entities để sử dụng enums
- [ ] Replace magic numbers/strings

### Phase 2: Error Handling
- [ ] Tạo custom exceptions (BusinessException, NotFoundException, etc.)
- [ ] Implement ExceptionHandlingMiddleware
- [ ] Register middleware trong Program.cs
- [ ] Update services để throw custom exceptions
- [ ] Cài đặt FluentValidation
- [ ] Tạo validators cho tất cả DTOs
- [ ] Register validators trong DI container

### Phase 3: Services
- [ ] Tạo IAuthService interface
- [ ] Implement AuthService
- [ ] Tạo ICompanyService interface
- [ ] Implement CompanyService
- [ ] Tạo IProfileService interface
- [ ] Implement ProfileService
- [ ] Refactor IJobPostService
- [ ] Refactor JobPostService implementation
- [ ] Move business logic từ controllers sang services

### Phase 4: Controllers
- [ ] Tạo ApiControllerBase
- [ ] Refactor AuthController
- [ ] Refactor JobPostsController
- [ ] Refactor CompaniesController
- [ ] Refactor ProfilesController
- [ ] Refactor FilesController
- [ ] Ensure consistent response format

### Phase 5: Configuration
- [ ] Tạo ServiceCollectionExtensions
- [ ] Tạo AuthenticationExtensions
- [ ] Tạo SwaggerExtensions
- [ ] Refactor Program.cs
- [ ] Move configurations to appropriate places

### Phase 6: Mapping (Optional)
- [ ] Cài đặt AutoMapper
- [ ] Tạo mapping profiles
- [ ] Update services để sử dụng AutoMapper
- [ ] Remove manual mapping code

### Phase 7: Advanced (Optional)
- [ ] Implement Specification Pattern
- [ ] Add structured logging (Serilog)
- [ ] Add request logging middleware
- [ ] Add performance monitoring
- [ ] Add health checks endpoint

### Phase 8: Testing & Documentation
- [ ] Write unit tests cho services
- [ ] Write integration tests cho API endpoints
- [ ] Update API documentation
- [ ] Update README.md
- [ ] Code review và cleanup

---

## Migration Strategy

### Approach: Incremental Refactoring

**Không làm:**
- Big bang refactor - thay đổi toàn bộ codebase cùng lúc
- Break existing functionality

**Nên làm:**
- Refactor từng feature module một
- Maintain backward compatibility
- Test thoroughly sau mỗi thay đổi
- Commit frequently với meaningful messages

### Suggested Order:

1. **Infrastructure First**: Enums, Constants, Exceptions
2. **DTOs**: Tách và organize
3. **Services**: Tạo mới và refactor
4. **Controllers**: Thin down từng cái một
5. **Middleware & Extensions**: Polish
6. **Optional Enhancements**: AutoMapper, Specifications, etc.

---

## Expected Benefits

### Code Quality
- Giảm code duplication
- Tăng testability
- Dễ maintain hơn
- Dễ mở rộng

### Performance
- Không ảnh hưởng đáng kể (chủ yếu là restructuring)
- Có thể cải thiện nhờ caching và optimization

### Developer Experience
- Dễ navigate codebase
- Clear separation of concerns
- Consistent patterns
- Better error messages

### Maintainability
- Easier to add new features
- Easier to fix bugs
- Better documentation through code structure
- Reduced technical debt

---

## Risks & Mitigation

### Risk 1: Breaking Changes
**Mitigation:**
- Comprehensive testing
- Incremental approach
- Keep old code until new code is verified

### Risk 2: Time Overrun
**Mitigation:**
- Prioritize essential refactoring
- Skip optional enhancements if needed
- Time-box each task

### Risk 3: Inconsistency During Transition
**Mitigation:**
- Document current refactoring status
- Use feature flags if needed
- Clear team communication

---

## Conclusion

Kế hoạch refactoring này sẽ cải thiện đáng kể chất lượng code và cấu trúc dự án. Thực hiện theo từng phase một cách cẩn thận sẽ đảm bảo không làm ảnh hưởng đến chức năng hiện tại trong khi cải thiện maintainability và scalability của hệ thống.

**Estimated Total Time:** 4-6 weeks (tùy vào độ ưu tiên và resources)

**Recommended Team Size:** 1-2 developers

**Success Metrics:**
- 90% code coverage với tests
- Zero breaking changes
- Reduced controller line count by 60%
- Improved response time consistency
- Better error handling coverage

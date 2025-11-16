# Backend Part-Time Job Finding System

## Tổng Quan Dự Án

**Part-Time Job Finding (PTJ)** là một hệ thống backend API được xây dựng bằng ASP.NET Core 9.0 để kết nối sinh viên tìm kiếm việc làm bán thời gian với các nhà tuyển dụng. Dự án được phát triển theo kiến trúc Clean Architecture với các tầng rõ ràng và tách biệt.

### Mục Đích
- Cung cấp nền tảng cho sinh viên tìm kiếm công việc bán thời gian
- Cho phép nhà tuyển dụng đăng tin tuyển dụng
- Quản lý hồ sơ ứng viên và công ty
- Hệ thống xác thực và phân quyền dựa trên role

## Công Nghệ Sử Dụng

### Core Technologies
- **.NET 9.0** - Framework chính
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **JWT Bearer Authentication** - Xác thực

### Libraries & Packages
- **BCrypt.Net-Next 4.0.3** - Mã hóa mật khẩu
- **Swashbuckle.AspNetCore 9.0.6** - Swagger/OpenAPI documentation
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** - Identity framework
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server provider
- **Microsoft.IdentityModel.Tokens** - JWT token validation

## Kiến Trúc Dự Án

### Cấu Trúc Thư Mục

```
backend_Part_time_JobFinding/
├── Controllers/           # API Controllers
│   ├── AuthController.cs          # Authentication endpoints
│   ├── CompaniesController.cs     # Company management
│   ├── JobPostsController.cs      # Job posting management
│   ├── ProfilesController.cs      # Student profile management
│   ├── UsersController.cs         # User management
│   └── FilesController.cs         # File operations
├── Models/               # Domain Entities
│   ├── AuthEntities.cs           # User, Role, UserRole, RefreshToken
│   ├── JobPosts.cs               # JobPost, JobShift, JobPostSkill
│   ├── ProfileEntities.cs        # Profile, ProfileSkill, ProfileExperience, etc.
│   ├── CompanyEntities.cs        # Company
│   ├── CompanyRequestEntities.cs # CompanyRegistrationRequest
│   ├── FileEntities.cs           # FileEntity
│   └── AppDbContext.cs           # Database context
├── Services/             # Business Logic Layer
│   ├── JwtService.cs             # JWT token generation/validation
│   ├── JobPostService.cs         # Job post business logic
│   ├── IJobPostService.cs        # Interface
│   ├── LocalFileStorageService.cs # File storage implementation
│   ├── IFileStorageService.cs    # Interface
│   └── JwtOptions.cs             # JWT configuration
├── Repositories/         # Data Access Layer
│   ├── GenericRepository.cs      # Generic CRUD operations
│   ├── IRepository.cs            # Repository interface
│   ├── UnitOfWork.cs             # Unit of Work pattern
│   └── IUnitOfWork.cs            # Interface
├── Migrations/           # EF Core Migrations
├── SQL/                  # SQL scripts
├── uploads/              # File storage
│   ├── avatars/
│   ├── resumes/
│   └── logos/
├── Program.cs            # Application entry point
├── appsettings.json      # Configuration
└── PTJ.Api.csproj        # Project file
```

### Patterns & Principles

1. **Repository Pattern**: Tách biệt logic truy cập dữ liệu
2. **Unit of Work Pattern**: Quản lý transactions
3. **Dependency Injection**: IoC container của ASP.NET Core
4. **Clean Architecture**: Tách biệt concerns và dependencies
5. **SOLID Principles**: Thiết kế code dễ maintain và extend

## Database Schema

### Schemas

Dự án sử dụng 4 database schemas để tổ chức dữ liệu:

1. **auth** - Authentication & Authorization
2. **core** - Core entities (Files)
3. **seeker** - Student/Job seeker profiles
4. **org** - Organizations (Companies)
5. **jobs** - Job postings

### Entities Chi Tiết

#### 1. Auth Schema (auth)

**Users** - Người dùng hệ thống
- UserId (PK, GUID)
- Email, NormalizedEmail (unique index where IsDeleted=0)
- PhoneNumber
- FullName
- PasswordHash (BCrypt)
- IsEmailVerified, IsPhoneVerified
- IsActive, IsDeleted
- AvatarFileId (FK)
- CreatedAt, UpdatedAt
- RowVer (timestamp)

**Roles** - Vai trò người dùng
- RoleId (PK, GUID)
- Code (ADMIN, EMPLOYER, STUDENT) - unique
- Name

**UserRoles** - Quan hệ User-Role (many-to-many)
- UserId, RoleId (composite PK)
- AssignedAt

**RefreshTokens** - Refresh tokens cho JWT
- TokenId (PK, GUID)
- UserId (FK)
- Token (string)
- ExpiresAt, RevokedAt
- CreatedAt

#### 2. Core Schema (core)

**Files** - Quản lý file uploads
- FileId (PK, GUID)
- FileName, ContentType
- ByteSize, StorageUrl
- StorageProvider (Local/Azure)
- Checksum (MD5)
- OwnerUserId (FK)
- CreatedAt, IsDeleted

#### 3. Seeker Schema (seeker)

**Profiles** - Hồ sơ sinh viên
- ProfileId (PK, GUID)
- StudentUserId (FK, unique)
- FirstName, LastName
- DateOfBirth, Gender
- Email, PhoneNumber
- Address (AddressLine1, Ward, District, City, Province)
- University, Major, StudentId
- GraduationYear, GPA (precision 3,2)
- AvatarFileId, ResumeFileId (FK)
- Bio
- CreatedAt, UpdatedAt, IsDeleted
- RowVer (timestamp)

**ProfileSkills** - Kỹ năng của profile
- ProfileId, SkillId (composite PK)
- ProficiencyLevel

**ProfileExperiences** - Kinh nghiệm làm việc
- ExperienceId (PK, GUID)
- ProfileId (FK)
- Title, CompanyName
- StartDate, EndDate, IsCurrent
- Description
- CreatedAt

**ProfileEducations** - Học vấn
- EducationId (PK, GUID)
- ProfileId (FK)
- School, Degree, FieldOfStudy
- StartDate, EndDate
- GPA (precision 3,2)
- CertificateFileId (FK)
- CreatedAt

**ProfileCertificates** - Chứng chỉ
- CertificateId (PK, GUID)
- ProfileId (FK)
- Name, IssuedBy
- IssuedDate, ExpiryDate
- CertificateFileId (FK)
- CreatedAt

#### 4. Org Schema (org)

**Companies** - Công ty
- CompanyId (PK, GUID)
- OwnerUserId (FK)
- Name, IndustryId
- Description, WebsiteUrl
- LogoFileId (FK)
- EmailPublic, PhonePublic
- Address (AddressLine1, Ward, District, City, Province, PostalCode)
- Latitude, Longitude
- Verification
- CreatedAt, UpdatedAt, IsDeleted
- RowVer (timestamp)

**CompanyRegistrationRequests** - Yêu cầu đăng ký công ty
- RequestId (PK, GUID)
- RequestedByUserId (FK)
- CompanyName, Description
- WebsiteUrl, EmailPublic, PhonePublic
- Address fields
- Status (0=Pending, 1=Approved, 2=Rejected)
- RequestedAt, ReviewedAt
- ReviewedByUserId (FK)
- ReviewNote
- CreatedCompanyId (FK)

#### 5. Jobs Schema (jobs)

**JobPosts** - Tin tuyển dụng
- JobPostId (PK, GUID)
- CompanyId (FK)
- Title, CategoryId
- Description, Requirements, Benefits
- StatusId (0=Draft, 1=UnderReview, 2=Published, 3=Closed)
- SalaryMin, SalaryMax (precision 12,2), Currency
- SalaryUnitId, ArrangementId
- Address fields, Latitude, Longitude
- Slots (số lượng tuyển)
- PublishAt, ExpireAt
- ViewCount
- CreatedBy (FK), CreatedAt, UpdatedAt
- IsDeleted
- RowVer (timestamp)

**JobShifts** - Ca làm việc
- JobShiftId (PK, GUID)
- JobPostId (FK)
- ShiftName
- DayOfWeek (0-6)
- StartTime, EndTime (TimeSpan)
- Note

**JobPostSkills** - Kỹ năng yêu cầu
- JobPostId, SkillId (composite PK)

## API Endpoints

### Authentication Endpoints (AuthController)

```
POST /api/auth/register
Body: { email, password, fullName?, phoneNumber? }
Response: { message, userId, email, fullName }
Description: Đăng ký tài khoản sinh viên mới (auto assign STUDENT role)

POST /api/auth/login
Body: { email, password }
Response: { accessToken, refreshToken, expiresIn, user }
Description: Đăng nhập và nhận JWT tokens

POST /api/auth/refresh
Body: { refreshToken }
Response: { accessToken, refreshToken, expiresIn }
Description: Làm mới access token bằng refresh token
```

### Company Management (CompaniesController)

```
GET /api/companies/my
Authorization: EMPLOYER role
Response: Array of companies owned by current user
Description: Lấy danh sách công ty của tôi

GET /api/companies/{id}
Authorization: Anonymous
Response: Company details with logo
Description: Xem chi tiết công ty

POST /api/companies
Authorization: Authenticated
Body: { name, description?, websiteUrl?, emailPublic?, phonePublic?, address fields }
Response: { message, requestId, status }
Description: Tạo yêu cầu đăng ký công ty (cần admin duyệt)

GET /api/companies/my/requests
Authorization: Authenticated
Response: Array of registration requests
Description: Xem trạng thái các yêu cầu đăng ký công ty

PUT /api/companies/{id}
Authorization: EMPLOYER role
Body: UpdateCompanyRequest (các field optional)
Response: { message }
Description: Cập nhật thông tin công ty

POST /api/companies/{id}/logo
Authorization: EMPLOYER role
Body: FormFile (multipart/form-data)
Response: { fileId, url, fileName }
Description: Upload logo công ty

DELETE /api/companies/{id}/logo
Authorization: EMPLOYER role
Response: { message }
Description: Xóa logo công ty

DELETE /api/companies/{id}
Authorization: EMPLOYER role
Response: { message }
Description: Xóa công ty (soft delete)
```

### Job Posts Management (JobPostsController)

```
GET /api/jobposts
Authorization: Anonymous
Query params: page, pageSize, categoryId?, minSalary?, maxSalary?, shiftName?, dayOfWeek?
Response: { items, total, page, pageSize }
Description: Lấy danh sách tin tuyển dụng (có filtering và pagination)
Note: Filtering logic - Salary overlap detection

GET /api/jobposts/{id}
Authorization: Anonymous
Response: JobPost with Company, JobShifts, JobPostSkills
Description: Xem chi tiết tin tuyển dụng (auto tăng viewCount)

GET /api/jobposts/my
Authorization: EMPLOYER role
Response: Array of job posts created by current user
Description: Lấy danh sách tin của tôi

GET /api/jobposts/company/{companyId}
Authorization: Anonymous
Response: Array of published job posts by company
Description: Lấy tin tuyển dụng theo công ty

POST /api/jobposts
Authorization: EMPLOYER role
Body: CreateJobPostRequest
Response: { message, jobPostId }
Description: Tạo tin tuyển dụng mới (status = Draft)

PUT /api/jobposts/{id}
Authorization: EMPLOYER role
Body: UpdateJobPostRequest (các field optional)
Response: { message }
Description: Cập nhật tin tuyển dụng

DELETE /api/jobposts/{id}
Authorization: EMPLOYER role
Response: { message }
Description: Xóa tin tuyển dụng (soft delete)

PUT /api/jobposts/{id}/status
Authorization: EMPLOYER role
Body: { statusId }
Response: { message }
Description: Thay đổi trạng thái tin tuyển dụng

POST /api/jobposts/{id}/shifts
Authorization: EMPLOYER role
Body: { shiftName?, dayOfWeek?, startTime?, endTime?, note? }
Response: { message, shiftId }
Description: Thêm ca làm việc

PUT /api/jobposts/shifts/{shiftId}
Authorization: EMPLOYER role
Body: UpdateShiftRequest
Response: { message }
Description: Cập nhật ca làm việc

DELETE /api/jobposts/shifts/{shiftId}
Authorization: EMPLOYER role
Response: { message }
Description: Xóa ca làm việc

GET /api/jobposts/{id}/shifts
Authorization: Anonymous
Response: Array of JobShifts
Description: Lấy danh sách ca làm việc
```

### Student Profiles (ProfilesController)

```
GET /api/profiles/me
Authorization: STUDENT role
Response: Profile with files, experiences, educations, certificates
Description: Lấy profile của tôi

POST /api/profiles/me
Authorization: STUDENT role
Body: CreateProfileRequest
Response: { message, profileId }
Description: Tạo profile (1 user chỉ có 1 profile)

PUT /api/profiles/me
Authorization: STUDENT role
Body: UpdateProfileRequest (các field optional)
Response: { message }
Description: Cập nhật profile

POST /api/profiles/me/avatar
Authorization: STUDENT role
Body: FormFile (multipart/form-data)
Response: { fileId, url, fileName }
Description: Upload avatar

POST /api/profiles/me/resume
Authorization: STUDENT role
Body: FormFile (multipart/form-data)
Response: { fileId, url, fileName }
Description: Upload CV

DELETE /api/profiles/me/avatar
Authorization: STUDENT role
Response: { message }
Description: Xóa avatar

DELETE /api/profiles/me/resume
Authorization: STUDENT role
Response: { message }
Description: Xóa CV
```

## Authentication & Authorization

### JWT Configuration

```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "ptj-api",
    "Audience": "ptj-client",
    "ExpiresMinutes": 60,
    "RefreshTokenDays": 30
  }
}
```

### Token Flow

1. **Login**: User gửi email/password → nhận accessToken (60 phút) và refreshToken (30 ngày)
2. **Authorization**: Mỗi request gửi kèm `Authorization: Bearer {accessToken}`
3. **Refresh**: Khi accessToken hết hạn, dùng refreshToken để lấy cặp token mới
4. **Logout**: Client xóa tokens (backend có thể revoke refreshToken)

### Role-Based Access Control

- **STUDENT**: Quản lý profile, xem job posts, apply jobs
- **EMPLOYER**: Quản lý công ty, đăng tin tuyển dụng, quản lý ứng viên
- **ADMIN**: Quản trị hệ thống, duyệt công ty, quản lý users

### Password Security

- Sử dụng BCrypt để hash password với cost factor mặc định
- Không lưu plain text password
- Password validation: tối thiểu 8 ký tự

## File Storage System

### Configuration

```json
{
  "FileStorage": {
    "Provider": "Local",
    "LocalPath": "uploads",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx"]
  }
}
```

### Features

1. **Local Storage**: Files được lưu trong thư mục `uploads/` theo folder (avatars, resumes, logos)
2. **File Naming**: GUID-based để tránh trùng lặp
3. **Validation**: Kiểm tra file size, extension
4. **Checksum**: MD5 hash để verify file integrity
5. **Metadata**: Lưu thông tin file trong database (FileEntity)
6. **Static Files**: Serve qua `/uploads/{folder}/{filename}`

### Upload Flow

1. Client upload file qua multipart/form-data
2. Server validate file (size, extension)
3. Generate unique filename (GUID + extension)
4. Save file to disk (`uploads/{folder}/{filename}`)
5. Calculate MD5 checksum
6. Save metadata to database (Files table)
7. Return file info (fileId, url, fileName)

## Tính Năng Chính

### 1. Job Search & Filtering

**Filtering Parameters:**
- Category: Lọc theo ngành nghề
- Salary Range: Overlap detection - tìm jobs có mức lương giao với range tìm kiếm
- Shift Name: LIKE search trên tên ca làm
- Day of Week: Lọc theo ngày trong tuần (0=Sunday, 6=Saturday)

**Pagination:**
- Default: page=1, pageSize=20
- Max pageSize: 100

**Example Query:**
```
GET /api/jobposts?page=1&pageSize=10&minSalary=50000&maxSalary=100000&dayOfWeek=1
```

### 2. Company Registration Workflow

1. User tạo yêu cầu đăng ký công ty (POST /api/companies)
2. Status = Pending (0)
3. Admin review và approve/reject
4. Nếu approved:
   - Tạo Company entity
   - Gán EMPLOYER role cho user
   - Link CreatedCompanyId trong request
5. User có thể xem trạng thái qua GET /api/companies/my/requests

### 3. Job Post Management

**Status Flow:**
- 0 = Draft: Tin nháp, chưa public
- 1 = UnderReview: Đang chờ duyệt (nếu cần)
- 2 = Published: Đã xuất bản, hiển thị công khai
- 3 = Closed: Đã đóng, không nhận ứng viên

**Features:**
- CRUD operations với ownership check
- Shift management (multiple shifts per job)
- Skill requirements
- View counter
- Soft delete

### 4. Student Profile

**Comprehensive Profile:**
- Personal info: Name, DOB, Gender, Contact
- Education: University, Major, GPA, Graduation year
- Files: Avatar, CV/Resume
- Additional sections:
  - Skills với proficiency level
  - Work experiences
  - Education history
  - Certificates

### 5. File Management

**Supported Use Cases:**
- Profile avatars
- Student resumes/CVs
- Company logos
- Education certificates
- Professional certificates

**Features:**
- Upload, download, delete
- Auto-generated URLs
- Ownership tracking
- Soft delete support

## Cài Đặt và Chạy Dự Án

### Prerequisites

- .NET 9.0 SDK
- SQL Server (Express hoặc Developer edition)
- Visual Studio 2022 / VS Code / Rider

### Setup Steps

1. **Clone repository**
```bash
git clone <repository-url>
cd backend_Part_time_JobFinding
```

2. **Cấu hình Database**

   Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost\\SQLEXPRESS;Database=PartTimeJobs;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. **Chạy Migrations**
```bash
dotnet ef database update
```

4. **Seed Data (Optional)**

   Chạy SQL scripts trong thư mục `SQL/` để tạo roles và test data

5. **Chạy Application**
```bash
dotnet run
```

6. **Truy cập Swagger UI**

   Mở browser: `https://localhost:<port>/swagger`

### Configuration Files

**appsettings.json** - Main configuration:
- ConnectionStrings: Database connection
- Jwt: Token configuration
- FileStorage: Upload settings
- Logging: Log levels

**launchSettings.json** - Debug profiles:
- Development environment
- HTTPS/HTTP ports
- Swagger launch

## Development Guidelines

### Code Organization

1. **Controllers**: Thin controllers, delegate to services
2. **Services**: Business logic, validation
3. **Repositories**: Data access only
4. **Models**: Pure entities, no logic

### Naming Conventions

- **Entities**: Singular (User, Company, JobPost)
- **Tables**: Plural (Users, Companies, JobPosts)
- **DTOs**: Suffix with Request/Response (CreateJobPostRequest)
- **Interfaces**: Prefix with I (IRepository, IJobPostService)

### Best Practices

1. **Async/Await**: Tất cả data access operations
2. **Validation**: DataAnnotations cho request DTOs
3. **Authorization**: [Authorize] attribute với Roles
4. **Error Handling**: Return appropriate HTTP status codes
5. **Soft Delete**: IsDeleted flag thay vì hard delete
6. **Timestamps**: CreatedAt, UpdatedAt tracking
7. **Optimistic Concurrency**: RowVer (timestamp) cho critical entities

### Database Patterns

1. **Foreign Keys**: Restrict delete behavior để tránh cascade delete
2. **Indexes**: Unique indexes trên email (với filter IsDeleted=0)
3. **Schemas**: Tổ chức logical theo domain
4. **Migrations**: Tên clear và descriptive

## API Response Patterns

### Success Responses

```json
// List with pagination
{
  "items": [...],
  "total": 100,
  "page": 1,
  "pageSize": 20
}

// Single entity
{
  "jobPostId": "guid",
  "title": "...",
  ...
}

// Action result
{
  "message": "Operation successful",
  "entityId": "guid"
}
```

### Error Responses

```json
// Validation error
{
  "message": "Validation failed",
  "errors": { "field": ["error message"] }
}

// Not found
{
  "message": "Resource not found"
}

// Unauthorized
{
  "message": "Email hoặc mật khẩu không đúng"
}

// Forbidden
{
  "message": "You don't have permission"
}
```

## Future Enhancements

### Planned Features

1. **Job Applications**: Ứng viên apply vào jobs
2. **Messaging**: Chat giữa employer và candidate
3. **Notifications**: Email/push notifications
4. **Reviews & Ratings**: Đánh giá công ty/ứng viên
5. **Advanced Search**: Full-text search, geolocation
6. **Analytics**: Dashboard cho employers
7. **Azure Blob Storage**: Cloud file storage option
8. **Payment Integration**: Premium job posts

### Technical Improvements

1. **Redis Caching**: Cache frequently accessed data
2. **Rate Limiting**: API throttling
3. **Logging**: Structured logging với Serilog
4. **Health Checks**: /health endpoint
5. **API Versioning**: Support multiple API versions
6. **GraphQL**: Alternative to REST
7. **Background Jobs**: Hangfire for scheduled tasks

## Security Considerations

### Current Implementation

1. **Password**: BCrypt hashing
2. **JWT**: Signed tokens với secret key
3. **HTTPS**: TLS encryption
4. **Authorization**: Role-based access control
5. **Input Validation**: DataAnnotations
6. **File Upload**: Extension and size validation

### Recommendations

1. **CORS**: Configure allowed origins
2. **Rate Limiting**: Prevent brute force
3. **Email Verification**: Verify email addresses
4. **2FA**: Two-factor authentication option
5. **Audit Logging**: Track sensitive operations
6. **SQL Injection**: EF Core parameterized queries (already safe)
7. **XSS Prevention**: Output encoding in frontend

## Testing Strategy

### Unit Tests (Recommended)

- Services: Business logic testing
- Repositories: Data access testing
- Controllers: Request/response testing

### Integration Tests

- API endpoints end-to-end
- Database operations
- File upload/download

### Test Data

- Use in-memory database hoặc test database
- Seed test users với different roles
- Mock file operations

## Troubleshooting

### Common Issues

1. **Migration Errors**:
   - Xóa database và migrations folder, tạo lại
   - Kiểm tra connection string

2. **JWT Token Invalid**:
   - Verify JWT secret key
   - Check token expiration
   - Ensure correct Issuer/Audience

3. **File Upload Fails**:
   - Check upload directory permissions
   - Verify file size limit
   - Check allowed extensions

4. **Authorization Failed**:
   - Ensure user has correct role
   - Check token is sent in Authorization header
   - Verify token hasn't expired

## Contact & Support

### Repository Information

- **Branch**: feature/filter
- **Framework**: .NET 9.0
- **Database**: SQL Server
- **Latest Migration**: AddCompanyRegistrationRequests

### Documentation

- Swagger UI: Available in development mode
- Entity Relationship Diagram: (To be created)
- API Postman Collection: (To be created)

---

**Last Updated**: 2025-11-16
**Version**: 1.0
**Status**: Active Development

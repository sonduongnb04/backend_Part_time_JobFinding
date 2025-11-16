# Part-Time Job Finding Platform - Clean Architecture

Hệ thống tìm kiếm việc làm bán thời gian cho sinh viên được xây dựng theo kiến trúc **Clean Architecture** với ASP.NET Core 9.0.

## 📐 Kiến trúc

Dự án được tổ chức theo nguyên tắc Clean Architecture với 4 layers chính:

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                    │
│                      (PTJ.API)                          │
│  - Controllers, Middleware, Filters                     │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Application Layer                       │
│                  (PTJ.Application)                       │
│  - DTOs, Service Interfaces, Validators, Mappings       │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Infrastructure Layer                     │
│                (PTJ.Infrastructure)                      │
│  - DbContext, Repositories, Service Implementations     │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    Domain Layer                          │
│                    (PTJ.Domain)                          │
│  - Entities, Enums, Repository Interfaces               │
└─────────────────────────────────────────────────────────┘
```

### Dependency Flow
- **API** → **Application** → **Infrastructure** → **Domain**
- Domain không phụ thuộc vào bất kỳ layer nào
- Application chỉ phụ thuộc vào Domain
- Infrastructure phụ thuộc vào Domain và Application
- API phụ thuộc vào tất cả các layers

## 📦 Cấu trúc Project

```
/backend_Part_time_JobFinding
├── src/
│   ├── PTJ.Domain/              # Domain Layer - Core business entities
│   │   ├── Common/              # Base entities, value objects
│   │   ├── Entities/            # Domain entities
│   │   ├── Enums/               # Domain enumerations
│   │   └── Interfaces/          # Repository interfaces
│   │
│   ├── PTJ.Application/         # Application Layer - Business logic
│   │   ├── Common/              # Result, PaginatedList, etc.
│   │   ├── DTOs/                # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── JobPost/
│   │   │   ├── Company/
│   │   │   ├── Profile/
│   │   │   └── Application/
│   │   └── Services/            # Service interfaces
│   │
│   ├── PTJ.Infrastructure/      # Infrastructure Layer - External concerns
│   │   ├── Persistence/         # DbContext
│   │   ├── Configurations/      # EF Core entity configurations
│   │   ├── Repositories/        # Repository implementations
│   │   └── Services/            # Service implementations (JWT, File, etc.)
│   │
│   └── PTJ.API/                 # Presentation Layer - Web API
│       ├── Controllers/         # API endpoints
│       ├── Middleware/          # Custom middleware
│       └── Filters/             # Action filters
│
├── PTJ.CleanArchitecture.sln   # Solution file
└── README.md
```

## 🗄️ Database Schema

Hệ thống sử dụng SQL Server với 5 schemas chính:

### 1. **auth** - Authentication & Authorization
- `Users` - Thông tin người dùng
- `Roles` - Vai trò (ADMIN, EMPLOYER, STUDENT)
- `UserRoles` - Gán vai trò cho user
- `RefreshTokens` - JWT refresh tokens

### 2. **org** - Organizations (Companies)
- `Companies` - Thông tin công ty
- `CompanyRegistrationRequests` - Yêu cầu đăng ký công ty

### 3. **seeker** - Job Seekers (Students)
- `Profiles` - Hồ sơ sinh viên
- `ProfileSkills` - Kỹ năng
- `ProfileExperiences` - Kinh nghiệm làm việc
- `ProfileEducations` - Học vấn
- `ProfileCertificates` - Chứng chỉ

### 4. **jobs** - Job Postings & Applications
- `JobPosts` - Tin tuyển dụng
- `JobShifts` - Ca làm việc
- `JobPostSkills` - Kỹ năng yêu cầu
- `Applications` - Đơn ứng tuyển
- `ApplicationHistory` - Lịch sử trạng thái
- `ApplicationStatuses` - Danh mục trạng thái

### 5. **core** - Core/Shared
- `Files` - Metadata file uploads

## 🚀 Cài đặt & Chạy

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019 or later
- Visual Studio 2022 / VS Code / Rider

### Bước 1: Clone Repository
```bash
git clone <repository-url>
cd backend_Part_time_JobFinding
```

### Bước 2: Cấu hình Database Connection
Cập nhật connection string trong `src/PTJ.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=PTJ_CleanArchitecture;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Bước 3: Restore Packages
```bash
dotnet restore PTJ.CleanArchitecture.sln
```

### Bước 4: Tạo Database & Migration

Nếu bạn có .NET CLI và EF Core tools:
```bash
# Cài đặt EF Core tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration
cd src/PTJ.API
dotnet ef migrations add InitialCreate --project ../PTJ.Infrastructure --startup-project . --context AppDbContext

# Update database
dotnet ef database update --project ../PTJ.Infrastructure --startup-project . --context AppDbContext
```

### Bước 5: Chạy Application
```bash
cd src/PTJ.API
dotnet run
```

Application sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

Swagger UI: `https://localhost:5001/swagger`

## 🔑 Tính năng chính

### ✅ Đã implement (Infrastructure layer)
- ✅ Clean Architecture structure
- ✅ Domain Entities với soft delete
- ✅ Repository Pattern & Unit of Work
- ✅ EF Core với Fluent API configurations
- ✅ JWT Authentication
- ✅ Database schemas & relationships
- ✅ Row versioning (concurrency control)
- ✅ Audit fields (CreatedAt, UpdatedAt, DeletedAt)

### 🔨 Cần implement thêm (Application Services)
Các service interfaces đã được định nghĩa, cần implement:

1. **AuthService** - Đăng ký, đăng nhập, refresh token
2. **JobPostService** - CRUD job posts, search, filtering
3. **CompanyService** - Quản lý thông tin công ty
4. **ProfileService** - Quản lý hồ sơ sinh viên
5. **ApplicationService** - Ứng tuyển, theo dõi trạng thái
6. **FileStorageService** - Upload/download files
7. **SearchService** - Tìm kiếm đa điều kiện

### 📝 Controllers cần tạo
- AuthController
- JobPostsController
- CompaniesController
- ProfilesController
- ApplicationsController
- FilesController

## 🔐 JWT Configuration

Cấu hình JWT trong `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32Characters",
    "Issuer": "PTJ.API",
    "Audience": "PTJ.Client",
    "ExpiresMinutes": 60,
    "RefreshTokenDays": 30
  }
}
```

**⚠️ QUAN TRỌNG**: Thay đổi JWT Key trong production!

## 🎯 API Endpoints (Dự kiến)

### Authentication
- `POST /api/auth/register` - Đăng ký tài khoản
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/revoke` - Thu hồi refresh token

### Job Posts
- `GET /api/jobposts` - Danh sách jobs (có phân trang)
- `GET /api/jobposts/{id}` - Chi tiết job
- `POST /api/jobposts` - Tạo job mới (Employer)
- `PUT /api/jobposts/{id}` - Cập nhật job
- `DELETE /api/jobposts/{id}` - Xóa job
- `GET /api/jobposts/search?q=...` - Tìm kiếm jobs

### Companies
- `GET /api/companies` - Danh sách công ty
- `GET /api/companies/{id}` - Chi tiết công ty
- `POST /api/companies` - Tạo công ty mới
- `PUT /api/companies/{id}` - Cập nhật thông tin công ty

### Profiles (Students)
- `GET /api/profiles/me` - Hồ sơ của user hiện tại
- `GET /api/profiles/{id}` - Chi tiết hồ sơ
- `POST /api/profiles` - Tạo/cập nhật hồ sơ
- `PUT /api/profiles/skills` - Cập nhật kỹ năng
- `PUT /api/profiles/experiences` - Cập nhật kinh nghiệm

### Applications
- `GET /api/applications` - Danh sách đơn ứng tuyển
- `GET /api/applications/{id}` - Chi tiết đơn
- `POST /api/applications` - Ứng tuyển việc làm
- `PUT /api/applications/{id}/status` - Thay đổi trạng thái
- `DELETE /api/applications/{id}` - Rút đơn ứng tuyển

### Files
- `POST /api/files/upload` - Upload file (avatar, resume, certificate)
- `GET /api/files/{id}` - Download file
- `DELETE /api/files/{id}` - Xóa file

## 📊 Business Rules

### User Roles
- **ADMIN**: Quản trị hệ thống, duyệt công ty
- **EMPLOYER**: Đăng tin tuyển dụng, xem hồ sơ ứng viên
- **STUDENT**: Tạo hồ sơ, ứng tuyển việc làm

### Job Post Status
- **Draft**: Bản nháp, chưa công khai
- **Active**: Đang tuyển dụng
- **Closed**: Đã đóng
- **Expired**: Hết hạn
- **Archived**: Lưu trữ

### Application Status Flow
```
Pending → Reviewing → Shortlisted → Interviewing → Offered → Accepted
   ↓           ↓           ↓             ↓            ↓
Rejected    Rejected    Rejected     Rejected    Withdrawn
```

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server 2019+
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI
- **Password Hashing**: BCrypt.Net
- **Architecture**: Clean Architecture / Onion Architecture

## 📚 Học Clean Architecture

### Nguyên tắc cốt lõi:
1. **Dependency Inversion**: Dependencies trỏ vào trong (Domain)
2. **Separation of Concerns**: Mỗi layer có trách nhiệm riêng biệt
3. **Testability**: Dễ dàng unit test từng layer
4. **Independent of UI/DB**: Domain không phụ thuộc framework

### Lợi ích:
- ✅ Dễ bảo trì và mở rộng
- ✅ Code tổ chức rõ ràng, dễ đọc
- ✅ Dễ thay đổi database/framework
- ✅ Testable và maintainable
- ✅ Team collaboration tốt hơn

## 📝 Ghi chú

- Tất cả entities sử dụng **soft delete** (IsDeleted flag)
- Hỗ trợ **concurrency control** với RowVersion
- Audit fields tự động cập nhật (CreatedAt, UpdatedAt, DeletedAt)
- Query filters tự động loại bỏ bản ghi đã xóa

## 🤝 Contributing

Khi implement thêm features:

1. Tạo Entity trong **Domain Layer**
2. Tạo Configuration trong **Infrastructure/Configurations**
3. Tạo DTO trong **Application/DTOs**
4. Tạo Service Interface trong **Application/Services**
5. Implement Service trong **Infrastructure/Services**
6. Tạo Controller trong **API/Controllers**
7. Register service trong **Program.cs**

## 📄 License

This project is licensed under the MIT License.

---

**Happy Coding! 🚀**

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PTJ.Application.Interfaces;
using PTJ.Application.Services;
using PTJ.Application.Validators.Auth;
using PTJ.Domain.Interfaces;
using PTJ.Infrastructure.Configuration;
using PTJ.Infrastructure.Data;
using PTJ.Infrastructure.Repositories;
using PTJ.Infrastructure.Services;
using System.Text;

namespace PTJ.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        // Add other services here as they are implemented
        // services.AddScoped<IProfileService, ProfileService>();
        // services.AddScoped<ICompanyService, CompanyService>();
        // services.AddScoped<IJobPostService, JobPostService>();
        // services.AddScoped<IUserService, UserService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work & Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // JWT Configuration
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        // Register all validators from the Application assembly
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }
}

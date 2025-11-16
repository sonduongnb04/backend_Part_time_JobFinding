using System.Net;
using System.Text.Json;
using PTJ.Application.DTOs.Common;

namespace PTJ.WebAPI.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            UnauthorizedAccessException => new
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Response = ApiResponse<object>.ErrorResult(
                    exception.Message,
                    new List<string> { exception.Message })
            },
            InvalidOperationException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = ApiResponse<object>.ErrorResult(
                    exception.Message,
                    new List<string> { exception.Message })
            },
            FileNotFoundException => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Response = ApiResponse<object>.ErrorResult(
                    "Resource not found",
                    new List<string> { exception.Message })
            },
            ArgumentNullException or ArgumentException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = ApiResponse<object>.ErrorResult(
                    "Invalid request",
                    new List<string> { exception.Message })
            },
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Response = ApiResponse<object>.ErrorResult(
                    "An internal server error occurred",
                    new List<string> { "Please contact support if the problem persists" })
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response.Response, jsonOptions));
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace BuildingBlocks.Extensions
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ve)
            {
                _logger.LogWarning(ve, "Validation exception occurred");

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                httpContext.Response.ContentType = "application/problem+json";

                var errors = ve.Errors.Select(e => e.ErrorMessage).ToArray();

                var problem = new ProblemDetails
                {
                    Title = "خطای اعتبارسنجی",
                    Status = httpContext.Response.StatusCode,
                    Detail = string.Join(" | ", errors),
                    Instance = httpContext.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await httpContext.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "خطای داخلی سرور",
                    Status = httpContext.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}

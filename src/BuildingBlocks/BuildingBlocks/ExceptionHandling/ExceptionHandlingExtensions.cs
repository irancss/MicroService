using BuildingBlocks.Extensions;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiExceptionMiddleware>();
        }
    }
}

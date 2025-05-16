using ApiGateway;
using Microsoft.Extensions.Caching.Memory;

namespace ApiGateway
{
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheMiddleware> _logger;

        public CacheMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<CacheMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // فقط درخواست‌های GET رو cache می‌کنیم
            if (context.Request.Method != "GET")
            {
                await _next(context);
                return;
            }

            // کلید cache رو بر اساس path و query string می‌سازیم
            var cacheKey = GenerateCacheKey(context.Request);

            // چک می‌کنیم آیا پاسخ تو cache هست یا نه
            if (_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                _logger.LogInformation($"Serving from cache: {cacheKey}");
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // جریان اصلی پاسخ رو نگه می‌داریم و یه جریان موقت می‌سازیم
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // درخواست رو به backend می‌فرستیم
            await _next(context);

            // اگه درخواست موفق بود (200)، پاسخ رو cache می‌کنیم
            if (context.Response.StatusCode == 200)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                _cache.Set(cacheKey, responseText, TimeSpan.FromMinutes(10)); // 30 دقیقه ذخیره می‌شه
                _logger.LogInformation($"Cached response for: {cacheKey}");

                // پاسخ رو به جریان اصلی کپی می‌کنیم
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                // اگه موفق نبود، جریان اصلی رو برمی‌گردونیم
                context.Response.Body = originalBodyStream;
            }
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            // کلید cache شامل path و query string
            var queryString = request.QueryString.HasValue ? request.QueryString.Value : "";
            return $"{request.Path}{queryString}";
        }
    }
}
// Extension method برای ثبت middleware
public static class CacheMiddlewareExtensions
{
    public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CacheMiddleware>();
    }
}

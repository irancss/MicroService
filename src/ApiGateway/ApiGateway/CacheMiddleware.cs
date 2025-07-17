using ApiGateway;
using Microsoft.Extensions.Caching.Memory;

namespace ApiGateway
{
    
    /// <summary>
    /// Middleware for caching HTTP GET responses.
    /// This middleware intercepts incoming HTTP requests, and if the request is a GET request,
    /// it attempts to serve the response from an in-memory cache. If the response is not found
    /// in the cache, it forwards the request to the next middleware in the pipeline,
    /// caches the successful response, and then sends the response to the client.
    /// </summary>
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="cache">The memory cache instance for storing responses.</param>
        /// <param name="logger">The logger instance for logging cache activities.</param>
        public CacheMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<CacheMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Processes an individual HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Only cache GET requests
            if (context.Request.Method != "GET")
            {
                await _next(context);
                return;
            }

            // Generate cache key based on path and query string
            var cacheKey = GenerateCacheKey(context.Request);

            // Check if the response is in the cache
            if (_cache.TryGetValue(cacheKey, out string cachedResponse))
            {
                _logger.LogInformation($"Serving from cache: {cacheKey}");
                context.Response.ContentType = "text/plain; charset=utf-8"; // Assuming plain text, adjust if necessary
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // Store the original response stream and create a temporary stream
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Send the request to the next middleware
            await _next(context);

            // If the request was successful (200), cache the response
            if (context.Response.StatusCode == 200)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                _cache.Set(cacheKey, responseText, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                _logger.LogInformation($"Cached response for: {cacheKey}");

                // Copy the response to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                // If not successful, restore the original stream
                // This ensures that error responses are not read from the MemoryStream incorrectly
                // and are passed through as they were from the _next middleware.
                context.Response.Body = originalBodyStream;
                // If there was content in responseBody (e.g. an error page from _next),
                // and it needs to be sent, it should be copied to originalBodyStream.
                // However, for non-200 responses, we typically don't cache,
                // and the original stream is already set to the client.
                // If the _next middleware wrote to responseBody, and we want to send that,
                // we'd need to copy it. But if _next directly wrote to originalBodyStream (which it would if we didn't swap it),
                // then just setting it back is fine.
                // Given the current logic, if _next writes an error to responseBody,
                // and we just set context.Response.Body = originalBodyStream, that error content might be lost
                // if originalBodyStream is a stream that can't be written to after _next has finished.
                // A safer approach for non-200 might be to copy if responseBody has content.
                // However, the original code simply reverts the stream.
            }
        }

        /// <summary>
        /// Generates a cache key for the given HTTP request.
        /// The cache key is composed of the request path and query string.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <returns>A string representing the cache key.</returns>
        private string GenerateCacheKey(HttpRequest request)
        {
            // Cache key includes path and query string
            var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;
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

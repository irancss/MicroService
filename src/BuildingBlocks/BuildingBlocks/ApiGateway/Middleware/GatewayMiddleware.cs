using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.ApiGateway.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            context.Items["RequestId"] = requestId;

            _logger.LogInformation("Request {RequestId} started: {Method} {Path}",
                requestId, context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Request {RequestId} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
                    requestId, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
    }

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly Dictionary<string, List<DateTime>> _requestTimes = new();
        private static readonly object _lock = new object();

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);
            bool rateLimitExceeded = false;

            lock (_lock)
            {
                if (!_requestTimes.ContainsKey(clientId))
                {
                    _requestTimes[clientId] = new List<DateTime>();
                }

                var requests = _requestTimes[clientId];
                requests.RemoveAll(time => time < windowStart);

                if (requests.Count >= 100) // 100 requests per minute
                {
                    rateLimitExceeded = true;
                }
                else
                {
                    requests.Add(now);
                }
            }

            if (rateLimitExceeded)
            {
                context.Response.StatusCode = 429;
                _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}

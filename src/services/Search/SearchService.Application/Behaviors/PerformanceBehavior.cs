using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SearchService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior for performance monitoring
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // Log slow requests (>500ms)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("Slow request detected: {RequestName} took {ElapsedMilliseconds}ms", 
                requestName, elapsedMilliseconds);
        }

        return response;
    }
}

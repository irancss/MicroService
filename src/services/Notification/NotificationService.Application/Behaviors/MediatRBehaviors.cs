using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace NotificationService.Application.Behaviors;

public class ResilienceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ResilienceBehavior<TRequest, TResponse>> _logger;

    public ResilienceBehavior(ILogger<ResilienceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only apply retry policy to notification commands, not queries
        var requestName = typeof(TRequest).Name;
        if (!requestName.Contains("Command"))
        {
            return await next();
        }

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry attempt {RetryCount} for {RequestName} after {Delay}ms. Exception: {Exception}",
                        retryCount, requestName, timespan.TotalMilliseconds, outcome.Exception?.Message);
                });

        try
        {
            return await retryPolicy.ExecuteAsync(async () =>
            {
                return await next();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute {RequestName} after all retry attempts", requestName);
            throw;
        }
    }
}

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            var response = await next();
            var elapsed = DateTime.UtcNow - startTime;
            
            _logger.LogInformation("Completed {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, elapsed.TotalMilliseconds);
                
            return response;
        }
        catch (Exception ex)
        {
            var elapsed = DateTime.UtcNow - startTime;
            
            _logger.LogError(ex, "Failed {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, elapsed.TotalMilliseconds);
                
            throw;
        }
    }
}

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<FluentValidation.IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<FluentValidation.IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new FluentValidation.ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new FluentValidation.ValidationException(failures);
            }
        }

        return await next();
    }
}

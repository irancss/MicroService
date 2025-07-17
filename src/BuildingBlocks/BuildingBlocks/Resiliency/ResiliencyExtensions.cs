using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net;

namespace BuildingBlocks.Resiliency
{
    /// <summary>
    /// Resiliency patterns using Polly for HTTP clients and general operations
    /// </summary>
    public static class ResiliencyExtensions
    {
        public static IServiceCollection AddResiliency(this IServiceCollection services, IConfiguration configuration)
        {
            var resiliencySettings = configuration.GetSection("Resiliency").Get<ResiliencySettings>() ?? new ResiliencySettings();
            services.Configure<ResiliencySettings>(configuration.GetSection("Resiliency"));

            // Add HTTP client with Polly policies
            services.AddHttpClient("resilient-client")
                .AddPolicyHandler(GetRetryPolicy(resiliencySettings))
                .AddPolicyHandler(GetCircuitBreakerPolicy(resiliencySettings))
                .AddPolicyHandler(GetTimeoutPolicy(resiliencySettings));

            // Register resilient HTTP client factory
            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ResiliencySettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    retryCount: settings.RetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogWarning("Retry {RetryCount} for {OperationKey} in {TimeSpan}ms due to: {Exception}",
                            retryCount, context.OperationKey, timespan.TotalMilliseconds, outcome.Exception?.Message);
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ResiliencySettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: settings.CircuitBreakerFailureThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationOfBreakInSeconds),
                    onBreak: (delegateResult, timespan, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogWarning("Circuit breaker opened for {Duration}s for operation: {OperationKey}",
                            timespan.TotalSeconds, context.OperationKey);
                    },
                    onReset: (context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogInformation("Circuit breaker reset for operation: {OperationKey}", context.OperationKey);
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ResiliencySettings settings)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(settings.TimeoutInSeconds);
        }
    }

    public class ResiliencySettings
    {
        public int RetryCount { get; set; } = 3;
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        public int CircuitBreakerDurationOfBreakInSeconds { get; set; } = 30;
        public int TimeoutInSeconds { get; set; } = 10;
    }

    public interface IResilientHttpClientFactory
    {
        HttpClient CreateClient(string name = "resilient-client");
        Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> operation, string operationKey = "http-operation");
    }

    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ResilientHttpClientFactory> _logger;
        private readonly ResiliencySettings _settings;

        public ResilientHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<ResilientHttpClientFactory> logger,
            Microsoft.Extensions.Options.IOptions<ResiliencySettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settings = settings.Value;
        }

        public HttpClient CreateClient(string name = "resilient-client")
        {
            return _httpClientFactory.CreateClient(name);
        }

        public async Task<T> ExecuteAsync<T>(Func<HttpClient, Task<T>> operation, string operationKey = "http-operation")
        {
            var policy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    retryCount: _settings.RetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} for {OperationKey} in {TimeSpan}ms due to: {Exception}",
                            retryCount, operationKey, timespan.TotalMilliseconds, exception.Message);
                    });

            var context = new Context(operationKey);
            context.Add("logger", _logger);

            using var httpClient = CreateClient();
            return await policy.ExecuteAsync(async (ctx) => await operation(httpClient), context);
        }
    }

    public static class PollyContextExtensions
    {
        public static Microsoft.Extensions.Logging.ILogger? GetLogger(this Context context)
        {
            return context.TryGetValue("logger", out var logger) ? logger as Microsoft.Extensions.Logging.ILogger : null;
        }
    }
}

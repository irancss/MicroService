using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly.Registry;

namespace BuildingBlocks.Resiliency
{
    public static class ResiliencyExtensions
    {
        public static IServiceCollection AddResiliency(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ResiliencySettings>(configuration.GetSection("Resiliency"));
            var resiliencySettings = configuration.GetSection("Resiliency").Get<ResiliencySettings>() ?? new ResiliencySettings();

            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add("default-retry", GetRetryPolicy(resiliencySettings));
            policyRegistry.Add("default-circuit-breaker", GetCircuitBreakerPolicy(resiliencySettings));
            policyRegistry.Add("default-timeout", GetTimeoutPolicy(resiliencySettings));

            return services;
        }

        public static IHttpClientBuilder AddResilientHttpPolicies(this IHttpClientBuilder builder, IConfiguration configuration)
        {
            var resiliencySettings = configuration.GetSection("Resiliency").Get<ResiliencySettings>() ?? new ResiliencySettings();

            return builder
                .AddPolicyHandler(GetRetryPolicy(resiliencySettings))
                .AddPolicyHandler(GetCircuitBreakerPolicy(resiliencySettings))
                .AddPolicyHandler(GetTimeoutPolicy(resiliencySettings));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ResiliencySettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    retryCount: settings.RetryCount,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                        TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        context.GetLogger()?.LogWarning(
                            "Retry {RetryCount} for {PolicyKey} in {TimeSpan}ms, due to: {ExceptionMessage}",
                            retryCount, context.PolicyKey, timespan.TotalMilliseconds, outcome.Exception?.Message);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ResiliencySettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: settings.CircuitBreakerFailureThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationOfBreakInSeconds),
                    onBreak: (result, timespan, context) => {
                        context.GetLogger()?.LogWarning("Circuit breaker opened for {Duration}s for {PolicyKey}, due to: {ExceptionMessage}",
                            timespan.TotalSeconds, context.PolicyKey, result.Exception?.Message);
                    },
                    onReset: (context) => {
                        context.GetLogger()?.LogInformation("Circuit breaker reset for {PolicyKey}", context.PolicyKey);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ResiliencySettings settings)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(settings.TimeoutInSeconds));
        }
    }

    public class ResiliencySettings
    {
        public int RetryCount { get; set; } = 3;
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        public int CircuitBreakerDurationOfBreakInSeconds { get; set; } = 30;
        public int TimeoutInSeconds { get; set; } = 10;
    }

    public static class PollyContextExtensions
    {
        private static readonly string LoggerKey = "ILogger";

        public static Context WithLogger(this Context context, ILogger logger)
        {
            context[LoggerKey] = logger;
            return context;
        }

        public static ILogger? GetLogger(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out var loggerObject) && loggerObject is ILogger logger)
            {
                return logger;
            }
            return null;
        }
    }
}
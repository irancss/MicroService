using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BuildingBlocks.HealthCheck
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddBuildingBlocksHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var healthCheckBuilder = services.AddHealthChecks();

            // Add RabbitMQ health check
            var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMQ");
            if (!string.IsNullOrEmpty(rabbitMqConnectionString))
            {
                healthCheckBuilder.AddRabbitMQ(rabbitMqConnectionString, name: "rabbitmq");
            }

            // Add Consul health check
            var consulSettings = configuration.GetSection("Consul").Get<ConsulHealthCheckSettings>();
            if (consulSettings != null)
            {
                healthCheckBuilder.AddConsul(options =>
                {
                    options.HostName = consulSettings.Host;
                    options.Port = consulSettings.Port;
                }, name: "consul");
            }

            // Add custom application health check
            healthCheckBuilder.AddCheck<ApplicationHealthCheck>("application");

            return services;
        }

        public static IApplicationBuilder UseBuildingBlocksHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponse
            });

            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = WriteHealthCheckResponse
            });

            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteHealthCheckResponse
            });

            return app;
        }

        private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    exception = entry.Value.Exception?.Message,
                    duration = entry.Value.Duration.ToString(),
                    data = entry.Value.Data
                }),
                totalDuration = report.TotalDuration
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }));
        }
    }

    public class ConsulHealthCheckSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 8500;
    }

    public class ApplicationHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Add your custom health check logic here
            // For example, check database connectivity, external services, etc.
            
            try
            {
                // Simulate health check logic
                var isHealthy = true; // Replace with actual health check logic
                
                if (isHealthy)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Application is healthy"));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Application is unhealthy"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Health check failed: {ex.Message}"));
            }
        }
    }
}

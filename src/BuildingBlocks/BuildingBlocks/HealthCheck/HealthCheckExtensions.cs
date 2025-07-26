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

            // Liveness check: Is the app process alive?
            healthCheckBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

            // Readiness checks: Is the app ready to handle requests (dependencies are up)?

            // RabbitMQ health check
            var rabbitMqConnectionString = configuration["RabbitMQ:Host"]; // Read host instead of a full connection string
            if (!string.IsNullOrEmpty(rabbitMqConnectionString))
            {
                healthCheckBuilder.AddRabbitMQ(
                    // رشته را به یک شیء Uri تبدیل می‌کنیم
                    new Uri($"amqp://{configuration["RabbitMQ:Username"]}:{configuration["RabbitMQ:Password"]}@{configuration["RabbitMQ:Host"]}{configuration["RabbitMQ:VirtualHost"]}"),
                    name: "rabbitmq",
                    tags: new[] { "ready" });
            }

            // Consul health check
            var consulAddress = configuration["Consul:Address"];
            if (!string.IsNullOrEmpty(consulAddress))
            {
                healthCheckBuilder.AddConsul(options =>
                {
                    var consulUri = new Uri(consulAddress);
                    options.HostName = consulUri.Host;
                    options.Port = consulUri.Port;
                    // اگر از HTTPS برای کنسول استفاده می‌کنید، این خط را اضافه کنید:
                    // options.Scheme = consulUri.Scheme; 
                }, name: "consul", tags: new[] { "ready" });
            }

            // Database health check (PostgreSQL)
            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(dbConnectionString))
            {
                healthCheckBuilder.AddNpgSql(dbConnectionString, name: "database", tags: new[] { "ready" });
            }

            // Redis health check
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                healthCheckBuilder.AddRedis(redisConnectionString, name: "redis", tags: new[] { "ready" });
            }

            return services;
        }

        public static IApplicationBuilder UseBuildingBlocksHealthChecks(this IApplicationBuilder app)
        {
            // Responds on /health/live - Checks if the app is running
            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live"),
                ResponseWriter = WriteHealthCheckResponse
            });

            // Responds on /health/ready - Checks if the app and its dependencies are ready to accept traffic
            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = _ => true, // Check all, including dependencies
                ResponseWriter = WriteHealthCheckResponse
            });

            return app;
        }

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration.TotalMilliseconds,
                results = report.Entries.ToDictionary(
                    e => e.Key,
                    e => new
                    {
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.TotalMilliseconds,
                        exception = e.Value.Exception?.Message,
                        data = e.Value.Data
                    })
            }, options);

            return context.Response.WriteAsync(json);
        }
    }
}
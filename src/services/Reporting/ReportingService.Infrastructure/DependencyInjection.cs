using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportingService.Application.Interfaces;
using ReportingService.Infrastructure.BackgroundJobs;
using ReportingService.Infrastructure.Data;
using ReportingService.Infrastructure.Messaging.Consumers;
using ReportingService.Infrastructure.Repositories;

namespace ReportingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ReportingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ReportingDbContext).Assembly.FullName)));

        // Repository
        services.AddScoped<IReportingRepository, ReportingRepository>();

        // MassTransit with RabbitMQ
        services.AddMassTransit(x =>
        {
            // Add consumers
            x.AddConsumer<OrderCompletedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ") ?? "rabbitmq://localhost");

                cfg.ReceiveEndpoint("order-completed-events", e =>
                {
                    e.ConfigureConsumer<OrderCompletedEventConsumer>(context);
                    
                    // Configure retry policy (updated method)
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    
                    // Configure outbox (updated method)
                    e.UseInMemoryOutbox(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        // Hangfire with PostgreSQL
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection"));
        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount;
            options.Queues = new[] { "critical", "default", "low" };
        });

        // Background jobs
        services.AddScoped<SalesAggregationJobs>();

        return services;
    }

    public static void ConfigureHangfireJobs()
    {
        // Schedule daily sales aggregation to run every day at 2 AM
        RecurringJob.AddOrUpdate<SalesAggregationJobs>(
            "daily-sales-aggregation",
            job => job.RunDailySalesAggregation(),
            Cron.Daily(2)); // 2 AM every day

        // Schedule weekly cleanup to run every Sunday at 3 AM
        RecurringJob.AddOrUpdate<SalesAggregationJobs>(
            "weekly-cleanup",
            job => job.CleanupOldData(1095), // 3 years retention
            Cron.Weekly(DayOfWeek.Sunday, 3)); // 3 AM every Sunday
    }
}

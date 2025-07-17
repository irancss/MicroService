using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Cart.Application.Interfaces;
using Cart.Infrastructure.Repositories;
using Cart.Infrastructure.Services;
using Cart.Infrastructure.BackgroundJobs;

namespace Cart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Redis Configuration
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(connectionString);
        });

        // Repository Registration
        services.AddScoped<ICartRepository, RedisCartRepository>();
        services.AddScoped<ICartConfigurationService, CartConfigurationService>();

        // gRPC Clients Configuration
        services.Configure<GrpcSettings>(configuration.GetSection("GrpcSettings"));
        services.AddScoped<IInventoryGrpcClient, InventoryGrpcClient>();
        services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();

        // MassTransit Configuration
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = configuration.GetSection("RabbitMQ");
                cfg.Host(rabbitMqSettings["Host"], h =>
                {
                    h.Username(rabbitMqSettings["Username"]);
                    h.Password(rabbitMqSettings["Password"]);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        // Event Publisher
        services.AddScoped<IEventPublisher, EventPublisher>();

        // Notification Service
        services.AddScoped<INotificationService, NotificationService>();

        // Hangfire Configuration
        services.AddHangfire(config =>
        {
            var connectionString = configuration.GetConnectionString("Hangfire");
            config.UsePostgreSqlStorage(connectionString);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
        });

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount;
            options.Queues = new[] { "default", "critical", "normal", "low" };
        });

        // Background Jobs
        services.AddScoped<CartAbandonmentJob>();
        services.AddHostedService<HangfireJobScheduler>();

        return services;
    }
}

using BuildingBlocks.Infrastructure.Caching;
using Cart.Application.Interfaces;
using Cart.Infrastructure.BackgroundJobs;
using Cart.Infrastructure.Data;
using Cart.Infrastructure.Repositories;
using Cart.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Cart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddScoped<IDistributedCacheService, RedisCacheService>();

        // PostgreSQL DbContext for NextPurchaseCart
        services.AddDbContext<CartDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        // Repositories
        services.AddScoped<IActiveCartRepository, ActiveCartRepository>();
        services.AddScoped<INextPurchaseCartRepository, NextPurchaseCartRepository>();

        // Services
        services.AddScoped<ICartConfigurationService, RedisCartConfigurationService>();
        // INotificationService از IMessageBus استفاده می‌کند که در Program.cs ثبت می‌شود.
        services.AddScoped<INotificationService, NotificationService>();

        // gRPC Clients (using BuildingBlocks ServiceMesh)
        services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();
        services.AddScoped<IInventoryGrpcClient, InventoryGrpcClient>();

        // Background Services (replaces Hangfire)
        services.AddHostedService<ActiveCartExpirationService>();
        // services.AddHostedService<AbandonedCartProcessorService>(); // For notifications

        return services;
    }
}

using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.ServiceMesh.Http;
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
        // 1. Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddScoped<IDistributedCacheService, RedisCacheService>(); // از BuildingBlocks

        // 2. PostgreSQL DbContext
        services.AddDbContext<CartDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        // 3. Repositories
        services.AddScoped<IActiveCartRepository, ActiveCartRepository>();
        services.AddScoped<INextPurchaseCartRepository, NextPurchaseCartRepository>();

        // 4. Services
        services.AddScoped<ICartConfigurationService, RedisCartConfigurationService>();
        services.AddScoped<INotificationService, NotificationService>();

        // 5. HTTP Clients for inter-service communication
        // این کلاینت‌ها از IServiceMeshHttpClient (که در Program.cs ثبت شده) استفاده می‌کنند.
        services.AddScoped<ICatalogClient, CatalogHttpClient>();
        services.AddScoped<IInventoryClient, InventoryHttpClient>();

        // 6. Background Job Logic (فقط کلاس منطق، نه سرور Hangfire)
        services.AddScoped<CartJobs>(); // این کلاس توسط Hangfire در لایه API فراخوانی خواهد شد

        return services;
    }
}

public static class InventoryDefaults
{
    public const string Name = "InventoryGrpcClient";
    public const string ServiceName = "inventory-service"; // نام ثبت شده در Consul
}
// public static class CatalogDefaults
// {
//     public const string Name = "CatalogGrpcClient";
//     public const string ServiceName = "catalog-service";
// }
using BuildingBlocks.Infrastructure.Caching;
using Cart.Application.Interfaces;
using Cart.Infrastructure.Repositories;
using Cart.Infrastructure.Services;
using StackExchange.Redis;

namespace Cart.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Redis
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            services.AddScoped<IDistributedCacheService, RedisCacheService>(); // از BuildingBlocks

            // Repositories
            //services.AddScoped<ICartRepository, RedisCartRepository>();

            // Services
            services.AddScoped<ICartConfigurationService, RedisCartConfigurationService>();
            services.AddScoped<INotificationService, NotificationService>();

            // gRPC Clients
            services.AddScoped<ICatalogClient, CatalogHttpClient>();
            services.AddScoped<IInventoryClient, InventoryHttpClient>();

            // (IEventPublisher با IEventBus از BuildingBlocks جایگزین شده و مستقیم تزریق می‌شود)

            return services;
        }
    }
}

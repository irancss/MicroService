using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MassTransit;
using DiscountService.Application.Interfaces;
using DiscountService.Infrastructure.Data;
using DiscountService.Infrastructure.Repositories;
using DiscountService.Infrastructure.Services;
using DiscountService.Infrastructure.Messaging;

namespace DiscountService.Infrastructure;

/// <summary>
/// Extension method to register infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Entity Framework
        services.AddDbContext<DiscountDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        // Register repositories
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IDiscountUsageHistoryRepository, DiscountUsageHistoryRepository>();

        // Register services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register application database context interface
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<DiscountDbContext>());

        // Register MassTransit for RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetConnectionString("RabbitMQ");
                cfg.Host(rabbitMqConfig);

                cfg.ReceiveEndpoint("discount-order-created", e =>
                {
                    e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
                    e.UseMessageRetry(r => r.Exponential(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(5)));
                });
            });
        });

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Shopping.SharedKernel.Abstractions;
using Shopping.SharedKernel.Infrastructure.Persistence;
using Shopping.SharedKernel.Infrastructure.EventBus;
using Shopping.SharedKernel.Infrastructure.Caching;
using Shopping.SharedKernel.Infrastructure.Outbox;
using Shopping.SharedKernel.Core.Contracts;
using Shopping.SharedKernel.Core.Services;
using MediatR;
using System.Reflection;

namespace Shopping.SharedKernel.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services, IConfiguration configuration)
    {
        // Core Services
        services.AddScoped<IDateTime, DateTimeService>();
        
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }

    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<IRepositoryFactory, UnitOfWork<TContext>>();
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
        return services;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        // For development/testing - use in-memory implementation
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IIntegrationEventBus, InMemoryIntegrationEventBus>();
        
        // For production, replace with:
        // services.AddScoped<IEventBus, RabbitMQEventBus>();
        // services.AddScoped<IIntegrationEventBus, RabbitMQIntegrationEventBus>();
        
        return services;
    }

    public static IServiceCollection AddDistributedCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Distributed cache configuration
        // services.AddScoped<IDistributedCacheService, RedisDistributedCacheService>();
        return services;
    }

    public static IServiceCollection AddOutboxPattern(this IServiceCollection services)
    {
        services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();
        services.AddScoped<IOutboxEventProcessor, OutboxEventProcessor>();
        return services;
    }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2>(this IServiceCollection services)
            where TContext1 : DbContext
            where TContext2 : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
            services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2, TContext3>(
            this IServiceCollection services)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
            services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();
            services.AddScoped<IUnitOfWork<TContext3>, UnitOfWork<TContext3>>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2, TContext3, TContext4>(
            this IServiceCollection services)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
            where TContext4 : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
            services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();
            services.AddScoped<IUnitOfWork<TContext3>, UnitOfWork<TContext3>>();
            services.AddScoped<IUnitOfWork<TContext4>, UnitOfWork<TContext4>>();
            return services;
        }
    }
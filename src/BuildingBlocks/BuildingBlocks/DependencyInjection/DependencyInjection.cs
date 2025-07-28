using BuildingBlocks.Abstractions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Core.Contracts;
using BuildingBlocks.Core.Services;
using BuildingBlocks.Identity;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Interceptors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.CrossCutting.Correlation;
using BuildingBlocks.Messaging;

namespace BuildingBlocks.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        // Core Services
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>(); // [جدید] ثبت سرویس کاربر جاری

        // MediatR
        if (assemblies.Any())
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);

                // [جدید] ثبت Pipeline Behaviors
                // ترتیب ثبت مهم است. اعتبارسنجی باید قبل از تراکنش اجرا شود.
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            });
        }

        // Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));


        // Cross-cutting
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();

        // [جدید] ثبت تمام Validator ها از اسمبلی‌های داده شده
        if (assemblies.Any())
        {
            services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Scoped);
        }

        // Messaging
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        // Interceptors
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddSingleton<DomainEventDispatchingInterceptor>(); 

        return services;
    }

    /// <summary>
    /// [اصلاح شد] متد ساده شده برای ثبت UnitOfWork.
    /// اگر به چندین DbContext نیاز دارید، این متد را به ازای هر کدام یک بار فراخوانی کنید.
    /// ex: services.AddUnitOfWork<OrderDbContext>();
    ///     services.AddUnitOfWork<CatalogDbContext>();
    /// </summary>
    /// <summary>
    /// [اصلاح شد] متد ساده شده برای ثبت UnitOfWork و DbContext.
    /// این متد اکنون DbContext را نیز ثبت می‌کند تا در TransactionBehavior قابل تزریق باشد.
    /// </summary>
    public static IServiceCollection AddUnitOfWorkAndDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        // ثبت DbContext به صورت Scoped تا در TransactionBehavior قابل تزریق باشد
        services.AddScoped<DbContext, TContext>();

        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IUnitOfWork<TContext>>());

        return services;
    }
}

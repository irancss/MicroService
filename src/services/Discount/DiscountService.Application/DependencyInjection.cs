using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using System.Reflection;
using DiscountService.Domain.Services;

namespace DiscountService.Application;

/// <summary>
/// Extension method to register application services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register AutoMapper
        services.AddAutoMapper(cfg => {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register domain services
        services.AddScoped<IDiscountCalculationService, DiscountCalculationService>();

        return services;
    }
}

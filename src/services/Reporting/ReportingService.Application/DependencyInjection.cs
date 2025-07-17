using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ReportingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // Register FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Register AutoMapper
        services.AddAutoMapper(assembly);

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Cart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR و Validators قبلاً توسط AddSharedKernel از BuildingBlocks ثبت شده‌اند.
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

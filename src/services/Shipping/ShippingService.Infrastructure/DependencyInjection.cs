using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShippingService.Application.Services;
using ShippingService.Domain.Services;
using ShippingService.Infrastructure.Data;
using ShippingService.Infrastructure.Services;

namespace ShippingService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ShippingDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register domain services
            services.AddScoped<IShippingCostCalculatorService, ShippingCostCalculatorService>();
            services.AddScoped<IRouteOptimizationService, GoogleRouteOptimizationService>();
            services.AddScoped<INotificationService, TwilioNotificationService>();
            services.AddScoped<Domain.Services.IPremiumSubscriptionService, PremiumSubscriptionService>();
            services.AddScoped<Domain.Services.IFreeShippingRuleService, FreeShippingRuleService>();

            return services;
        }
    }
}
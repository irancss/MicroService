using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VendorService.Infrastructure.Data.Configurations;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VendorDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=VendorDb;Username=postgres;Password=123");
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Data.Configurations
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=OrderDb;Username=postgres;Password=123");
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Domain.Interfaces;

namespace TicketService.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<ITicketRepository, TicketRepository>();


            services.AddDbContext<TicketDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=TicketDb;Username=postgres;Password=123");
            });
            return services;
        }
    }
}

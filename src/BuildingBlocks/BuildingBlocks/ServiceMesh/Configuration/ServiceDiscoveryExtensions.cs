using BuildingBlocks.Models;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.ServiceDiscovery
{
    public static class ServiceDiscoveryExtensions
    {
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulSettings>(configuration.GetSection("Consul"));

            services.AddSingleton<IConsulClient, ConsulClient>(p =>
            {
                var settings = p.GetRequiredService<IOptions<ConsulSettings>>().Value;
                if (string.IsNullOrEmpty(settings.Address))
                {
                    throw new InvalidOperationException("Consul address is not configured.");
                }
                return new ConsulClient(config => { config.Address = new Uri(settings.Address); });
            });

            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            services.AddSingleton<IHostedService, ConsulServiceRegistrationService>();

            return services;
        }
    }
}
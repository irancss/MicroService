using BuildingBlocks.Models;
using BuildingBlocks.ServiceDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.ServiceDiscovery
{
    public class ConsulServiceRegistrationService : IHostedService
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsulServiceRegistrationService> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private string? _serviceId;

        public ConsulServiceRegistrationService(
            IServiceDiscovery serviceDiscovery,
            IConfiguration configuration,
            ILogger<ConsulServiceRegistrationService> logger,
            IHostApplicationLifetime lifetime)
        {
            _serviceDiscovery = serviceDiscovery;
            _configuration = configuration;
            _logger = logger;
            _lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    var serviceConfig = _configuration.GetSection("Service");
                    var registration = new ServiceRegistration
                    {
                        ServiceId = serviceConfig["InstanceId"] ?? $"{serviceConfig["Name"]}-{Guid.NewGuid()}",
                        ServiceName = serviceConfig["Name"]!,
                        Address = serviceConfig["Address"]!,
                        Port = serviceConfig.GetValue<int>("Port"),
                        Tags = new[] { $"version={serviceConfig["Version"]}" }
                    };

                    _serviceId = registration.ServiceId;

                    await _serviceDiscovery.RegisterServiceAsync(registration, cancellationToken);
                    _logger.LogInformation("Service [{ServiceId}] registered with Consul.", _serviceId);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Failed to register service with Consul. Application will be terminated.");
                    _lifetime.StopApplication();
                }
            });
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_serviceId))
            {
                try
                {
                    await _serviceDiscovery.DeregisterServiceAsync(_serviceId, cancellationToken);
                    _logger.LogInformation("Service [{ServiceId}] deregistered from Consul.", _serviceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deregister service [{ServiceId}] from Consul.", _serviceId);
                }
            }
        }
    }
}
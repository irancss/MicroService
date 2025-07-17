using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consul;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh.LoadBalancing;
using BuildingBlocks.ServiceMesh.CircuitBreaker;
using BuildingBlocks.ServiceMesh.Http;

namespace BuildingBlocks.ServiceMesh.Configuration
{
    public static class ServiceMeshExtensions
    {
        public static IServiceCollection AddServiceMesh(this IServiceCollection services, IConfiguration configuration)
        {
            var consulSettings = configuration.GetSection("Consul").Get<ConsulSettings>();
            
            if (consulSettings == null)
            {
                throw new InvalidOperationException("Consul configuration is missing");
            }

            // Register Consul client
            services.AddSingleton<IConsulClient>(provider =>
            {
                return new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{consulSettings.Host}:{consulSettings.Port}");
                    config.Datacenter = consulSettings.Datacenter;
                });
            });

            // Register service discovery
            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            services.AddSingleton<ILoadBalancer, RoundRobinLoadBalancer>();
            services.AddSingleton<ICircuitBreakerFactory, CircuitBreakerFactory>();

            // Register HTTP client factory with service discovery
            services.AddHttpClient("ServiceMesh")
                .AddTypedClient<IServiceMeshHttpClient, ServiceMeshHttpClient>();

            // Register service registration background service
            services.AddHostedService<ServiceRegistrationService>();

            return services;
        }
    }

    public class ConsulSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 8500;
        public string Datacenter { get; set; } = "dc1";
    }

    public class ServiceMeshSettings
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public string Address { get; set; } = "localhost";
        public int Port { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool EnableHealthCheck { get; set; } = true;
        public bool EnableLoadBalancing { get; set; } = true;
        public bool EnableCircuitBreaker { get; set; } = true;
    }

    public class ServiceRegistrationService : BackgroundService
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceRegistrationService> _logger;
        private string? _serviceId;

        public ServiceRegistrationService(
            IServiceDiscovery serviceDiscovery,
            IConfiguration configuration,
            ILogger<ServiceRegistrationService> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var settings = _configuration.GetSection("ServiceMesh").Get<ServiceMeshSettings>();
            
            if (settings != null && !string.IsNullOrEmpty(settings.ServiceName))
            {
                _serviceId = settings.ServiceId ?? $"{settings.ServiceName}-{Guid.NewGuid()}";
                
                var registration = new ServiceRegistration
                {
                    ServiceId = _serviceId,
                    ServiceName = settings.ServiceName,
                    Address = settings.Address,
                    Port = settings.Port,
                    Tags = settings.Tags.ToArray()
                };

                try
                {
                    await _serviceDiscovery.RegisterServiceAsync(registration);
                    _logger.LogInformation("Service {ServiceName} registered successfully", settings.ServiceName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to register service {ServiceName}", settings.ServiceName);
                }
            }

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_serviceId))
            {
                try
                {
                    await _serviceDiscovery.DeregisterServiceAsync(_serviceId);
                    _logger.LogInformation("Service {ServiceId} deregistered successfully", _serviceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deregister service {ServiceId}", _serviceId);
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}

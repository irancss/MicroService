using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.ServiceDiscovery
{
    /// <summary>
    /// Service discovery interface for registering and discovering services
    /// </summary>
    public interface IConsulServiceDiscovery
    {
        Task RegisterServiceAsync(ServiceRegistration registration);
        Task<IEnumerable<ServiceInstance>> DiscoverServicesAsync(string serviceName);
        Task DeregisterServiceAsync(string serviceId);
        Task<bool> IsServiceHealthyAsync(string serviceId);
    }

    /// <summary>
    /// Service registration information
    /// </summary>
    public class ServiceRegistration
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string HealthCheckUrl { get; set; } = string.Empty;
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Service instance information
    /// </summary>
    public class ServiceInstance
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public bool IsHealthy { get; set; }
    }

    /// <summary>
    /// Consul service discovery implementation
    /// </summary>
    public class ConsulServiceDiscovery : IServiceDiscovery, IConsulServiceDiscovery
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger<ConsulServiceDiscovery> _logger;

        public ConsulServiceDiscovery(IConsulClient consulClient, ILogger<ConsulServiceDiscovery> logger)
        {
            _consulClient = consulClient;
            _logger = logger;
        }

        public async Task RegisterServiceAsync(ServiceRegistration registration)
        {
            var agentServiceRegistration = new AgentServiceRegistration
            {
                ID = registration.ServiceId,
                Name = registration.ServiceName,
                Address = registration.Address,
                Port = registration.Port,
                Tags = registration.Tags,
                Check = new AgentServiceCheck
                {
                    HTTP = registration.HealthCheckUrl,
                    Interval = registration.HealthCheckInterval,
                    Timeout = registration.HealthCheckTimeout,
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await _consulClient.Agent.ServiceRegister(agentServiceRegistration);
            _logger.LogInformation("Service {ServiceName} with ID {ServiceId} registered successfully", 
                registration.ServiceName, registration.ServiceId);
        }

        public async Task<IEnumerable<ServiceInstance>> DiscoverServicesAsync(string serviceName)
        {
            var services = await _consulClient.Health.Service(serviceName, tag: "", passingOnly: false);
            
            return services.Response.Select(service => new ServiceInstance
            {
                ServiceId = service.Service.ID,
                ServiceName = service.Service.Service,
                Address = service.Service.Address,
                Port = service.Service.Port,
                Tags = service.Service.Tags ?? Array.Empty<string>(),
                IsHealthy = service.Checks.All(check => check.Status == HealthStatus.Passing)
            });
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            await _consulClient.Agent.ServiceDeregister(serviceId);
            _logger.LogInformation("Service with ID {ServiceId} deregistered successfully", serviceId);
        }

        public async Task<bool> IsServiceHealthyAsync(string serviceId)
        {
            var health = await _consulClient.Health.Checks(serviceId);
            return health.Response.All(check => check.Status == HealthStatus.Passing);
        }

        // IServiceDiscovery implementations
        public async Task<ServiceInstance?> GetServiceInstanceAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            var instances = await GetServiceInstancesAsync(serviceName, cancellationToken);
            return instances.FirstOrDefault();
        }

        public async Task<IEnumerable<ServiceInstance>> GetServiceInstancesAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            return await DiscoverServicesAsync(serviceName);
        }

        public async Task RegisterServiceAsync(ServiceRegistration registration, CancellationToken cancellationToken = default)
        {
            await RegisterServiceAsync(registration);
        }

        public async Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
        {
            await _consulClient.Agent.ServiceDeregister(serviceId);
            _logger.LogInformation("Service with ID {ServiceId} deregistered successfully", serviceId);
        }
    }

    /// <summary>
    /// Configuration settings for Consul
    /// </summary>
    public class ConsulSettings
    {
        public string Address { get; set; } = "http://localhost:8500";
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = "localhost";
        public int Port { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Background service for automatic service registration and deregistration
    /// </summary>
    public class ConsulServiceRegistration : BackgroundService
    {
        private readonly IConsulServiceDiscovery _serviceDiscovery;
        private readonly ConsulSettings _consulSettings;
        private readonly ILogger<ConsulServiceRegistration> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public ConsulServiceRegistration(
            IConsulServiceDiscovery serviceDiscovery,
            IOptions<ConsulSettings> consulSettings,
            ILogger<ConsulServiceRegistration> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            _serviceDiscovery = serviceDiscovery;
            _consulSettings = consulSettings.Value;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _applicationLifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    var registration = new ServiceRegistration
                    {
                        ServiceId = _consulSettings.ServiceId,
                        ServiceName = _consulSettings.ServiceName,
                        Address = _consulSettings.ServiceAddress,
                        Port = _consulSettings.Port,
                        Tags = _consulSettings.Tags,
                        HealthCheckUrl = $"http://{_consulSettings.ServiceAddress}:{_consulSettings.Port}/health",
                        HealthCheckInterval = _consulSettings.HealthCheckInterval
                    };

                    await _serviceDiscovery.RegisterServiceAsync(registration);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to register service with Consul");
                }
            });

            _applicationLifetime.ApplicationStopping.Register(async () =>
            {
                try
                {
                    await _serviceDiscovery.DeregisterServiceAsync(_consulSettings.ServiceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deregister service from Consul");
                }
            });

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

    /// <summary>
    /// Extension methods for registering Consul service discovery
    /// </summary>
    public static class ConsulServiceDiscoveryExtensions
    {
        public static IServiceCollection AddConsulServiceDiscovery(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ConsulSettings>(configuration.GetSection("Consul"));
            
            services.AddSingleton<IConsulClient>(provider =>
            {
                var consulSettings = provider.GetRequiredService<IOptions<ConsulSettings>>().Value;
                return new ConsulClient(config =>
                {
                    config.Address = new Uri(consulSettings.Address);
                });
            });

            services.AddScoped<IConsulServiceDiscovery, ConsulServiceDiscovery>();
            services.AddHostedService<ConsulServiceRegistration>();

            return services;
        }
    }

    /// <summary>
    /// HTTP client that uses Consul for service discovery with client-side load balancing
    /// </summary>
    public interface IConsulServiceClient
    {
        Task<HttpResponseMessage> GetAsync(string serviceName, string endpoint);
        Task<HttpResponseMessage> PostAsync(string serviceName, string endpoint, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string serviceName, string endpoint, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string serviceName, string endpoint);
    }

    public class ConsulServiceClient : IConsulServiceClient
    {
        private readonly IConsulServiceDiscovery _serviceDiscovery;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConsulServiceClient> _logger;
        private readonly Random _random = new();

        public ConsulServiceClient(
            IConsulServiceDiscovery serviceDiscovery,
            HttpClient httpClient,
            ILogger<ConsulServiceClient> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetAsync(string serviceName, string endpoint)
        {
            var serviceUrl = await GetServiceUrlAsync(serviceName);
            return await _httpClient.GetAsync($"{serviceUrl}/{endpoint.TrimStart('/')}");
        }

        public async Task<HttpResponseMessage> PostAsync(string serviceName, string endpoint, HttpContent content)
        {
            var serviceUrl = await GetServiceUrlAsync(serviceName);
            return await _httpClient.PostAsync($"{serviceUrl}/{endpoint.TrimStart('/')}", content);
        }

        public async Task<HttpResponseMessage> PutAsync(string serviceName, string endpoint, HttpContent content)
        {
            var serviceUrl = await GetServiceUrlAsync(serviceName);
            return await _httpClient.PutAsync($"{serviceUrl}/{endpoint.TrimStart('/')}", content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string serviceName, string endpoint)
        {
            var serviceUrl = await GetServiceUrlAsync(serviceName);
            return await _httpClient.DeleteAsync($"{serviceUrl}/{endpoint.TrimStart('/')}");
        }

        private async Task<string> GetServiceUrlAsync(string serviceName)
        {
            var services = await _serviceDiscovery.DiscoverServicesAsync(serviceName);
            var healthyServices = services.Where(s => s.IsHealthy).ToList();

            if (!healthyServices.Any())
            {
                throw new InvalidOperationException($"No healthy instances found for service: {serviceName}");
            }

            // Simple round-robin load balancing
            var selectedService = healthyServices[_random.Next(healthyServices.Count)];
            var serviceUrl = $"http://{selectedService.Address}:{selectedService.Port}";

            _logger.LogDebug("Selected service instance: {ServiceUrl} for service: {ServiceName}", 
                serviceUrl, serviceName);

            return serviceUrl;
        }
    }
}
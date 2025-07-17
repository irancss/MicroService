using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.ServiceMesh.ServiceDiscovery
{
    public interface IServiceDiscovery
    {
        Task RegisterServiceAsync(ServiceRegistration registration);
        Task DeregisterServiceAsync(string serviceId);
        Task<IEnumerable<ServiceInstance>> DiscoverServicesAsync(string serviceName);
        Task<ServiceInstance?> DiscoverServiceAsync(string serviceName);
    }

    public class ConsulServiceDiscovery : IServiceDiscovery
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
            var consulRegistration = new AgentServiceRegistration
            {
                ID = registration.Id,
                Name = registration.ServiceName,
                Address = registration.Address,
                Port = registration.Port,
                Tags = registration.Tags?.ToArray(),
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{registration.Address}:{registration.Port}/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await _consulClient.Agent.ServiceRegister(consulRegistration);
            _logger.LogInformation("Service {ServiceName} registered with ID {ServiceId}", 
                registration.ServiceName, registration.Id);
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            await _consulClient.Agent.ServiceDeregister(serviceId);
            _logger.LogInformation("Service {ServiceId} deregistered", serviceId);
        }

        public async Task<IEnumerable<ServiceInstance>> DiscoverServicesAsync(string serviceName)
        {
            var services = await _consulClient.Health.Service(serviceName, "", true);
            
            return services.Response.Select(service => new ServiceInstance
            {
                Id = service.Service.ID,
                ServiceName = service.Service.Service,
                Address = service.Service.Address,
                Port = service.Service.Port,
                Tags = service.Service.Tags?.ToList() ?? new List<string>(),
                IsHealthy = service.Checks.All(check => check.Status == HealthStatus.Passing)
            });
        }

        public async Task<ServiceInstance?> DiscoverServiceAsync(string serviceName)
        {
            var services = await DiscoverServicesAsync(serviceName);
            var healthyServices = services.Where(s => s.IsHealthy).ToList();
            
            if (!healthyServices.Any())
                return null;

            // Simple round-robin selection
            var random = new Random();
            return healthyServices[random.Next(healthyServices.Count)];
        }
    }

    public class ServiceRegistration
    {
        public string Id { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public List<string>? Tags { get; set; }
    }

    public class ServiceInstance
    {
        public string Id { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsHealthy { get; set; }
        
        public string BaseUrl => $"http://{Address}:{Port}";
    }
}

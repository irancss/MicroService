using BuildingBlocks.Models;
using Consul;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.ServiceDiscovery
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger<ConsulServiceDiscovery> _logger;

        public ConsulServiceDiscovery(IConsulClient consulClient, ILogger<ConsulServiceDiscovery> logger)
        {
            _consulClient = consulClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiceInstance>> GetServiceInstancesAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            try
            {
                var queryResult = await _consulClient.Health.Service(serviceName, tag: "", passingOnly: true, cancellationToken);
                if (queryResult.Response == null)
                {
                    return Enumerable.Empty<ServiceInstance>();
                }

                return queryResult.Response.Select(service => new ServiceInstance
                {
                    ServiceId = service.Service.ID,
                    ServiceName = service.Service.Service,
                    Host = string.IsNullOrEmpty(service.Service.Address) ? service.Node.Address : service.Service.Address,
                    Port = service.Service.Port,
                    Tags = service.Service.Tags ?? Array.Empty<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover instances for service {ServiceName} from Consul.", serviceName);
                return Enumerable.Empty<ServiceInstance>();
            }
        }

        public async Task<ServiceInstance?> GetServiceInstanceAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            var instances = await GetServiceInstancesAsync(serviceName, cancellationToken);
            // In a real scenario, a load balancer would be used here. This is just a simple fallback.
            return instances.FirstOrDefault();
        }

        public Task RegisterServiceAsync(ServiceRegistration registration, CancellationToken cancellationToken = default)
        {
            var consulRegistration = new AgentServiceRegistration
            {
                ID = registration.ServiceId,
                Name = registration.ServiceName,
                Address = registration.Address,
                Port = registration.Port,
                Tags = registration.Tags.ToArray(),
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{registration.Address}:{registration.Port}{registration.HealthCheckEndpoint}",
                    Interval = registration.HealthCheckInterval,
                    Timeout = registration.HealthCheckTimeout,
                    DeregisterCriticalServiceAfter = registration.DeregisterCriticalServiceAfter
                }
            };
            _logger.LogInformation("Registering service '{ServiceName}' with ID '{ServiceId}' and health check '{HealthCheckUrl}'",
                consulRegistration.Name, consulRegistration.ID, consulRegistration.Check.HTTP);

            return _consulClient.Agent.ServiceRegister(consulRegistration, cancellationToken);
        }

        public Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
        {
            return _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
        }
    }
}
using BuildingBlocks.Models;

namespace BuildingBlocks.ServiceDiscovery; 

/// <summary>
/// Interface for service discovery operations.
/// </summary>
public interface IServiceDiscovery
{
    Task<ServiceInstance?> GetServiceInstanceAsync(string serviceName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceInstance>> GetServiceInstancesAsync(string serviceName, CancellationToken cancellationToken = default);
    Task RegisterServiceAsync(ServiceRegistration registration, CancellationToken cancellationToken = default);
    Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default);
}
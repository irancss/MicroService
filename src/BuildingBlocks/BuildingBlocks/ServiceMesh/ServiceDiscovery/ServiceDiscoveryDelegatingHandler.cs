using BuildingBlocks.Models;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh.LoadBalancing;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BuildingBlocks.ServiceMesh
{
    /// <summary>
    /// [اصلاح شد] این DelegatingHandler اکنون از ILoadBalancer برای انتخاب یک نمونه از سرویس استفاده می‌کند.
    /// </summary>
    public class ServiceDiscoveryDelegatingHandler : DelegatingHandler
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILoadBalancer _loadBalancer;
        private readonly ILogger<ServiceDiscoveryDelegatingHandler> _logger;

        public ServiceDiscoveryDelegatingHandler(
            IServiceDiscovery serviceDiscovery,
            ILoadBalancer loadBalancer,
            ILogger<ServiceDiscoveryDelegatingHandler> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _loadBalancer = loadBalancer;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var host = request.RequestUri?.Host;
            if (string.IsNullOrEmpty(host) || request.RequestUri?.IsLoopback == true || host.Contains('.'))
            {
                // It's a regular FQDN or localhost, not a service name, so pass it through.
                return await base.SendAsync(request, cancellationToken);
            }

            _logger.LogDebug("Resolving service name '{ServiceName}' via service discovery.", host);

            var instances = (await _serviceDiscovery.GetServiceInstancesAsync(host, cancellationToken)).ToList();
            if (!instances.Any())
            {
                _logger.LogWarning("No healthy instances found for service '{ServiceName}'.", host);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { ReasonPhrase = $"Service '{host}' not available" };
            }

            ServiceInstance? selectedInstance = _loadBalancer.SelectInstance(instances, host);
            if (selectedInstance == null)
            {
                _logger.LogWarning("Load balancer could not select an instance for service '{ServiceName}'.", host);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { ReasonPhrase = $"No instance available for service '{host}' after load balancing" };
            }

            var originalUri = request.RequestUri;
            var newUri = new Uri($"{originalUri.Scheme}://{selectedInstance.Host}:{selectedInstance.Port}{originalUri.PathAndQuery}");
            request.RequestUri = newUri;

            _logger.LogInformation("Request to service '{ServiceName}' is being routed to instance {InstanceUrl}", host, newUri);

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                // Restore original URI for subsequent handlers in the pipeline
                request.RequestUri = originalUri;
            }
        }
    }
}
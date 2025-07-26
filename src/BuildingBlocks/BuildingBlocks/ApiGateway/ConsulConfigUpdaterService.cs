using BuildingBlocks.ServiceDiscovery;
using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;
using DestinationConfig = Yarp.ReverseProxy.Configuration.DestinationConfig;
using RouteConfig = Yarp.ReverseProxy.Configuration.RouteConfig;

namespace BuildingBlocks.ApiGateway
{
    // [نکته] این سرویس، هسته اصلی قابلیت Gateway پویا است و پیاده‌سازی آن با Blocking Queries بسیار بهینه و صحیح است.
    // این فایل بدون تغییر باقی می‌ماند.
    public class ConsulConfigUpdaterService : BackgroundService
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConsulClient _consulClient;
        private readonly InMemoryConfigProvider _configProvider;
        private readonly ILogger<ConsulConfigUpdaterService> _logger;
        private readonly YarpRoutesConfig _yarpRoutesConfig;
        private ulong _lastConsulIndex = 0;

        public ConsulConfigUpdaterService(
            IServiceDiscovery serviceDiscovery,
            IProxyConfigProvider configProvider,
            IConsulClient consulClient,
            IOptions<YarpRoutesConfig> yarpRoutesConfig,
            ILogger<ConsulConfigUpdaterService> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _consulClient = consulClient;
            _configProvider = (InMemoryConfigProvider)configProvider;
            _logger = logger;
            _yarpRoutesConfig = yarpRoutesConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await UpdateConfigAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogTrace("Waiting for service changes from Consul using index {ConsulIndex}.", _lastConsulIndex);

                    var queryOptions = new QueryOptions { WaitIndex = _lastConsulIndex };
                    var servicesResult = await _consulClient.Catalog.Services(queryOptions, stoppingToken);

                    if (servicesResult.LastIndex > _lastConsulIndex)
                    {
                        _logger.LogInformation("Detected changes in Consul services. New index: {NewIndex}. Refreshing YARP configuration.", servicesResult.LastIndex);

                        await UpdateConfigAsync(stoppingToken);

                        _lastConsulIndex = servicesResult.LastIndex;
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Consul config updater is stopping.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while watching for Consul configuration changes.");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }
        }

        private async Task UpdateConfigAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to refresh YARP configuration from Consul...");

            var routes = new List<RouteConfig>();
            var clusters = new List<ClusterConfig>();

            if (_yarpRoutesConfig.ServiceRoutes == null)
            {
                _logger.LogWarning("YARP service routes configuration is missing.");
                return;
            }

            foreach (var (serviceName, serviceRoute) in _yarpRoutesConfig.ServiceRoutes)
            {
                var instances = (await _serviceDiscovery.GetServiceInstancesAsync(serviceName, cancellationToken)).ToList();
                if (instances.Any())
                {
                    var clusterId = $"{serviceName}-cluster";

                    routes.Add(new RouteConfig
                    {
                        RouteId = $"{serviceName}-route",
                        ClusterId = clusterId,
                        Match = new RouteMatch { Path = serviceRoute.Path },
                        AuthorizationPolicy = serviceRoute.RequiresAuthentication ? "default" : null
                    });

                    clusters.Add(new ClusterConfig
                    {
                        ClusterId = clusterId,
                        LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                        Destinations = instances.ToDictionary(
                            instance => instance.ServiceId,
                            instance => new DestinationConfig { Address = instance.BaseUrl }
                        ),
                        HealthCheck = new HealthCheckConfig
                        {
                            Active = new ActiveHealthCheckConfig
                            {
                                Enabled = true,
                                Interval = TimeSpan.FromSeconds(10),
                                Timeout = TimeSpan.FromSeconds(10),
                                Policy = "ConsecutiveFailures",
                                Path = "/health/live"
                            }
                        }
                    });
                }
                else
                {
                    _logger.LogWarning("No healthy instances found for service: {ServiceName}", serviceName);
                }
            }

            _logger.LogInformation("Updating YARP config: Found {RouteCount} routes and {ClusterCount} clusters.", routes.Count, clusters.Count);
            _configProvider.Update(routes, clusters);
        }
    }

    public class YarpRoutesConfig
    {
        public Dictionary<string, ServiceRoute>? ServiceRoutes { get; set; }
    }

    public class ServiceRoute
    {
        public string Path { get; set; } = string.Empty;
        public bool RequiresAuthentication { get; set; } = false;
    }
}
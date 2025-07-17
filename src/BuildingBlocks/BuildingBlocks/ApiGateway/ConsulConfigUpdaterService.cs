using BuildingBlocks.ServiceDiscovery;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace BuildingBlocks.ApiGateway
{

public class ConsulConfigUpdaterService : BackgroundService
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly InMemoryConfigProvider _configProvider;
    private readonly ILogger<ConsulConfigUpdaterService> _logger;
    private readonly Dictionary<string, string> _serviceRoutes;

    public ConsulConfigUpdaterService(
        IServiceDiscovery serviceDiscovery,
        IProxyConfigProvider configProvider, // اینجا IProxyConfigProvider را می‌گیریم
        ILogger<ConsulConfigUpdaterService> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _configProvider = (InMemoryConfigProvider)configProvider; // و به نوع مشخص خودمان کست می‌کنیم
        _logger = logger;

        // این تنظیمات را می‌توانید از IConfiguration بخوانید تا هاردکد نباشد
        _serviceRoutes = new Dictionary<string, string>
        {
            ["product-service"] = "/api/products/{**catch-all}",
            ["order-service"] = "/api/orders/{**catch-all}",
            ["user-service"] = "/api/users/{**catch-all}",
            // ... سایر سرویس‌ها
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateConfigAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating YARP configuration from Consul.");
            }

            // زمان انتظار را می‌توانید در کانفیگ قرار دهید
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }

    private async Task UpdateConfigAsync()
    {
        _logger.LogInformation("Checking for service updates from Consul...");

        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var (serviceName, routePath) in _serviceRoutes)
        {
            var instances = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);
            if (instances.Any())
            {
                var clusterId = $"{serviceName}-cluster";
                
                var route = new RouteConfig
                {
                    RouteId = $"{serviceName}-route",
                    ClusterId = clusterId,
                    Match = new RouteMatch { Path = routePath },
                    // اضافه کردن پالیسی احراز هویت به صورت پویا
                    AuthorizationPolicy = RequiresAuthentication(serviceName) ? "Bearer" : null,
                    // پاک کردن پیشوند مسیر برای ارسال به سرویس مقصد
                    Transforms = new[]
                    {
                        new Dictionary<string, string> { ["PathRemovePrefix"] = $"/api/{serviceName.Split('-')[0]}" }
                        // مثال: /api/products/1 -> /1
                    }
                };
                routes.Add(route);

                var cluster = new ClusterConfig
                {
                    ClusterId = clusterId,
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = instances.ToDictionary(
                        instance => $"{instance.Id}", // استفاده از ID سرویس برای یکتایی
                        instance => new DestinationConfig { Address = instance.Address }
                    ),
                    HealthCheck = new HealthCheckConfig
                    {
                        Active = new ActiveHealthCheckConfig
                        {
                            Enabled = true,
                            Interval = TimeSpan.FromSeconds(10),
                            Timeout = TimeSpan.FromSeconds(10),
                            Policy = "ConsecutiveFailures",
                            Path = "/health"
                        }
                    }
                };
                clusters.Add(cluster);
            }
            else
            {
                _logger.LogWarning("No healthy instances found for service: {ServiceName}", serviceName);
            }
        }
        
        _logger.LogInformation("Found {RouteCount} routes and {ClusterCount} clusters.", routes.Count, clusters.Count);
        _configProvider.Update(routes, clusters);
    }

    private static bool RequiresAuthentication(string serviceName)
    {
        var protectedServices = new[] { "order-service", "user-service" };
        return protectedServices.Contains(serviceName, StringComparer.OrdinalIgnoreCase);
    }
    }
}
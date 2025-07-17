using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Health;
using BuildingBlocks.ServiceDiscovery;

namespace BuildingBlocks.ApiGateway
{
    /// <summary>
    /// YARP-based API Gateway with Consul Service Discovery
    /// </summary>
    public static class YarpApiGatewayExtensions
    {
        public static IServiceCollection AddYarpApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
             // --- شروع تغییرات برای YARP 2.0.1 ---
        var yarpBuilder = services.AddReverseProxy();
        
        // 1. ثبت یک IProxyConfigProvider سفارشی به صورت Singleton
        services.AddSingleton<InMemoryConfigProvider>();
        services.AddSingleton<IProxyConfigProvider>(sp => sp.GetRequiredService<InMemoryConfigProvider>());

        // 2. ثبت BackgroundService برای به‌روزرسانی کانفیگ
        services.AddHostedService<ConsulConfigUpdaterService>();
        // --- پایان تغییرات ---

        // ثبت سایر وابستگی‌ها
        services.AddSingleton<IServiceDiscovery, BuildingBlocks.ServiceDiscovery.ConsulServiceDiscovery>();
        
        services.AddScoped<IAggregationService, AggregationService>();
        
        AddJwtAuthentication(services, configuration);
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        return services;
        }

        private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>();
            if (jwtSettings != null && !string.IsNullOrEmpty(jwtSettings.SecretKey))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

                services.AddAuthorization();
            }
        }

        public static IApplicationBuilder UseYarpApiGateway(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            // Add custom middleware for aggregation endpoints
            app.UseMiddleware<AggregationMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();

                // Add aggregation endpoints
                endpoints.MapGet("/api/aggregated/dashboard/{userId}", async (string userId, IAggregationService aggregationService) =>
                {
                    var dashboardData = await aggregationService.GetDashboardDataAsync(userId);
                    return Results.Ok(dashboardData);
                });
            });

            return app;
        }
    }

    /// <summary>
    /// Consul-based proxy configuration provider for YARP
    /// </summary>
    public class ConsulProxyConfigProvider : IProxyConfigProvider
    {

        private volatile ConsulProxyConfig _config;

        public ConsulProxyConfigProvider()
        {
            // با یک کانفیگ خالی شروع می‌کنیم
            _config = new ConsulProxyConfig(new List<RouteConfig>(), new List<ClusterConfig>());
        }

        public IProxyConfig GetConfig() => _config;

        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var oldConfig = _config;
            _config = new ConsulProxyConfig(routes, clusters);
            // به کانفیگ قبلی سیگنال تغییر می‌دهیم تا YARP از کانفیگ جدید استفاده کند
            oldConfig.SignalChange();
        }

        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILogger<ConsulProxyConfigProvider> _logger;
        private readonly IConfiguration _configuration;
        private ConsulProxyConfig _config;

        // public ConsulProxyConfigProvider(
        //     IServiceDiscovery serviceDiscovery, 
        //     ILogger<ConsulProxyConfigProvider> logger,
        //     IConfiguration configuration)
        // {
        //     _serviceDiscovery = serviceDiscovery;
        //     _logger = logger;
        //     _configuration = configuration;
        //     _config = new ConsulProxyConfig();

        //     // Initialize configuration
        //     _ = Task.Run(LoadConfigurationAsync);
        // }

        // public IProxyConfig GetConfig() => _config;

        private async Task LoadConfigurationAsync()
        {
            try
            {
                var routes = new List<RouteConfig>();
                var clusters = new Dictionary<string, ClusterConfig>();

                // Define route patterns for services
                var serviceRoutes = new Dictionary<string, string>
                {
                    ["product-service"] = "/api/products/{**catch-all}",
                    ["order-service"] = "/api/orders/{**catch-all}",
                    ["user-service"] = "/api/users/{**catch-all}",
                    ["notification-service"] = "/api/notifications/{**catch-all}",
                    ["shipping-service"] = "/api/shipping/{**catch-all}",
                    ["payment-service"] = "/api/payments/{**catch-all}"
                };

                foreach (var serviceRoute in serviceRoutes)
                {
                    var serviceName = serviceRoute.Key;
                    var routePattern = serviceRoute.Value;

                    // Discover service instances
                    var instances = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);
                    if (instances.Any())
                    {
                        // Create route
                        var route = new RouteConfig
                        {
                            RouteId = $"{serviceName}-route",
                            ClusterId = $"{serviceName}-cluster",
                            Match = new RouteMatch
                            {
                                Path = routePattern
                            },
                            Transforms = new[]
                            {
                                new Dictionary<string, string> { ["PathPattern"] = "/{**catch-all}" }
                            }
                        };

                        // Add authentication requirement
                        if (RequiresAuthentication(serviceName))
                        {
                            route = route with
                            {
                                AuthorizationPolicy = "Bearer"
                            };
                        }

                        routes.Add(route);

                        // Create cluster
                        var destinations = instances.Select((instance, index) =>
                            new KeyValuePair<string, DestinationConfig>(
                                $"{serviceName}-destination-{index}",
                                new DestinationConfig { Address = instance.Address }
                            )).ToDictionary(x => x.Key, x => x.Value);

                        var cluster = new ClusterConfig
                        {
                            ClusterId = $"{serviceName}-cluster",
                            Destinations = destinations,
                            LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                            HealthCheck = new HealthCheckConfig
                            {
                                Active = new ActiveHealthCheckConfig
                                {
                                    Enabled = true,
                                    Interval = TimeSpan.FromSeconds(30),
                                    Timeout = TimeSpan.FromSeconds(5),
                                    Policy = "ConsecutiveFailures",
                                    Path = "/health"
                                }
                            }
                        };

                        clusters[cluster.ClusterId] = cluster;
                    }
                    else
                    {
                        _logger.LogWarning("No instances found for service: {ServiceName}", serviceName);
                    }
                }

                _config = new ConsulProxyConfig
                {
                    Routes = routes,
                    Clusters = clusters.Values.ToList()
                };

                _logger.LogInformation("Loaded {RouteCount} routes and {ClusterCount} clusters from Consul",
                    routes.Count, clusters.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading configuration from Consul");
            }
        }

        private static bool RequiresAuthentication(string serviceName)
        {
            // Services that require authentication
            var protectedServices = new[] { "order-service", "user-service", "payment-service" };
            return protectedServices.Contains(serviceName);
        }
    }

    /// <summary>
    /// Custom proxy configuration for Consul
    /// </summary>
    public class ConsulProxyConfig : IProxyConfig
    {
        // این CancellationTokenSource برای اطلاع‌رسانی به YARP استفاده می‌شود
        private CancellationTokenSource _cts = new();

        public IReadOnlyList<RouteConfig> Routes { get; private set; }
        public IReadOnlyList<ClusterConfig> Clusters { get; private set; }
        public IChangeToken ChangeToken { get; private set; }

        public ConsulProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        // این متد برای تریگر کردن رفرش کانفیگ استفاده می‌شود
        public void SignalChange()
        {
            _cts.Cancel();
        }
    }

    /// <summary>
    /// Aggregation service for combining multiple service responses
    /// </summary>
    public interface IAggregationService
    {
        Task<DashboardData> GetDashboardDataAsync(string userId);
        Task<OrderDetailsData> GetOrderDetailsAsync(int orderId);
    }

    /// <summary>
    /// Implementation of aggregation service
    /// </summary>
    public class AggregationService : IAggregationService
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AggregationService> _logger;

        public AggregationService(
            IServiceDiscovery serviceDiscovery,
            HttpClient httpClient,
            ILogger<AggregationService> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DashboardData> GetDashboardDataAsync(string userId)
        {
            try
            {
                // Discover services
                var userService = await _serviceDiscovery.GetServiceInstancesAsync("user-service");
                var orderService = await _serviceDiscovery.GetServiceInstancesAsync("order-service");
                var productService = await _serviceDiscovery.GetServiceInstancesAsync("product-service");

                // Execute requests in parallel
                var userTask = GetUserDataAsync(userService.FirstOrDefault(), userId);
                var ordersTask = GetUserOrdersAsync(orderService.FirstOrDefault(), userId);
                var recommendationsTask = GetRecommendationsAsync(productService.FirstOrDefault(), userId);

                await Task.WhenAll(userTask, ordersTask, recommendationsTask);

                return new DashboardData
                {
                    User = await userTask,
                    RecentOrders = await ordersTask,
                    Recommendations = await recommendationsTask
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating dashboard data for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<OrderDetailsData> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var orderService = await _serviceDiscovery.GetServiceInstancesAsync("order-service");
                var productService = await _serviceDiscovery.GetServiceInstancesAsync("product-service");
                var shippingService = await _serviceDiscovery.GetServiceInstancesAsync("shipping-service");

                var orderTask = GetOrderAsync(orderService.FirstOrDefault(), orderId);
                var shippingTask = GetShippingInfoAsync(shippingService.FirstOrDefault(), orderId);

                await Task.WhenAll(orderTask, shippingTask);

                var order = await orderTask;
                var shipping = await shippingTask;

                // Get product details for order items
                var productTasks = order.Items.Select(item =>
                    GetProductAsync(productService.FirstOrDefault(), item.ProductId)).ToArray();

                var products = await Task.WhenAll(productTasks);

                return new OrderDetailsData
                {
                    Order = order,
                    Shipping = shipping,
                    Products = products.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating order details for order: {OrderId}", orderId);
                throw;
            }
        }

        private async Task<UserData> GetUserDataAsync(ServiceInstance? service, string userId)
        {
            if (service == null) return new UserData();

            var response = await _httpClient.GetAsync($"{service.Address}/api/users/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<UserData>(json) ?? new UserData();
        }

        private async Task<List<OrderData>> GetUserOrdersAsync(ServiceInstance? service, string userId)
        {
            if (service == null) return new List<OrderData>();

            var response = await _httpClient.GetAsync($"{service.Address}/api/orders/user/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<OrderData>>(json) ?? new List<OrderData>();
        }

        private async Task<List<ProductData>> GetRecommendationsAsync(ServiceInstance? service, string userId)
        {
            if (service == null) return new List<ProductData>();

            var response = await _httpClient.GetAsync($"{service.Address}/api/products/recommendations/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<ProductData>>(json) ?? new List<ProductData>();
        }

        private async Task<OrderData> GetOrderAsync(ServiceInstance? service, int orderId)
        {
            if (service == null) return new OrderData();

            var response = await _httpClient.GetAsync($"{service.Address}/api/orders/{orderId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<OrderData>(json) ?? new OrderData();
        }

        private async Task<ShippingData> GetShippingInfoAsync(ServiceInstance? service, int orderId)
        {
            if (service == null) return new ShippingData();

            var response = await _httpClient.GetAsync($"{service.Address}/api/shipping/order/{orderId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ShippingData>(json) ?? new ShippingData();
        }

        private async Task<ProductData> GetProductAsync(ServiceInstance? service, int productId)
        {
            if (service == null) return new ProductData();

            var response = await _httpClient.GetAsync($"{service.Address}/api/products/{productId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ProductData>(json) ?? new ProductData();
        }
    }

    /// <summary>
    /// Middleware for handling aggregation endpoints
    /// </summary>
    public class AggregationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AggregationMiddleware> _logger;

        public AggregationMiddleware(RequestDelegate next, ILogger<AggregationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add correlation ID for tracking requests across services
            if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
            }

            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            _logger.LogInformation("Processing request {Method} {Path} with correlation ID: {CorrelationId}",
                context.Request.Method, context.Request.Path, correlationId);

            await _next(context);
        }
    }

    // Data models for aggregation
    public class DashboardData
    {
        public UserData User { get; set; } = new();
        public List<OrderData> RecentOrders { get; set; } = new();
        public List<ProductData> Recommendations { get; set; } = new();
    }

    public class OrderDetailsData
    {
        public OrderData Order { get; set; } = new();
        public ShippingData Shipping { get; set; } = new();
        public List<ProductData> Products { get; set; } = new();
    }

    public class UserData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class OrderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemData> Items { get; set; } = new();
    }

    public class OrderItemData
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ShippingData
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }


    public class ConsulConfigUpdaterService : BackgroundService
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ConsulProxyConfigProvider _configProvider;
    private readonly ILogger<ConsulConfigUpdaterService> _logger;
    private readonly Dictionary<string, string> _serviceRoutes;

    public ConsulConfigUpdaterService(
        IServiceDiscovery serviceDiscovery,
        IProxyConfigProvider configProvider,
        ILogger<ConsulConfigUpdaterService> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _configProvider = (ConsulProxyConfigProvider)configProvider; // Cast to our implementation
        _logger = logger;

        // این قسمت را می‌توان از IConfiguration نیز خواند
        _serviceRoutes = new Dictionary<string, string>
        {
            ["product-service"] = "/api/products/{**catch-all}",
            ["order-service"] = "/api/orders/{**catch-all}",
            ["user-service"] = "/api/users/{**catch-all}"
            // ... سایر سرویس‌ها
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await LoadConfigurationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating YARP configuration from Consul.");
            }

            // هر 30 ثانیه یک بار چک کن (این زمان را می‌توانید در کانفیگ قرار دهید)
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task LoadConfigurationAsync()
    {
        _logger.LogInformation("Attempting to update YARP configuration from Consul...");

        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var serviceRoute in _serviceRoutes)
        {
            var serviceName = serviceRoute.Key;
            var instances = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);

            if (instances.Any())
            {
                var clusterId = $"{serviceName}-cluster";

                // ایجاد Route
                routes.Add(new RouteConfig
                {
                    RouteId = $"{serviceName}-route",
                    ClusterId = clusterId,
                    Match = new RouteMatch { Path = serviceRoute.Value },
                    AuthorizationPolicy = RequiresAuthentication(serviceName) ? "Bearer" : null
                });

                // ایجاد Cluster
                clusters.Add(new ClusterConfig
                {
                    ClusterId = clusterId,
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = instances.ToDictionary(
                        instance => $"{serviceName}-{Guid.NewGuid()}", // Unique ID for each destination
                        instance => new DestinationConfig { Address = instance.Address }
                    ),
                    HealthCheck = new HealthCheckConfig
                    {
                        Active = new ActiveHealthCheckConfig
                        {
                            Enabled = true,
                            Interval = TimeSpan.FromSeconds(10),
                            Timeout = TimeSpan.FromSeconds(5),
                            Policy = "ConsecutiveFailures",
                            Path = "/health"
                        }
                    }
                });
            }
            else
            {
                _logger.LogWarning("No instances found for service: {ServiceName}", serviceName);
            }
        }
        
        // اگر تغییراتی وجود داشت، کانفیگ را آپدیت کن
        if (routes.Count > 0 || clusters.Count > 0)
        {
             _logger.LogInformation("Loaded {RouteCount} routes and {ClusterCount} clusters. Updating YARP...", routes.Count, clusters.Count);
            _configProvider.Update(routes, clusters);
        }
    }

    private static bool RequiresAuthentication(string serviceName)
    {
        var protectedServices = new[] { "order-service", "user-service" };
        return protectedServices.Contains(serviceName);
    }
    }
}

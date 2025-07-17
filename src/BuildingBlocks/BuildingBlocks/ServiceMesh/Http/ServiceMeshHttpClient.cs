using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.ServiceMesh.LoadBalancing;
using BuildingBlocks.ServiceMesh.CircuitBreaker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.ServiceMesh.Http
{
    public class ServiceMeshHttpClient : IServiceMeshHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILoadBalancer _loadBalancer;
        private readonly ICircuitBreakerFactory _circuitBreakerFactory;
        private readonly ILogger<ServiceMeshHttpClient> _logger;

        private readonly IPolicyRegistry<string> _policyRegistry;


          public ServiceMeshHttpClient(
            HttpClient httpClient,
            IServiceDiscovery serviceDiscovery,
            ILoadBalancer loadBalancer,
            ILogger<ServiceMeshHttpClient> logger)
        {
            _httpClient = httpClient;
            _serviceDiscovery = serviceDiscovery;
            _loadBalancer = loadBalancer;
            _logger = logger;

            // یک رجیستری برای نگهداری پالیسی‌ها ایجاد می‌کنیم
            _policyRegistry = new PolicyRegistry();
        }

         private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(string serviceName)
        {
            return _policyRegistry.GetOrAdd(serviceName, (key) =>
            {
                _logger.LogInformation("Creating new circuit breaker policy for service: {ServiceName}", key);
                return Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500) // خطاهای سمت سرور
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 5,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (result, timespan) => _logger.LogWarning("Circuit breaker for {Service} opened for {Duration}s. Reason: {Reason}", key, timespan.TotalSeconds, result.Exception?.Message ?? result.Result?.ReasonPhrase),
                        onReset: () => _logger.LogInformation("Circuit breaker for {Service} reset.", key)
                    );
            });
        }

         public async Task<HttpResponseMessage> GetAsync(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            var policy = GetCircuitBreakerPolicy(serviceName);
            
            // اجرای درخواست از طریق پالیسی
            return await policy.ExecuteAsync(async () =>
            {
                var serviceInstance = await GetServiceInstanceAsync(serviceName);
                if (serviceInstance == null)
                    throw new InvalidOperationException($"Service {serviceName} not found");

                var url = $"{serviceInstance.BaseUrl}{path}";
                _logger.LogDebug("GET request to {Url}", url);
                return await _httpClient.GetAsync(url, cancellationToken);
            });
        }

        public async Task<T?> GetAsync<T>(string serviceName, string endpoint, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                return default;

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);

            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{endpoint}";
                _logger.LogDebug("GET request to {Url}", url);

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);
            });
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                return default;

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{endpoint}";
                var json = JsonSerializer.Serialize(request, JsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogDebug("POST request to {Url}", url);
                
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonSerializerOptions);
            });
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string serviceName, string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                return default;

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{endpoint}";
                var json = JsonSerializer.Serialize(request, JsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogDebug("PUT request to {Url}", url);
                
                var response = await _httpClient.PutAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<TResponse>(responseContent, JsonSerializerOptions);
            });
        }

        public async Task<HttpResponseMessage> SendAsync(string serviceName, HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                throw new InvalidOperationException($"No healthy instances found for service: {serviceName}");

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                // Update the request URI with the service instance
                var builder = new UriBuilder(request.RequestUri!)
                {
                    Scheme = "http",
                    Host = serviceInstance.Address,
                    Port = serviceInstance.Port
                };
                request.RequestUri = builder.Uri;
                
                _logger.LogDebug("{Method} request to {Url}", request.Method, request.RequestUri);
                
                return await _httpClient.SendAsync(request, cancellationToken);
            });
        }

        // Interface implementation methods
        public async Task<HttpResponseMessage> GetAsync(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                throw new InvalidOperationException($"Service {serviceName} not found");

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{path}";
                _logger.LogDebug("GET request to {Url}", url);
                
                var response = await _httpClient.GetAsync(url, cancellationToken);
                return response;
            });
        }

        public async Task<HttpResponseMessage> PostAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                throw new InvalidOperationException($"Service {serviceName} not found");

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{path}";
                _logger.LogDebug("POST request to {Url}", url);
                
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                return response;
            });
        }

        public async Task<HttpResponseMessage> PutAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                throw new InvalidOperationException($"Service {serviceName} not found");

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{path}";
                _logger.LogDebug("PUT request to {Url}", url);
                
                var response = await _httpClient.PutAsync(url, content, cancellationToken);
                return response;
            });
        }

        public async Task<HttpResponseMessage> DeleteAsync(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            var serviceInstance = await GetServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
                throw new InvalidOperationException($"Service {serviceName} not found");

            var circuitBreaker = _circuitBreakerFactory.CreateCircuitBreaker(serviceName);
            
            return await circuitBreaker.ExecuteAsync(async () =>
            {
                var url = $"http://{serviceInstance.Address}:{serviceInstance.Port}{path}";
                _logger.LogDebug("DELETE request to {Url}", url);
                
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                return response;
            });
        }

        public async Task<T?> GetFromJsonAsync<T>(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(serviceName, path, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value, JsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            return await PostAsync(serviceName, path, content, cancellationToken);
        }

        private async Task<ServiceInstance?> GetServiceInstanceAsync(string serviceName)
        {
            try
            {
                var services = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);
                var healthyServices = services.Where(s => s.IsHealthy).ToList();
                
                if (!healthyServices.Any())
                {
                    _logger.LogWarning("No healthy instances found for service: {ServiceName}", serviceName);
                    return null;
                }

                return _loadBalancer.SelectService(healthyServices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error discovering service instances for: {ServiceName}", serviceName);
                return null;
            }
        }

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}

using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BuildingBlocks.ServiceMesh.Http
{
    /// <summary>
    /// Implementation of a resilient HttpClient that uses a delegating handler for service discovery.
    /// Policies like Retry and Circuit Breaker are applied via IHttpClientFactory configuration.
    /// </summary>
    public class ServiceMeshHttpClient : IServiceMeshHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceMeshHttpClient> _logger;

        public ServiceMeshHttpClient(HttpClient httpClient, ILogger<ServiceMeshHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // The ServiceDiscoveryDelegatingHandler will automatically resolve the service name in the URL.
        // We just need to use the format: http://{service-name}/api/path
        public Task<HttpResponseMessage> GetAsync(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetAsync(BuildUri(serviceName, path), cancellationToken);
        }

        public Task<T?> GetFromJsonAsync<T>(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetFromJsonAsync<T>(BuildUri(serviceName, path), cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default)
        {
            return _httpClient.PostAsync(BuildUri(serviceName, path), content, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default)
        {
            return _httpClient.PostAsJsonAsync(BuildUri(serviceName, path), value, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default)
        {
            return _httpClient.PutAsync(BuildUri(serviceName, path), content, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default)
        {
            return _httpClient.PutAsJsonAsync(BuildUri(serviceName, path), value, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(string serviceName, string path, CancellationToken cancellationToken = default)
        {
            return _httpClient.DeleteAsync(BuildUri(serviceName, path), cancellationToken);
        }

        private static string BuildUri(string serviceName, string path)
        {
            // The scheme (http) is important for the delegating handler to parse the host correctly.
            return $"http://{serviceName}{path}";
        }
    }
}
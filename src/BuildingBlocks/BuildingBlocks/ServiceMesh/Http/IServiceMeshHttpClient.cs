using System.Net.Http.Json;

namespace BuildingBlocks.ServiceMesh.Http
{
    /// <summary>
    /// HTTP client with integrated service discovery, load balancing, and resiliency.
    /// This is the recommended client for all inter-service communication.
    /// </summary>
    public interface IServiceMeshHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string serviceName, string path, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAsync(string serviceName, string path, CancellationToken cancellationToken = default);

        Task<T?> GetFromJsonAsync<T>(string serviceName, string path, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default);
    }
}
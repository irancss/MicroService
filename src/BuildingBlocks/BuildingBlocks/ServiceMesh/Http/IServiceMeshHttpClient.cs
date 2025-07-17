namespace BuildingBlocks.ServiceMesh.Http
{
    /// <summary>
    /// HTTP client with service mesh capabilities including service discovery and load balancing
    /// </summary>
    public interface IServiceMeshHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string serviceName, string path, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsync(string serviceName, string path, HttpContent? content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAsync(string serviceName, string path, CancellationToken cancellationToken = default);
        Task<T?> GetFromJsonAsync<T>(string serviceName, string path, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string serviceName, string path, T value, CancellationToken cancellationToken = default);
    }
}

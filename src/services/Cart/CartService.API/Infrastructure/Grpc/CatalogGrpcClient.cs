

using BuildingBlocks.ServiceMesh.Http;
using Cart.Application.Interfaces;

namespace Cart.API.Infrastructure.Grpc;



// DTO for the response from Catalog service's API
file record ProductInfoResponse(string Id, string Name, string? ImageUrl, bool IsActive);

public class CatalogGrpcClient : ICatalogGrpcClient
{
    private readonly IServiceMeshHttpClient _serviceMeshClient;
    private const string ServiceName = "catalog-service";

    public CatalogGrpcClient(IServiceMeshHttpClient serviceMeshClient)
    {
        _serviceMeshClient = serviceMeshClient;
    }

    public async Task<ProductInfo?> GetProductInfoAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _serviceMeshClient.GetFromJsonAsync<ProductInfoResponse>(ServiceName, $"/api/v1/products/{productId}", cancellationToken);
            return response != null ? new ProductInfo(response.Id, response.Name, response.ImageUrl, response.IsActive) : null;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
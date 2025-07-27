using BuildingBlocks.ServiceMesh.Http;
using Cart.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cart.Infrastructure.Services;

// این DTO برای نگهداری پاسخ دریافتی از API سرویس Catalog است.
// نام propertyها باید دقیقاً با نام propertyهای JSON خروجی API هماهنگ باشد.
file record ProductInfoResponse(string Id, string Name, string? ImageUrl, bool IsActive);

public class CatalogGrpcClient : ICatalogGrpcClient
{
    private readonly IServiceMeshHttpClient _serviceMeshClient;
    private readonly ILogger<CatalogGrpcClient> _logger;

    // نام سرویس همانطور که در Consul ثبت شده است.
    private const string ServiceName = "catalog-service";

    public CatalogGrpcClient(IServiceMeshHttpClient serviceMeshClient, ILogger<CatalogGrpcClient> logger)
    {
        _serviceMeshClient = serviceMeshClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves product information from the Catalog microservice.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ProductInfo object if found; otherwise, null.</returns>
    public async Task<ProductInfo?> GetProductInfoAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            return null;
        }

        try
        {
            // از IServiceMeshHttpClient برای فراخوانی امن و تاب‌آور سرویس Catalog استفاده می‌کنیم.
            // این کلاینت به صورت خودکار آدرس سرویس را از Consul پیدا می‌کند.
            // فرض بر این است که سرویس Catalog یک endpoint HTTP GET در این آدرس دارد.
            var response = await _serviceMeshClient.GetFromJsonAsync<ProductInfoResponse>(
                ServiceName,
                $"/api/v1/products/{productId}",
                cancellationToken);

            if (response is null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found in Catalog service.", productId);
                return null;
            }

            // پاسخ دریافتی را به رکورد ProductInfo که در لایه Application تعریف شده، تبدیل می‌کنیم.
            return new ProductInfo(response.Id, response.Name, response.ImageUrl, response.IsActive);
        }
        catch (HttpRequestException ex)
        {
            // اگر سرویس پاسخ 404 بدهد، یعنی محصول یافت نشده است.
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product with ID {ProductId} resulted in a 404 Not Found from Catalog service.", productId);
                return null;
            }

            // برای خطاهای دیگر (مانند 500)، خطا را لاگ کرده و دوباره پرتاب می‌کنیم تا Polly (Circuit Breaker) آن را مدیریت کند.
            _logger.LogError(ex, "HTTP error occurred while calling Catalog service for Product ID {ProductId}.", productId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while getting product info for {ProductId}.", productId);
            throw; // اجازه می‌دهیم خطاهای غیرمنتظره به لایه‌های بالاتر بروند.
        }
    }
}

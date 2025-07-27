using BuildingBlocks.Messaging;
using BuildingBlocks.ServiceMesh.Http;
using Cart.Application.Interfaces;

namespace Cart.Infrastructure.Services;

// رکوردهایی که برای ارتباط با سرویس Inventory استفاده می‌شوند
// بهتر است اینها در یک پروژه Contracts مشترک باشند، اما فعلاً اینجا خوب است
public record PriceResponse(decimal Price);
public record StockResponse(bool IsInStock);
public record ReserveStockCommand(string CartId, Dictionary<string, int> Items);
public record ReleaseStockCommand(string CartId, string Reason);

public class InventoryHttpClient : IInventoryClient
{
    private readonly IServiceMeshHttpClient _serviceMeshClient;
    private readonly IMessageBus _messageBus;
    private const string ServiceName = "inventory-service";

    public InventoryHttpClient(IServiceMeshHttpClient serviceMeshClient, IMessageBus messageBus)
    {
        _serviceMeshClient = serviceMeshClient;
        _messageBus = messageBus;
    }

    public async Task<bool> CheckStockAvailabilityAsync(string productId, int quantity, CancellationToken cancellationToken = default)
    {
        // فقط تماس HTTP
        var response = await _serviceMeshClient.GetFromJsonAsync<StockResponse>(
            ServiceName,
            $"/api/v1/stock/{productId}/check?quantity={quantity}",
            cancellationToken);
        return response?.IsInStock ?? false;
    }

    public async Task<decimal?> GetCurrentPriceAsync(string productId, CancellationToken cancellationToken = default)
    {
        // فقط تماس HTTP
        var response = await _serviceMeshClient.GetFromJsonAsync<PriceResponse>(
            ServiceName,
            $"/api/v1/products/{productId}/price",
            cancellationToken);
        return response?.Price;
    }

    // عملیات‌های نوشتن (Write) از طریق پیام‌رسانی برای پایداری بیشتر انجام می‌شوند
    public Task ReserveStockAsync(string cartId, Dictionary<string, int> items, CancellationToken cancellationToken = default)
    {
        var command = new ReserveStockCommand(cartId, items);
        // ارسال Command به صف مشخص سرویس Inventory
        return _messageBus.SendAsync(command, cancellationToken);
    }

    public Task ReleaseStockAsync(string cartId, string reason, CancellationToken cancellationToken = default)
    {
        var command = new ReleaseStockCommand(cartId, reason);
        return _messageBus.SendAsync(command, cancellationToken);
    }
}
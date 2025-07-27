using BuildingBlocks.Messaging;
using BuildingBlocks.ServiceMesh.Http;
using Cart.Application.Interfaces;

namespace Cart.API.Infrastructure.Grpc
{
    // DTOs & Commands for Inventory Service
    file record PriceResponse(decimal Price);
    file record StockResponse(bool IsInStock);
    public record ReserveStockCommand(string CartId, Dictionary<string, int> Items);
    public record ReleaseStockCommand(string CartId, string Reason);

    public class InventoryGrpcClient : IInventoryGrpcClient
    {
        private readonly IServiceMeshHttpClient _serviceMeshClient;
        private readonly IMessageBus _messageBus;
        private const string ServiceName = "inventory-service";

        public InventoryGrpcClient(IServiceMeshHttpClient serviceMeshClient, IMessageBus messageBus)
        {
            _serviceMeshClient = serviceMeshClient;
            _messageBus = messageBus;
        }

        public async Task<bool> CheckStockAvailabilityAsync(string productId, int quantity, CancellationToken cancellationToken = default)
        {
            var response = await _serviceMeshClient.GetFromJsonAsync<StockResponse>(ServiceName, $"/api/v1/stock/{productId}/check?quantity={quantity}", cancellationToken);
            return response?.IsInStock ?? false;
        }

        public async Task<decimal?> GetCurrentPriceAsync(string productId, CancellationToken cancellationToken = default)
        {
            var response = await _serviceMeshClient.GetFromJsonAsync<PriceResponse>(ServiceName, $"/api/v1/products/{productId}/price", cancellationToken);
            return response?.Price;
        }

        public Task ReserveStockAsync(string cartId, Dictionary<string, int> items, CancellationToken cancellationToken = default)
        {
            var command = new ReserveStockCommand(cartId, items);
            return _messageBus.SendAsync(command, cancellationToken);
        }

        public Task ReleaseStockAsync(string cartId, string reason, CancellationToken cancellationToken = default)
        {
            var command = new ReleaseStockCommand(cartId, reason);
            return _messageBus.SendAsync(command, cancellationToken);
        }
    }
}

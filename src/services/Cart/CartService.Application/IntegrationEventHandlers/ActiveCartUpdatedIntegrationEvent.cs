using BuildingBlocks.Messaging.Events.Base;

namespace Cart.Application.IntegrationEventHandlers
{
    // Published whenever the active cart is modified (add, remove, clear).
    // Inventory service listens to this to manage stock reservations.
    public record ActiveCartUpdatedIntegrationEvent(
        string UserId,
        string CartId,
        int TotalItems,
        decimal TotalPrice,
        List<CartItemDetails> Items
    ) : IntegrationEvent;

    public record CartItemDetails(string ProductId, int Quantity, decimal Price);
}

// این هندلر به رویداد دامنه گوش می‌دهد
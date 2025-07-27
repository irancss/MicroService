using BuildingBlocks.Messaging.Events.Base;

namespace Cart.Application.IntegrationEventHandlers
{
    public record CartUpdatedIntegrationEvent(
        string UserId,
        int TotalItems,
        decimal TotalPrice,
        List<CartItemDetails> Items
    ) : IntegrationEvent;

}

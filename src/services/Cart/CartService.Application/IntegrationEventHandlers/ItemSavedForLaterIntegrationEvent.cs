using BuildingBlocks.Messaging.Events.Base;

namespace Cart.Application.IntegrationEventHandlers
{
    public record ItemSavedForLaterIntegrationEvent(
        string UserId,
        string ProductId,
        decimal Price
    ) : IntegrationEvent;
}

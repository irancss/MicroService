using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Contracts
{
    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public Guid ProductId { get; init; }
        public decimal NewPrice { get; init; }
        public decimal OldPrice { get; init; }
    }

    public record ProductStockChangedIntegrationEvent(Guid ProductId, int NewStockQuantity) : IntegrationEvent;
}

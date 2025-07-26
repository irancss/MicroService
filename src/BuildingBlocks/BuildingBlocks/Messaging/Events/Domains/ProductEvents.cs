using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Domains
{
    public record ProductCreatedEvent : IntegrationEvent
    {
        public int ProductId { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int InitialStock { get; init; }
    }

    public record ProductUpdatedEvent : IntegrationEvent
    {
        public int ProductId { get; init; }
        public Dictionary<string, object> Changes { get; init; } = new();
    }

    public record StockUpdatedEvent : IntegrationEvent
    {
        public int ProductId { get; init; }
        public int QuantityChanged { get; init; }
        public int NewStockLevel { get; init; }
        public string Reason { get; init; } = string.Empty;
    }

    public record ProductDiscontinuedEvent(int ProductId) : IntegrationEvent;
}
using BuildingBlocks.Messaging.Events.Base;


namespace ProductService.Domain.Events;

public record VendorActivatedEvent(string VendorId) : IntegrationEvent;
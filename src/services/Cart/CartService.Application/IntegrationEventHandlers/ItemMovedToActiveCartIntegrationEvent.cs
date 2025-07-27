using BuildingBlocks.Messaging.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.IntegrationEventHandlers
{
    // Published for analytics when an item is moved from next-purchase to active cart.
    public record ItemMovedToActiveCartIntegrationEvent(
        string UserId,
        string ProductId,
        decimal Price
    ) : IntegrationEvent;
}

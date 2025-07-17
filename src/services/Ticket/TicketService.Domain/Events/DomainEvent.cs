// filepath: c:\Users\SHIELD SYSTEM\source\repos\MicroService\src\services\ProductApi\TicketService.Core\Events\DomainEvent.cs
using System;

namespace TicketService.Domain.Events // Or TicketService.Domain.Events
{
    public abstract class DomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        // Add other common properties for domain events if needed
    }
}
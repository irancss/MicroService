using MediatR;
using TicketService.Domain.Common;

namespace TicketService.Infrastructure.Common
{
    public static  class MediatorExtensions
    {
        public static async Task DispatchDomainEvents(this IMediator mediator, TicketDbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<IEntityWithDomainEvent>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity);


            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();
            entities.ToList().ForEach(e => e.ClearDomainEvents());


            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}

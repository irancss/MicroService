using MassTransit.Mediator;
using ProductService.Domain.Common;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Common
{
    public static  class MediatorExtensions
    {
        public static async Task DispatchDomainEvents(this IMediator mediator, ProductDbContext context)
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

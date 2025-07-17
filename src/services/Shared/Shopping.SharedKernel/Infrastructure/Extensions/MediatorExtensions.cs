


using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopping.SharedKernel.Domain.Events;

namespace Shopping.SharedKernel.Infrastructure.Extensions;


public static class MediatorExtensions
{
    // Notice the change from ProductDbContext to the generic DbContext
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<IEntityWithDomainEvent>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // It's important to clear events BEFORE dispatching them
        // to avoid issues with re-entrant calls.
        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
using BuildingBlocks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Extensions;

public static class MediatorExtensions
{
    /// <summary>
    /// Dispatches all domain events from entities tracked by the DbContext.
    /// This should be called before SaveChangesAsync.
    /// </summary>
    public static async Task DispatchDomainEvents(this IMediator mediator, DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<IEntityWithDomainEvent>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList(); // To avoid issues with collection modification during iteration

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // It's important to clear events BEFORE dispatching them
        // to avoid issues with re-entrant calls if a handler causes more events.
        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
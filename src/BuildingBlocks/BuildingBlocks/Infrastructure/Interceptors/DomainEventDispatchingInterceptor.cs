using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BuildingBlocks.Domain.Entities;
using MediatR;

namespace BuildingBlocks.Infrastructure.Interceptors
{
    public class DomainEventDispatchingInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public DomainEventDispatchingInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DispatchEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await DispatchEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchEvents(DbContext? context)
        {
            if (context == null) return;

            var entities = context.ChangeTracker
                .Entries<IEntityWithDomainEvent>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entities.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}

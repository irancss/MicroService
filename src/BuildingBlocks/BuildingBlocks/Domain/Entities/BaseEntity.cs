using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// Abstract base class for all entities.
/// [اصلاح شد] ارجاع به IDomainEvent از یک مکان واحد.
/// </summary>
public abstract class BaseEntity<TId> : IEntityWithDomainEvent where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// A convenient base class for entities with a Guid ID.
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}
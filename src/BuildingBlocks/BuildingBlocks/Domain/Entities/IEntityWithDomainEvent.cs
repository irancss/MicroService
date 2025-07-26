using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Entities; // [اصلاح شد] Namespace برای هماهنگی با BaseEntity

/// <summary>
/// [اصلاح شد] اینترفیس در یک مکان واحد (همراه با BaseEntity) تعریف شده است.
/// Marks an entity as having domain events.
/// </summary>
public interface IEntityWithDomainEvent
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    void RemoveDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}
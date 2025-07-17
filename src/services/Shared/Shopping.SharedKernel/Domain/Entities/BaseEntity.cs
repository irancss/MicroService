
using System.ComponentModel.DataAnnotations.Schema;
using Shopping.SharedKernel.Domain.Events;
using Shopping.SharedKernel.Domain.Interfaces;

namespace Shopping.SharedKernel.Domain.Entities;
public abstract class BaseEntity : IBaseId<string>, IEntityWithDomainEvent
{
    // public string Id { get; set; }
    //private readonly List<BaseEvent> _domainEvents = new();
    //[NotMapped]
    //public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
    public string Id { get; set; } = Guid.NewGuid().ToString();


    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

}
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketService.Domain.Common
{
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

    public interface IBaseId<T> where T : class
    {
        public T Id { get; set; }
    }

    public interface IEntityWithDomainEvent
    {
        IReadOnlyCollection<BaseEvent> DomainEvents { get; }
        void AddDomainEvent(BaseEvent domainEvent);
        void RemoveDomainEvent(BaseEvent domainEvent);
        void ClearDomainEvents();
    }
}

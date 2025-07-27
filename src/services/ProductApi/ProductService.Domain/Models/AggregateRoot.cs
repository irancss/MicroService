using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models
{
    /// <summary>
    /// A base class for aggregate roots in this microservice, inheriting auditing and domain event capabilities.
    /// </summary>
    public abstract class AggregateRoot<TId> : AuditableEntity<TId> where TId : notnull
    {
        // این کلاس می‌تواند در آینده منطق مشترک بین Aggregate های این سرویس را نگه دارد.
        protected AggregateRoot() { }
    }

    /// <summary>
    /// A convenient base class for aggregate roots with a Guid ID.
    /// </summary>
    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
        protected AggregateRoot() { }
    }
}

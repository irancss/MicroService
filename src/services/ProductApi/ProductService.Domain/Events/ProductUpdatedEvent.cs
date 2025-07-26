using ProductService.Domain.Models;
using BuildingBlocks.Domain.Events;

namespace ProductService.Domain.Events;

public class ProductUpdatedEvent : IDomainEvent
{
    public Product Product { get; }
    public ProductUpdatedEvent(Product product)
    {
        Product = product;
    }

    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}
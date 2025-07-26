using ProductService.Domain.Models;
using BuildingBlocks.Domain.Events;

namespace ProductService.Domain.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public Product Product { get; }

    public ProductCreatedEvent(Product product)
    {
        Product = product;
    }

    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}
using ProductService.Domain.Models;

namespace ProductService.Domain.Events;

public class ProductCreatedEvent : DomainEvent
{
    public Product Product { get; }

    public ProductCreatedEvent(Product product)
    {
        Product = product;
    }
}
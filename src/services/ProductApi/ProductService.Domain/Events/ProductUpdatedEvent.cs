using ProductService.Domain.Models;

namespace ProductService.Domain.Events;

public class ProductUpdatedEvent : DomainEvent
{
    public Product Product { get; }
    public ProductUpdatedEvent(Product product)
    {
        Product = product;
    }
}
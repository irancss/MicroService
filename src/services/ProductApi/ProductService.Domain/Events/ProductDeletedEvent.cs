namespace ProductService.Domain.Events;

public class ProductDeletedEvent : DomainEvent
{
    public int ProductId { get; }
    public ProductDeletedEvent(int productId)
    {
        ProductId = productId;
    }
}
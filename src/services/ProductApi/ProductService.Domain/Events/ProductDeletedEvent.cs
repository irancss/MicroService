using BuildingBlocks.Domain.Events;


namespace ProductService.Domain.Events;

public class ProductDeletedEvent : IDomainEvent
{
    public int ProductId { get; }
    public ProductDeletedEvent(int productId)
    {
        ProductId = productId;
    }

    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}
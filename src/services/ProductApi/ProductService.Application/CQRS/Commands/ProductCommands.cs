using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Commands
{
    public record CreateProductCommand(
        string Name,
        string? Description,
        decimal Price,
        string Sku,
        int Stock,
        Guid? BrandId,
        List<Guid> CategoryIds
    ) : CommandBase<Guid>; // شناسه محصول جدید را برمی‌گرداند

    public record UpdateProductCommand(
        Guid ProductId,
        string Name,
        string? Description,
        decimal Price,
        Guid? BrandId
    ) : CommandBase;

    public record UpdateProductStockCommand(
        Guid ProductId,
        int NewQuantity
    ) : CommandBase;

    public record DeleteProductCommand(
        Guid ProductId
    ) : CommandBase;
}

using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Brand
{
    public record CreateBrandCommand(
        string Name,
        string? Description
    ) : CommandBase<Guid>; // شناسه برند جدید را برمی‌گرداند

    public record UpdateBrandCommand(
        Guid BrandId,
        string Name,
        string? Description
    ) : CommandBase;

    public record DeleteBrandCommand(
        Guid BrandId
    ) : CommandBase;
}

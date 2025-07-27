using BuildingBlocks.Application.Abstractions;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.CQRS.Brand
{
    public record GetBrandByIdQuery(Guid BrandId) : IQuery<BrandDto?>;

    public record GetAllBrandsQuery() : IQuery<IEnumerable<BrandDto>>;
}

using ProductService.Application.DTOs.Product;
using BuildingBlocks.Application.Abstractions;
using ProductService.Application.DTOs;

namespace ProductService.Application.CQRS.Queries
{
    public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto?>;

    // برای فیلتر و صفحه‌بندی پیشرفته از کتابخانه‌هایی مثل Sieve استفاده می‌کنیم که در BuildingBlocks شما بود.
    // فعلا برای سادگی یک کوئری ساده تعریف می‌کنیم.
    public record GetProductsQuery(int PageNumber, int PageSize) : IQuery<PaginatedList<ProductDto>>;
}

using BuildingBlocks.Application.Mappings;
using ProductService.Domain.Models;

namespace ProductService.Application.DTOs.Product;

public class ProductDto : IMapFrom<Domain.Models.Product>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; } // برای نمایش در UI
    public List<CategoryDto> Categories { get; set; } = new();
}

public class CategoryDto : IMapFrom<Category>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BrandDto : IMapFrom<Domain.Models.Brand>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// این کلاس برای پاسخ‌های صفحه‌بندی شده استفاده می‌شود و باید در BuildingBlocks باشد.
// فرض می‌کنم در BuildingBlocks شما چیزی شبیه این وجود دارد:
// public record PaginatedList<T>(IReadOnlyCollection<T> Items, int TotalCount, int PageNumber, int PageSize);
// اگر نیست، می‌توانید از PaginatedList قبلی خودتان استفاده کنید.
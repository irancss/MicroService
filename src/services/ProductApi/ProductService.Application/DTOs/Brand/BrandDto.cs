using ProductService.Application.Mappings;

namespace ProductService.Application.DTOs.Brand
{


    public class BrandDto : IMapFrom<Domain.Models.Brand>
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; } // URL-friendly identifier
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }

    public class BrandListDto : BrandDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}

using ProductService.Domain.Common;

namespace ProductService.Domain.Models
{
    public class Brand : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; } // URL-friendly identifier
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        public virtual ICollection<ProductBrand> ProductBrands { get; set; } = new List<ProductBrand>();
    }
}

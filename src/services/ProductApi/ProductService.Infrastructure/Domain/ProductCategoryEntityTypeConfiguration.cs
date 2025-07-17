using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class ProductCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories) // Assuming Product has ICollection<ProductCategory> ProductCategories
            .HasForeignKey(pc => pc.ProductId);

        builder.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories) // Assuming Category has ICollection<ProductCategory> ProductCategories
            .HasForeignKey(pc => pc.CategoryId);
            
    }
}
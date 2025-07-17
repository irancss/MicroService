using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class ProductBrandEntityTypeConfiguration : IEntityTypeConfiguration<ProductBrand>
{
    public void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        builder.ToTable("ProductBrands"); // Join table for Many-to-Many between Product and Brand
        builder.HasKey(pb => new { pb.ProductId, pb.BrandId });

        builder.HasOne(pb => pb.Product)
            .WithMany(p => p.ProductBrands) // Assuming Product has ICollection<ProductBrand> ProductBrands
            .HasForeignKey(pb => pb.ProductId);

        builder.HasOne(pb => pb.Brand)
            .WithMany(b => b.ProductBrands) // Assuming Brand has ICollection<ProductBrand> ProductBrands
            .HasForeignKey(pb => pb.BrandId);
    }
}
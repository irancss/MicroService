using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class ProductTagEntityTypeConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.ToTable("ProductTags");
        builder.HasKey(pt => new { pt.ProductId, pt.TagId });

        builder.HasOne(pt => pt.Product)
            .WithMany(p => p.ProductTags) // Assuming Product has ICollection<ProductTag> ProductTags
            .HasForeignKey(pt => pt.ProductId);

        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.ProductTags) // Assuming Tag has ICollection<ProductTag> ProductTags
            .HasForeignKey(pt => pt.TagId);
    }
}
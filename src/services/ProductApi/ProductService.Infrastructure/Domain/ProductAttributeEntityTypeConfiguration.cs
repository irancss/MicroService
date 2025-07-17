using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.Domain;

public class ProductAttributeEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductAttribute>
{
    public override void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        base.Configure(builder);

        builder.Property(pa => pa.Description).HasMaxLength(255);
        // Add any other ProductAttribute-specific configuration here
    }
}

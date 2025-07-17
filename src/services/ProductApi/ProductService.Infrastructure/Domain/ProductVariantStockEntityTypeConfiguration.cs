using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Domain; // For BaseEntityTypeConfiguration

namespace ProductService.Infrastructure.Domain;

public class ProductVariantStockEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductVariantStock>
{
    public override void Configure(EntityTypeBuilder<ProductVariantStock> builder)
    {
        base.Configure(builder);

        builder.Property(pvs => pvs.Quantity).IsRequired();
        builder.Property(pvs => pvs.WarehouseLocation).HasMaxLength(100);
        builder.Property(pvs => pvs.LastStockUpdatedAt).IsRequired();

        // Configure relationships if needed
        // builder.Property(pvs => pvs.ProductVariantId).IsRequired();
    }
}

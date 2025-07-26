using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Domain;

namespace ProductService.Infrastructure.Domain;

public class ProductVariantEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductVariant>
{
    public override void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        base.Configure(builder);

        builder.Property(pv => pv.Sku).IsRequired().HasMaxLength(100);
        builder.HasIndex(pv => pv.Sku).IsUnique();
        builder.Property(pv => pv.Name).HasMaxLength(255);
        builder.Property(pv => pv.PriceModifier).HasColumnType("decimal(18,2)");

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .IsRequired();

        builder.HasMany(pv => pv.Prices)
            .WithOne(pvp => pvp.ProductVariant)
            .HasForeignKey(pvp => pvp.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Uncomment if you want to configure Stock as one-to-one
        // builder.HasOne(pv => pv.Stock)
        //     .WithOne(pvs => pvs.ProductVariant)
        //     .HasForeignKey<ProductVariantStock>(pvs => pvs.ProductVariantId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}

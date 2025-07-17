using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.Domain;

public class ProductVariantPriceEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductVariantPrice>
{
    public override void Configure(EntityTypeBuilder<ProductVariantPrice> builder)
    {
        base.Configure(builder);

        builder.Property(pvp => pvp.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(pvp => pvp.Currency).IsRequired().HasDefaultValue("تومان");
        builder.Property(pvp => pvp.EffectiveFrom).IsRequired();
        builder.Property(pvp => pvp.EffectiveTo); // Nullable for current price

        builder.HasOne(pvp => pvp.ProductVariant)
            .WithMany(pv => pv.Prices)
            .HasForeignKey(pvp => pvp.ProductVariantId)
            .IsRequired();
    }
}
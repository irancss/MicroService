using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class ProductImageEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductImage>
{
    public override void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        base.Configure(builder);

        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(1024);
        builder.Property(pi => pi.AltText).HasMaxLength(255);
        builder.Property(pi => pi.IsPrimary).IsRequired();
        builder.Property(pi => pi.DisplayOrder).IsRequired();

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .IsRequired();
    }
}

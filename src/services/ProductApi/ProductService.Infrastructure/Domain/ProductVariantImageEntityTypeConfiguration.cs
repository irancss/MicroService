using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Domain;

namespace ProductService.Infrastructure.Domain
{
    public class ProductVariantImageEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductVariantImage>
    {
        public override void Configure(EntityTypeBuilder<ProductVariantImage> builder)
        {
            base.Configure(builder);

            builder.Property(pvi => pvi.ImageUrl).IsRequired().HasMaxLength(1024);
            builder.Property(pvi => pvi.AltText).HasMaxLength(255);
            builder.Property(pvi => pvi.IsPrimary).IsRequired();
            builder.Property(pvi => pvi.DisplayOrder).IsRequired();

            builder.HasOne(pvi => pvi.ProductVariant)
                .WithMany(pv => pv.Images)
                .HasForeignKey(pvi => pvi.ProductVariantId)
                .IsRequired();
        }
    }
}

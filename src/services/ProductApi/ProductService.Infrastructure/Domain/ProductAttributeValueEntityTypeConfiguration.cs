using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.Domain
{
    public class ProductAttributeValueEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductAttributeValue>
    {
        public override void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            base.Configure(builder);

            builder.Property(pav => pav.Value)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(pav => pav.ProductAttribute)
                .WithMany(pa => pa.PossibleValues)
                .HasForeignKey(pav => pav.ProductAttributeId)
                .IsRequired();

            builder.HasOne(pav => pav.ProductVariant)
                .WithMany(pv => pv.DefiningAttributes)
                .HasForeignKey(pav => pav.ProductVariantId)
                .IsRequired(false);
        }
    }
}

using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class ProductDescriptiveAttributeEntityTypeConfiguration 
    : BaseEntityTypeConfiguration<ProductDescriptiveAttribute>
{
    public override void Configure(EntityTypeBuilder<ProductDescriptiveAttribute> builder)
    {
        base.Configure(builder);

        builder.Property(pda => pda.AttributeName).IsRequired().HasMaxLength(100);
        builder.Property(pda => pda.AttributeValue).IsRequired().HasMaxLength(500);

        builder.HasOne(pda => pda.Product)
            .WithMany(p => p.DescriptiveAttributes)
            .HasForeignKey(pda => pda.ProductId)
            .IsRequired();
    }
}

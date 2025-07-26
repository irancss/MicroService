using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain
{
    public class ProductSpecificationEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductSpecification>
    {
        public override void Configure(EntityTypeBuilder<ProductSpecification> builder)
        {
            base.Configure(builder);

            builder.Property(ps => ps.SpecificationName).IsRequired().HasMaxLength(100);
            builder.Property(ps => ps.SpecificationValue).IsRequired().HasMaxLength(500);
            builder.Property(ps => ps.DisplayOrder).IsRequired();

            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.Specifications)
                .HasForeignKey(ps => ps.ProductId)
                .IsRequired();
        }
    }
}

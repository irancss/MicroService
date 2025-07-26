using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain
{
    public class BrandEntityTypeConfiguration : BaseEntityTypeConfiguration<Brand>
    {
        public override void Configure(EntityTypeBuilder<Brand> builder)
        {
            base.Configure(builder);

            builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(b => b.Name).IsUnique();
            builder.Property(b => b.Description).HasMaxLength(500);
            builder.Property(b => b.LogoUrl).HasMaxLength(500);
        }
    }
}

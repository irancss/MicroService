using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Domain
{
    public class ProductEntityTypeConfiguration : BaseEntityTypeConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder); // تنظیمات پایه را اعمال می‌کند

            builder.Ignore(p => p.DomainEvents);

            builder.Property(p => p.Name)
                .HasConversion(
                    productName => productName.Value,
                    value => ProductName.Create(value)
                )
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Sku).HasMaxLength(100);
            builder.HasIndex(p => p.Sku).IsUnique().HasFilter("\"Sku\" IS NOT NULL");
            builder.Property(p => p.StockQuantity).IsRequired();
            builder.Property(p => p.Brand).HasMaxLength(100);
            builder.Property(p => p.Weight).HasColumnType("decimal(18,3)");

            builder.OwnsOne(p => p.Dimensions, dim =>
            {
                dim.Property(d => d.Length).HasColumnType("decimal(18,2)").HasColumnName("DimensionLength");
                dim.Property(d => d.Width).HasColumnType("decimal(18,2)").HasColumnName("DimensionWidth");
                dim.Property(d => d.Height).HasColumnType("decimal(18,2)").HasColumnName("DimensionHeight");
            });

            builder.Property(p => p.VendorId).HasMaxLength(100);
            builder.Property(p => p.Manufacturer).HasMaxLength(100);
            builder.Property(p => p.IsActive).IsRequired();

            // روابط
            builder.HasMany(p => p.Questions)
                .WithOne(q => q.Product)
                .HasForeignKey(q => q.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Variants)
                .WithOne(pv => pv.Product)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ScheduledDiscounts)
                .WithOne(sd => sd.Product)
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

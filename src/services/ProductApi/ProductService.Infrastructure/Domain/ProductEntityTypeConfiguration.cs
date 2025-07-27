using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Domain
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            // پیکربندی Value Object ها
            builder.Property(p => p.Name)
                .HasConversion(name => name.Value, value => ProductName.For(value))
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Price)
                .HasConversion(price => price.Value, value => ProductPrice.For(value))
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Sku)
                .HasConversion(sku => sku.Value, value => Sku.For(value))
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.Sku).IsUnique();

            // روابط
            builder.HasOne(p => p.Brand)
                .WithMany() // یک برند می‌تواند محصولات زیادی داشته باشد
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.SetNull); // اگر برند حذف شد، محصولات بدون برند شوند

            // رابطه چند به چند با Category
            builder.HasMany(p => p.ProductCategories)
                .WithOne(pc => pc.Product)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // رابطه چند به چند با Tag
            builder.HasMany(p => p.ProductTags)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // نادیده گرفتن رویدادهای دامنه
            builder.Ignore(p => p.DomainEvents);
        }
    }
}

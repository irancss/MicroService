using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Infrastructure.Data.Configurations
{
    public class NextPurchaseCartConfiguration : IEntityTypeConfiguration<NextPurchaseCart>
    {
        public void Configure(EntityTypeBuilder<NextPurchaseCart> builder)
        {
            builder.ToTable("NextPurchaseCarts");

            builder.HasKey(c => c.Id);

            // UserId is the primary key and is not auto-generated
            builder.Property(c => c.Id).ValueGeneratedNever();

            // Configure the ownership of NextPurchaseItem
            builder.OwnsMany(c => c.Items, item =>
            {
                item.ToTable("NextPurchaseItems");
                item.WithOwner().HasForeignKey("NextPurchaseCartId");
                item.HasKey("Id"); // EF Core needs a key for owned collections

                item.Property(i => i.ProductId).IsRequired().HasMaxLength(100);
                item.Property(i => i.VariantId).HasMaxLength(100);
                item.Property(i => i.ProductName).IsRequired().HasMaxLength(255);
                item.Property(i => i.SavedPrice).HasColumnType("decimal(18,2)");
            });

            // AuditableEntity properties are configured in BuildingBlocks
        }
    }
}

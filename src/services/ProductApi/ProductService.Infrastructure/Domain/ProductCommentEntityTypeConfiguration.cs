using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductService.Infrastructure.Domain
{
    public class ProductCommentEntityTypeConfiguration : BaseEntityTypeConfiguration<ProductComment>
    {
        public override void Configure(EntityTypeBuilder<ProductComment> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.ProductId).IsRequired();
            builder.Property(c => c.UserId).IsRequired();
            builder.Property(c => c.Content).IsRequired().HasMaxLength(1500);
            builder.Property(c => c.Status).IsRequired();
            builder.Property(c => c.StatusCommentUser).IsRequired();

            builder.HasIndex(c => new { c.ProductId, c.UserId }).IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Domain;

namespace ProductService.Infrastructure.Domain;

public class TagEntityTypeConfiguration : BaseEntityTypeConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.Name).IsUnique();
        // No need to configure CreatedAt, UpdatedAt, etc. again
    }
}

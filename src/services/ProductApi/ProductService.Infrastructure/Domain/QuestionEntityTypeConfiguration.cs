using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.Domain
{
    public class QuestionEntityTypeConfiguration : BaseEntityTypeConfiguration<Question>
    {
        public override void Configure(EntityTypeBuilder<Question> builder)
        {
            base.Configure(builder);

            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(q => q.UserId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(q => q.IsApproved)
                .IsRequired();

            builder.Ignore(q => q.IsAnswered);

            builder.HasOne(q => q.Product)
                .WithMany(p => p.Questions)
                .HasForeignKey(q => q.ProductId)
                .IsRequired();
        }
    }
}
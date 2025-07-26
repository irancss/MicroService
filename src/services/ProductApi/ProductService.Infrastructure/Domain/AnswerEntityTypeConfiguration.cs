using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ProductService.Infrastructure.Domain
{
    public class AnswerEntityTypeConfiguration : BaseEntityTypeConfiguration<Answer>
    {
        public override void Configure(EntityTypeBuilder<Answer> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.AnswerText)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(100); // Adjust if UserId is not string

            builder.Property(a => a.IsApproved)
                .IsRequired();

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
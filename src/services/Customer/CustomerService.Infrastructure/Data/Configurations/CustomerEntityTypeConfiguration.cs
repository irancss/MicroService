using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerService.Infrastructure.Data.Configurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.PhoneNumber)
                .HasMaxLength(20);

            // Configure owned entity for Address
            builder.OwnsMany(c => c.Addresses, a =>
            {
                a.ToTable("Addresses");
                a.WithOwner().HasForeignKey("CustomerId");
                a.Property(addr => addr.Street).HasMaxLength(200).IsRequired();
                a.Property(addr => addr.City).HasMaxLength(100).IsRequired();
                a.Property(addr => addr.State).HasMaxLength(100).IsRequired();
                a.Property(addr => addr.Country).HasMaxLength(100).IsRequired();
                a.Property(addr => addr.ZipCode).HasMaxLength(20).IsRequired();
            });
        }
    }
}

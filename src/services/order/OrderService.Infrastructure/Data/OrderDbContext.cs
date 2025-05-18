using Microsoft.EntityFrameworkCore;
using OrderService.Core.Models;

namespace OrderService.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example configuration for PostgreSQL (snake_case naming)
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).HasColumnName("id");
                entity.Property(o => o.CustomerId).HasColumnName("customer_id");
                entity.Property(o => o.OrderDate).HasColumnName("order_date");
                // Add other property mappings as needed
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Id).HasColumnName("id");
                entity.Property(oi => oi.OrderId).HasColumnName("order_id");
                entity.Property(oi => oi.ProductId).HasColumnName("product_id");
                entity.Property(oi => oi.Quantity).HasColumnName("quantity");
                // Add other property mappings as needed
            });
        }
    }
}

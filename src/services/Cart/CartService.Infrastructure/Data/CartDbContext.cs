using BuildingBlocks.Application.Data;
using BuildingBlocks.Messaging.Persistence;
using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Data
{
    public class CartDbContext : DbContext, IApplicationDbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<NextPurchaseCart> NextPurchaseCarts { get; set; }

        // پیاده‌سازی اعضای IApplicationDbContext
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<StoredEvent> StoredEvents { get; set; } // برای Event Sourcing/Auditing

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);

            // پیکربندی برای موجودیت‌های Outbox و StoredEvent
            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.ToTable("OutboxMessages");
                builder.HasKey(e => e.Id);
            });

            modelBuilder.Entity<StoredEvent>(builder =>
            {
                builder.ToTable("StoredEvents");
                builder.HasKey(e => e.Id);
            });
        }
    }
}

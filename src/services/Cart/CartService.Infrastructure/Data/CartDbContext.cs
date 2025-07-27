using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<NextPurchaseCart> NextPurchaseCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        }
    }
}

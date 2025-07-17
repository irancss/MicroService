

using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Models;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply configurations for entities here
        // e.g., modelBuilder.ApplyConfiguration(new YourEntityConfiguration());
    }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    // Define DbSet properties for your entities
    // public DbSet<YourEntity> YourEntities { get; set; }
}
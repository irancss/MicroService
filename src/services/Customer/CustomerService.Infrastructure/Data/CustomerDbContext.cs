using BuildingBlocks.Extensions;
using BuildingBlocks.Infrastructure.Data;
using BuildingBlocks.Messaging.Persistence;
using CustomerService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CustomerService.Infrastructure.Data
{
    public class CustomerDbContext : DbContext, IApplicationDbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }


        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        
    }
}

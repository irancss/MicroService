using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Moq; // از پکیج Moq برای ساخت mock استفاده می‌کنیم

namespace CustomerService.Infrastructure.Data
{
    public class CustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // یا appsettings.json
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString,
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(CustomerDbContext).Assembly.FullName);
                });

            // DbContext دیگر به IMediator نیاز ندارد
            return new CustomerDbContext(optionsBuilder.Options);
        }
    }
}

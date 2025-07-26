using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer.Data.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // ۱. یک Configuration Builder بسازید تا بتوانید به appsettings.json دسترسی پیدا کنید.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            // ۲. Connection String را مستقیماً از Configuration بخوانید.
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'DefaultConnection' was not found in configuration.");
            }

            // ۳. یک DbContextOptionsBuilder جدید بسازید.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // ۴. به آن بگویید که از PostgreSQL با Connection String خوانده شده استفاده کند.
            optionsBuilder.UseNpgsql(connectionString);

            // ۵. یک نمونه از DbContext خود را با این آپشن‌ها بسازید و برگردانید.
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

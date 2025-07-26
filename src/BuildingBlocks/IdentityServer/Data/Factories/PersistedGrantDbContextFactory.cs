using IdentityServer8.EntityFramework.DbContexts;
using IdentityServer8.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer.Data.Factories
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString, dbOpts =>
                dbOpts.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

            return new PersistedGrantDbContext(builder.Options, new OperationalStoreOptions());
        }
    }
}

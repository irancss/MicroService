namespace IdentityServer.Data.Factories
{
    // در فایل Data/Factories/ConfigurationDbContextFactory.cs

    using IdentityServer8.EntityFramework.DbContexts;
    using IdentityServer8.EntityFramework.Options;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using System.IO;


    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString, dbOpts =>
                dbOpts.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));

            return new ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());
        }
    }
}

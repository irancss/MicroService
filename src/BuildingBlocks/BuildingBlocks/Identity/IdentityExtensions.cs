using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer8.EntityFramework.DbContexts;
using IdentityServer8.EntityFramework.Mappers;
using IdentityServer8.Models;
using IdentityServer8.Services;
using System.Security.Claims;

namespace BuildingBlocks.Identity
{
    /// <summary>
    /// Extension methods for configuring IdentityServer 8 with ASP.NET Core Identity
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Adds IdentityServer 8 with PostgreSQL persistence and ASP.NET Core Identity integration
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddIdentityServer8(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=localhost;Database=IdentityServerDb;Username=postgres;Password=postgres";

            // Add Entity Framework contexts
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDbContext<PersistedGrantDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDbContext<ConfigurationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Add ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add IdentityServer 8
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                options.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<ApplicationUser>();

            // Development signing credential (use proper certificate in production)
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                // TODO: Add production certificate
                // builder.AddSigningCredential(certificate);
                throw new Exception("Need to configure key material for production");
            }

            return services;
        }

        /// <summary>
        /// Configures IdentityServer 8 middleware pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseIdentityServer8(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
            return app;
        }

        /// <summary>
        /// Seeds the database with initial configuration data
        /// </summary>
        /// <param name="app">The application</param>
        public static async Task SeedIdentityServerData(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            await context.Database.MigrateAsync();

            if (!context.Clients.Any())
            {
                foreach (var client in GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scope in GetApiScopes())
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }

        private static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // Machine to machine client
                new Client
                {
                    ClientId = "microservice.client",
                    ClientName = "Microservice Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "api1", "api2" }
                },

                // Interactive ASP.NET Core Web App
                new Client
                {
                    ClientId = "web.client",
                    ClientName = "Web Application",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "api1" },
                    AllowOfflineAccess = true
                }
            };
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "User Roles", new[] { ClaimTypes.Role })
            };
        }

        private static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("api1", "API 1"),
                new ApiScope("api2", "API 2")
            };
        }
    }

    /// <summary>
    /// Application user for ASP.NET Core Identity
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Application database context
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional configuration can be added here
        }
    }
}

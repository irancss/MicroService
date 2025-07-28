using BuildingBlocks.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Services;

namespace ProductService.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // ثبت DbContext و UnitOfWork با استفاده از متد کمکی BuildingBlocks
            var connectionString = configuration.GetConnectionString("PostgresConnection");
            services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddUnitOfWorkAndDbContext<ProductDbContext>(); // این متد را در فاز 1 ساختیم

            // ثبت Repository ها
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // ثبت سرویس Seed کننده دیتابیس
            services.AddScoped<ApplicationDbContextInitialiser>();

            return services;
        }

        // متد کمکی برای اجرای Seed در Program.cs
        public static async Task InitialiseDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
    }
}

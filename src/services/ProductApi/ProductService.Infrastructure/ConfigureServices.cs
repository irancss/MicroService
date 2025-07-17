using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Services;

namespace ProductService.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();


            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=ProductDb;Username=postgres;Password=123");
            });
            return services;
        }
    }
}

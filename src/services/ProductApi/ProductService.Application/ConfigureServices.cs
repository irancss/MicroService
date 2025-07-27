using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;
using MediatR;
using System.Reflection;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Domain.Models;

namespace ProductService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddScoped<IBrandService , BrandService>();
            //services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAnswerService , AnswerService>();
            services.AddScoped<IBrandService, BrandService>();

            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register MediatR services
            services.AddMediatR(Assembly.GetExecutingAssembly());

            
            return services;
        }
        public class AssemblyReference { }
    }
}

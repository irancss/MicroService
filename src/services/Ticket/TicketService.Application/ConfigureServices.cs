using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;
using TicketService.Application.Interfaces;

namespace TicketService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITicketService, Services.TicketService>();

            services.AddScoped<ISieveProcessor, SieveProcessor>();
            services.Configure<SieveOptions>(options =>
            {
                options.CaseSensitive = false;
                options.DefaultPageSize = 10;
                options.MaxPageSize = 100;
                options.ThrowExceptions = true;
            });
            services.AddScoped<SieveProcessor>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using MarketingService.Infrastructure.Data;
using MarketingService.Infrastructure.Repositories;
using MarketingService.Infrastructure.BackgroundJobs;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<MarketingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<ILandingPageRepository, LandingPageRepository>();
        services.AddScoped<IUserSegmentRepository, UserSegmentRepository>();
        services.AddScoped<IUserSegmentMembershipRepository, UserSegmentMembershipRepository>();

        // Background Jobs with Hangfire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();
        
        services.AddScoped<UserSegmentationJob>();

        // Message Bus with MassTransit
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ"));
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
